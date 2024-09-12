using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace TF2.Content.Projectiles.Pyro
{
    public class DetonatorFlare : Flare
    {
        protected override void ProjectileStatistics()
        {
            SetProjectileSize(24, 14);
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override void ProjectileAI()
        {
            if (Projectile.timeLeft == 0)
            {
                Projectile.position = Projectile.Center;
                Projectile.Size = new Vector2(100, 100);
                Projectile.hide = true;
                Projectile.tileCollide = false;
                Projectile.Center = Projectile.position;
                FlareJump(Projectile.velocity);
            }
            SetRotation();
        }

        protected override bool ProjectileTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 0;
            return false;
        }

        protected override void ProjectileHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (TF2.IsPlayerOnFire(target))
                miniCrit = true;
        }

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (TF2.IsNPCOnFire(target))
                miniCrit = true;
        }

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info) => Projectile.timeLeft = 0;

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.timeLeft = 0;

        protected override void ProjectileDestroy(int timeLeft) => Explode(Projectile, new SoundStyle("TF2/Content/Sounds/SFX/Weapons/detonator_explode"));

        public void FlareJump(Vector2 velocity)
        {
            if (FindOwner(Projectile, 50f))
            {
                velocity *= 5f;
                velocity.X = Utils.Clamp(velocity.X, -25f, 25f);
                velocity.Y = Utils.Clamp(velocity.Y, -25f, 25f);
                Player.velocity -= velocity;
                if (Player.immuneNoBlink) return;
                int selfDamage = TF2.GetHealth(Player, 33.5);
                Player.Hurt(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[2].Format(Player.name)), selfDamage, 0, cooldownCounter: 5);
            }
        }

        public static void Explode(Projectile projectile, SoundStyle sound)
        {
            SoundEngine.PlaySound(sound, projectile.position);
            for (int i = 0; i < 50; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                dust.velocity *= 1.4f;
            }
            for (int i = 0; i < 80; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.RedTorch, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
                dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.IchorTorch, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
            }
        }

    }
}