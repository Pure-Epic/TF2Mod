using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace TF2.Content.Projectiles.Soldier
{
    public class RocketJumperRocket : Rocket
    {
        public override string Texture => "TF2/Content/Projectiles/Soldier/Rocket";

        protected override void ProjectileAI()
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (Projectile.Hitbox.Intersects(player.Hitbox) && player.whoAmI != Projectile.owner)
                    Projectile.timeLeft = 0;
            }
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (Projectile.Hitbox.Intersects(npc.Hitbox))
                    Projectile.timeLeft = 0;

            }
            if (Projectile.timeLeft == 0)
            {
                Projectile.position = Projectile.Center;
                Projectile.Size = new Vector2(100, 100);
                Projectile.hide = true;
                Projectile.tileCollide = false;
                Projectile.Center = Projectile.position;
            }
            SetRotation();
        }

        protected override bool ProjectileTileCollide(Vector2 oldVelocity)
        {
            if (FindOwner(Projectile, 50f))
            {
                oldVelocity *= 1.25f;
                oldVelocity.X = Utils.Clamp(oldVelocity.X, -25f, 25f);
                oldVelocity.Y = Utils.Clamp(oldVelocity.Y, -25f, 25f);
                Player.velocity -= oldVelocity;
                QuickFixMirror();
            }
            return false;
        }

        public void Explode()
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/rocket_jumper_explode"), Projectile.Center);
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
    }
}