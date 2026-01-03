using Microsoft.Xna.Framework;
using Terraria;
using TF2.Content.Projectiles.Soldier;

namespace TF2.Content.Projectiles.NPCs
{
    public class EnemyRocket : Rocket
    {
        public override string Texture => "TF2/Content/Projectiles/Soldier/Rocket";

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(40, 12);
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override void RocketJump(Vector2 velocity) => DetonateProjectile();
    }
}