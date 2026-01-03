using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TF2.Common;
using TF2.Content.NPCs.Buddies;

namespace TF2.Content.Projectiles.NPCs
{
    public class BuddyHealingBeam : TF2Projectile
    {
        public override string Texture => "TF2/Content/Items/Weapons/Medic/MediGun";

        protected virtual float HealMultiplier => 1f;

        protected virtual float UberchargeMultiplier => 1f;

        public virtual float OverhealLimit => 0.5f;

        public int HealCooldown
        {
            get => (Main.npc[npcOwner].ModNPC as MedicBuddyNPC).healTimer;
            set => (Main.npc[npcOwner].ModNPC as MedicBuddyNPC).healTimer = value;
        }

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(50, 50);
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            healingProjectile = true;
        }

        protected override void ProjectileAI()
        {
            NPC npc = Main.npc[npcOwner];
            if (npc != null && npc.active && npc.ModNPC is MedicBuddyNPC medic)
            {
                Projectile.Center = Player.Center;
                HealPlayer(Player);
                medic.isHealing = true;
                if (Player.moveSpeed > medic.BaseSpeed * medic.speedMultiplier)
                    medic.healingMovementSpeed = Player.moveSpeed;
                Projectile.timeLeft = TF2.Time(0.05);
                if (!medic.canHeal)
                    Projectile.Kill();
                float length = Vector2.Distance(npc.Center, Player.Center);
                for (int i = 0; i < TF2.Round(length / 25f); i++)
                {
                    Vector2 position = TF2.Lerp(npc.Center, Player.Center, Main.rand.NextFloat());
                    Dust dust = Main.dust[Dust.NewDust(position - Vector2.One * 10, 20, 20, DustID.RedTorch)];
                    dust.velocity = Player.velocity;
                    dust.noGravity = true;
                    dust.alpha = 128;
                    dust.scale = Main.rand.Next(10, 20) * 0.1f;
                }
            }
            else
                Projectile.Kill();
        }

        public override bool ShouldUpdatePosition() => false;

        protected void HealPlayer(Player target)
        {
            if (Main.npc[npcOwner].ModNPC is MedicBuddyNPC && HealCooldown <= 0)
            {
                int healingAmount = TF2.Round(TF2.GetRawHealth(target, 1.2f * target.GetModPlayer<TF2Player>().HealPenaltyMultiplier * HealMultiplier));
                if (healingAmount <= 0)
                    healingAmount = 1;
                if (Main.netMode == NetmodeID.SinglePlayer)
                    TF2.Overheal(target, healingAmount, OverhealLimit);
                else
                    TF2.OverhealMultiplayer(target, healingAmount, OverhealLimit);
                HealCooldown = TF2.Time(0.1);
            }
        }
    }
}