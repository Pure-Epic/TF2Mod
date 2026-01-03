using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Weapons.Soldier;

namespace TF2.Content.Buffs
{
    public class BattalionsBackupBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<BattalionsBackupPlayer>().bannerBuff = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<BattalionsBackupBuffNPC>().battalionsBackupBuff = true;
    }

    public class BattalionsBackupBuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool battalionsBackupBuff;

        public override void ResetEffects(NPC npc) => battalionsBackupBuff = false;

        public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.GetGlobalNPC<BattalionsBackupBuffNPC>().battalionsBackupBuff)
                modifiers.FinalDamage *= 0.65f;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (battalionsBackupBuff)
                modifiers.FinalDamage *= 0.5f;
        }
    }
}