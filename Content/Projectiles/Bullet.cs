using Terraria;
using Terraria.ID;
using TF2.Common;

namespace TF2.Content.Projectiles
{
    public class Bullet : TF2Projectile
    {
        protected override void ProjectileStatistics()
        {
            SetProjectileSize(50, 4);
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.penetrate = 1;
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
            Projectile.penetrate = Player.GetModPlayer<TF2Player>().pierce;
            projectileInitialized = true;
            return true;
        }

        protected override void ProjectileAI() => SetRotation();
    }
}