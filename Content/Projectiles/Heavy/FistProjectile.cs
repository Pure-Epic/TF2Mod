using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Heavy
{
    public class FistProjectile : ModProjectile
    {
        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public float CollisionWidth => 10f * Projectile.scale;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(50);
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.alpha = 128;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 360;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool PreDraw(ref Color lightColor) => TF2.DrawProjectile(Projectile, ref lightColor);

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Timer += 1;
            if (Timer >= 16)
            {
                Projectile.Kill();
                return;
            }
            else
                player.heldProj = Projectile.whoAmI;

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Main.player[Projectile.owner].AddBuff(ModContent.BuffType<HeavyCrit>(), 300);
    }

    public class GlovesOfRunningUrgentlyProjectile : FistProjectile
    { }

    public class FistsOfSteelProjectile : FistProjectile
    { }

    public class WarriorsSpiritProjectile : FistProjectile
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (hit.Crit && target.type != NPCID.TargetDummy && player.statLife < player.statLifeMax2)
                player.Heal((int)(player.statLifeMax2 * 0.16666666667f));
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!info.PvP) return;
            Player player = Main.player[Projectile.owner];
            if (player.GetModPlayer<TF2Player>().crit && player.statLife < player.statLifeMax2)
                player.Heal((int)(player.statLifeMax2 * 0.16666666667f));
        }
    }
}