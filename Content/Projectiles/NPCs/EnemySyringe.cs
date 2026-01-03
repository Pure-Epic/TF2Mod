using Terraria;
using Terraria.ID;

namespace TF2.Content.Projectiles.NPCs
{
    public class EnemySyringe : EnemyBullet
    {
        public override string Texture => "TF2/Content/Projectiles/Medic/Syringe";

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(50, 8);
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.penetrate = 1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
    }
}