using Microsoft.Xna.Framework;
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

        protected override void ProjectileAI()
        {
            base.ProjectileAI();
            if (ignited)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 5f;
                Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3());
            }
        }
    }
}