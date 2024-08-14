using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Weapons.Soldier;

namespace TF2.Content.Buffs
{
    public class DefenseRage : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<BattalionsBackupPlayer>().buffActive = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<DefenseRageNPC>().defenseRageBuff = true;
    }

    public class DefenseRageNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool defenseRageBuff;

        public override void ResetEffects(NPC npc) => defenseRageBuff = false;

        public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.GetGlobalNPC<DefenseRageNPC>().defenseRageBuff)
                modifiers.FinalDamage *= 0.65f;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (defenseRageBuff)
                modifiers.FinalDamage *= 0.5f;
        }
    }
}