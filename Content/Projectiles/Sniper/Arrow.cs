using Terraria;
using Terraria.ID;

namespace TF2.Content.Projectiles.Sniper
{
    public class Arrow : Bullet
    {
        protected override void ProjectileStatistics()
        {
            SetProjectileSize(40, 10);
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
    }
}