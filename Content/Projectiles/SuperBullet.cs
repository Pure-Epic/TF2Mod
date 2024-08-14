using Terraria;
using Terraria.ID;

namespace TF2.Content.Projectiles
{
    public class SuperBullet : Bullet
    {
        public override string Texture => "TF2/Content/Projectiles/Bullet";

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(50, 4);
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = TF2.Time(6);
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 9;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override bool ProjectilePreAI()
        {
            if (projectileInitialized) return true;
            projectileInitialized = true;
            return true;
        }

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.CritDamage.Flat = 9001f;
    }
}