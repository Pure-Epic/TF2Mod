using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TF2.Common;

namespace TF2.Content.Projectiles.Spy
{
    public class YourEternalRewardBeam : TF2Projectile
    {
        protected override void ProjectileStatistics()
        {
            SetProjectileSize(40, 14);
            AIType = ProjectileID.Bullet;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = TF2.Time(6);
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override bool ProjectilePreAI()
        {
            if (projectileInitialized) return true;
            TF2Player p = Player.GetModPlayer<TF2Player>();
            Projectile.penetrate = p.pierce;
            projectileInitialized = true;
            return true;
        }

        protected override void ProjectileAI()
        {
            SetRotation();
            for (int i = 0; i < 10; i++)
            {
                float x2 = Projectile.Center.X - Projectile.velocity.X / 10f * i;
                float y2 = Projectile.Center.Y - Projectile.velocity.Y / 10f * i;
                int num179 = Dust.NewDust(new Vector2(x2, y2), 1, 1, DustID.IceTorch);
                Main.dust[num179].alpha = Projectile.alpha;
                Main.dust[num179].position.X = x2;
                Main.dust[num179].position.Y = y2;
                Main.dust[num179].velocity *= 0f;
                Main.dust[num179].noGravity = true;
            }
        }

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info) => homing = false;

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => homing = false;
    }
}