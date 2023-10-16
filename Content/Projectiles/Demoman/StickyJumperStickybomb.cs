using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace TF2.Content.Projectiles.Demoman
{
    public class StickyJumperStickybomb : Stickybomb
    {
        public override void AI()
        {
            Timer++;
            if (Projectile.timeLeft == 0)
            {
                Projectile.tileCollide = false;
                Projectile.alpha = 255;
                Projectile.position = Projectile.Center;
                Projectile.width = 250;
                Projectile.height = 250;
                Projectile.Center = Projectile.position;
                if (TF2.FindPlayer(Projectile, 50f))
                {
                    velocity *= 2.5f;
                    velocity.X = Utils.Clamp(velocity.X, -25f, 25f);
                    velocity.Y = Utils.Clamp(velocity.Y, -25f, 25f);
                    Main.player[Projectile.owner].velocity -= velocity;
                }
            }
            if (!Stick && Projectile.timeLeft != 0)
                StartingAI();
            else
                GroundAI();
        }

        public override void OnKill(int timeLeft) => Explode();

        public void Explode()
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/sticky_jumper_explode1"), Projectile.Center);
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