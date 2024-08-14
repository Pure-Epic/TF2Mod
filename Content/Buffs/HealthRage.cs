using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Soldier;
using TF2.Content.NPCs.Buddies;
using TF2.Content.Projectiles;

namespace TF2.Content.Buffs
{
    public class HealthRage : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<ConcherorPlayer>().buffActive = true;
            TF2Player.SetPlayerSpeed(player, 135);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.ModNPC is MercenaryBuddy buddy)
                buddy.temporarySpeedMultiplier *= 10f;
        }
    }

    public class HealthRageNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.ModProjectile is TF2Projectile tf2Projectile)
            {
                NPC healedNPC = Main.npc[tf2Projectile.npcOwner];
                if (healedNPC.active && healedNPC.HasBuff<HealthRage>() && healedNPC.ModNPC is MercenaryBuddy buddy)
                    buddy.Heal(TF2.Round(healedNPC.lifeMax * 0.35f));
            }
        }
    }
}