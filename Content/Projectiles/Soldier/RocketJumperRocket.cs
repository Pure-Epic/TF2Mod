using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace TF2.Content.Projectiles.Soldier
{
    public class RocketJumperRocket : Rocket
    {
        public override string Texture => "TF2/Content/Projectiles/Soldier/Rocket";

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.position = Projectile.Center;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.Center = Projectile.position;
            Projectile.timeLeft = 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (TF2.FindPlayer(Projectile, 50f))
            {
                oldVelocity *= 5f;
                oldVelocity.X = Utils.Clamp(oldVelocity.X, -25f, 25f);
                oldVelocity.Y = Utils.Clamp(oldVelocity.Y, -25f, 25f);
                Main.player[Projectile.owner].velocity -= oldVelocity;
            }
            prime = true;
            return false;
        }

        public override void AI()
        {
            if (prime)
                Projectile.timeLeft = 0;
            if (Projectile.timeLeft == 0)
            {
                Projectile.tileCollide = false;
                Projectile.alpha = 255;
                Projectile.position = Projectile.Center;
                Projectile.width = 30;
                Projectile.height = 30;
                Projectile.Center = Projectile.position;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public void Explode()
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/rocket_jumper_explode1"), Projectile.Center);
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

        public override void RocketJump(Vector2 velocity) => base.RocketJump(velocity);
    }
}