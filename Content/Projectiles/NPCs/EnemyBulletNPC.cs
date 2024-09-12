using Terraria;
using Terraria.ID;

namespace TF2.Content.Projectiles.NPCs
{
    public class EnemyBulletNPC : TF2Projectile
    {
        public override string Texture => "TF2/Content/Projectiles/Bullet";

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(50, 4);
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.penetrate = 1;
            Projectile.hostile = true;
            Projectile.timeLeft = TF2.Time(6);
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 9;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override void ProjectileAI() => SetRotation();
    }
}