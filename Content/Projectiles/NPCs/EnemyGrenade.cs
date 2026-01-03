using Microsoft.Xna.Framework;
using Terraria;
using TF2.Content.Projectiles.Demoman;

namespace TF2.Content.Projectiles.NPCs
{
    public class EnemyGrenade : Grenade
    {
        protected override void ProjectileStatistics()
        {
            SetProjectileSize(25, 15);
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override string Texture => "TF2/Content/Projectiles/Demoman/Grenade";

        protected override void GrenadeJump(Vector2 velocity) => DetonateProjectile();
    }
}