using Terraria;
using Terraria.ID;
using TF2.Content.Projectiles.Pyro;

namespace TF2.Content.Projectiles.NPCs
{
    public class EnemyFireNPC : Fire
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Flames;

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(6, 6);
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = TF2.Time(1);
            Projectile.alpha = 255;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
    }
}