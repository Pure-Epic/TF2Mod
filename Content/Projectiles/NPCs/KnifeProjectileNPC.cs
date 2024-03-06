using Microsoft.Xna.Framework;
using Terraria;

namespace TF2.Content.Projectiles.NPCs
{
    public class KnifeProjectileNPC : TF2Projectile
    {
        public NPC thisNPC;

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(100, 100);
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = TF2.Time(6);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override void ProjectileAI()
        {
            if (thisNPC == null) return;
            Vector2 npcCenter = thisNPC.position;
            Projectile.position = npcCenter;
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

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.immune[Projectile.owner] = 48;
    }
}