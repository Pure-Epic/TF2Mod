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
                    DetonateProjectile();
            }
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (Projectile.Hitbox.Intersects(npc.Hitbox))
                    DetonateProjectile();

            }
            if (ProjectileDetonation)
            {
                Projectile.position = Projectile.Center;
                Projectile.Size = new Vector2(100, 100);
                Projectile.hide = true;
                Projectile.tileCollide = false;
                Projectile.Center = Projectile.position;
                RocketJump(Projectile.velocity);
            }
            SetRotation();
        }

        protected override bool ProjectileTileCollide(Vector2 oldVelocity)
        {
            DetonateProjectile();
            return false;
        }

        protected override void ProjectileDestroy(int timeLeft)
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
            Lighting.AddLight(Projectile.Center, new Vector3(255f, 190f, 0f));
        }

        protected override void RocketJump(Vector2 velocity)
        {
            if (FindOwner(Projectile, 100f))
            {
                velocity.X = Utils.Clamp(velocity.X, -15f, 15f);
                velocity.Y = Utils.Clamp(velocity.Y, -15f, 15f);
                Player.velocity -= velocity;
                QuickFixMirror();
            }
        }
    }
}