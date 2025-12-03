using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Heavy
{
    public class FistProjectile : TF2Projectile
    {
        public float CollisionWidth => 10f * Projectile.scale;

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(50, 50);
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = TF2.Time(6);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.alpha = 128;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override void ProjectileAI()
        {
            Timer += 1;
            if (Timer >= 16)
            {
                Projectile.Kill();
                return;
            }
            else
                Player.heldProj = Projectile.whoAmI;
            Vector2 playerCenter = Player.RotatedRelativePoint(Player.MountedCenter, reverseRotation: false, addGfxOffY: false);
            Projectile.Center = playerCenter + Projectile.velocity * 2f * (Timer - 1f);
            Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;
            SetVisualOffsets();
        }

        private void SetVisualOffsets()
        {
            const int HalfSpriteWidth = 32 / 2;
            const int HalfSpriteHeight = 32 / 2;

            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;

            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * 6f;
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
        }
    }

    public class KillingGlovesOfBoxingProjectile : FistProjectile
    {
        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info) => Player.AddBuff(ModContent.BuffType<KillingGlovesOfBoxingBuff>(), TF2.Time(5));

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Player.AddBuff(ModContent.BuffType<KillingGlovesOfBoxingBuff>(), TF2.Time(5));
    }

    public class GlovesOfRunningUrgentlyProjectile : FistProjectile
    { }

    public class FistsOfSteelProjectile : FistProjectile
    { }

    public class WarriorsSpiritProjectile : FistProjectile
    {
        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!info.PvP) return;
            if ((Player.GetModPlayer<TF2Player>().crit || crit) && !TF2Player.IsHealthFull(Player))
                Player.Heal(TF2.GetHealth(Player, 50));
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit && target.type != NPCID.TargetDummy && !TF2Player.IsHealthFull(Player))
                Player.Heal(TF2.GetHealth(Player, 50));
        }
    }

    public class EvictionNoticeProjectile : FistProjectile
    {
        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!info.PvP) return;
            Player.AddBuff(ModContent.BuffType<EvictionNoticeBuff>(), TF2.Time(3));
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type != NPCID.TargetDummy)
                Player.AddBuff(ModContent.BuffType<EvictionNoticeBuff>(), TF2.Time(3));
        }
    }
}