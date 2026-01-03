using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace TF2.Content.Projectiles.Demoman
{
    public class StickyJumperStickybomb : Stickybomb
    {
        protected override void ProjectileStatistics()
        {
            SetProjectileSize(22, 22);
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Stick = false;
        }

        protected override void ProjectileAI()
        {
            if (ProjectileDetonation)
            {
                Projectile.position = Projectile.Center;
                Projectile.Size = new Vector2(250, 250);
                Projectile.hide = true;
                Projectile.tileCollide = false;
                Projectile.tileCollide = false;
                Projectile.Center = Projectile.position;
                if (FindOwner(Projectile, 100f))
                {
                    velocity *= 2.5f;
                    velocity.X = Utils.Clamp(velocity.X, -15f, 15f);
                    velocity.Y = Utils.Clamp(velocity.Y, -15f, 15f);
                    Player.velocity -= velocity;
                    QuickFixMirror();
                }
            }
            if (!Stick && !ProjectileDetonation)
                StartingAI();
            else
                GroundAI();
        }

        protected override void ProjectileDestroy(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/sticky_jumper_explode"), Projectile.Center);
            for (int i = 0; i < 50; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                dust.velocity *= 1.4f;
            }
            for (int i = 0; i < 80; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
                dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
            }
            Lighting.AddLight(Projectile.Center, new Vector3(255f, 190f, 0f));
        }
    }
}