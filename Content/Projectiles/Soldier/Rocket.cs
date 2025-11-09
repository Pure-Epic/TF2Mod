using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace TF2.Content.Projectiles.Soldier
{
    public class Rocket : TF2Projectile
    {
        protected override void ProjectileStatistics()
        {
            SetProjectileSize(40, 12);
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
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
                RocketJump(Projectile.velocity);
            }
            SetRotation();
        }

        protected override bool ProjectileTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 0;
            return false;
        }

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info) => Projectile.timeLeft = 0;

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.timeLeft = 0;

        protected override void ProjectileDestroy(int timeLeft) => TF2.Explode(Projectile, new SoundStyle("TF2/Content/Sounds/SFX/explode"));

        public virtual void RocketJump(Vector2 velocity)
        {
            if (FindOwner(Projectile, 50f))
            {
                velocity *= 5f;
                velocity.X = Utils.Clamp(velocity.X, -25f, 25f);
                velocity.Y = Utils.Clamp(velocity.Y, -25f, 25f);
                Player.velocity -= velocity;
                if (Player.immuneNoBlink) return;
                int selfDamage = TF2.GetHealth(Player, 36.5);
                Player.Hurt(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[2].ToNetworkText(Player.name)), selfDamage, 0, cooldownCounter: 5);
            }
        }
    }
}