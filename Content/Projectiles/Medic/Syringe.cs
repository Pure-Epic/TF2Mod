using Terraria;
using Terraria.ID;

namespace TF2.Content.Projectiles.Medic
{
    public class Syringe : Bullet
    {
        protected override void ProjectileStatistics()
        {
            SetProjectileSize(30, 10);
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
    }
}