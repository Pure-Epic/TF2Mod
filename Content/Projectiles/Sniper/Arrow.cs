using Terraria;
using Terraria.ID;

namespace TF2.Content.Projectiles.Sniper
{
    public class Arrow : Bullet
    {
        protected override void ProjectileStatistics()
        {
            SetProjectileSize(40, 5);
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
    }
}