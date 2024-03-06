using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace TF2.Content.Projectiles.Demoman
{
    public class StickyJumperStickybomb : Stickybomb
    {
        protected override void ProjectileAI()
        {
            if (Projectile.timeLeft == 0)
            {
                Projectile.position = Projectile.Center;
                Projectile.Size = new Vector2(250, 250);
                Projectile.hide = true;
                Projectile.tileCollide = false;
                Projectile.tileCollide = false;
                Projectile.Center = Projectile.position;
                if (TF2.FindPlayer(Projectile, 50f))
                {
                    velocity *= 2.5f;
                    velocity.X = Utils.Clamp(velocity.X, -25f, 25f);
                    velocity.Y = Utils.Clamp(velocity.Y, -25f, 25f);
                    Player.velocity -= velocity;
                }
            }
            if (!Stick && Projectile.timeLeft != 0)
                StartingAI();
            else
                GroundAI();
        }

        protected override void ProjectileDestroy(int timeLeft) => Explode();

        public void Explode()
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
        }

        public override void StickyJump(Vector2 velocity) => base.StickyJump(velocity);
    }
}