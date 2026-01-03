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

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info) => DetonateProjectile();

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => DetonateProjectile();

        protected override void ProjectileDestroy(int timeLeft) => TF2.Explode(Projectile, new SoundStyle("TF2/Content/Sounds/SFX/explode"));

        protected virtual void RocketJump(Vector2 velocity)
        {
            if (FindOwner(Projectile, 100f))
            {
                velocity.X = Utils.Clamp(velocity.X, -15f, 15f);
                velocity.Y = Utils.Clamp(velocity.Y, -15f, 15f);
                Player.velocity -= velocity;
                QuickFixMirror();
                if (Player.immuneNoBlink) return;
                int selfDamage = TF2.GetHealth(Player, 36.5);
                Player.Hurt(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[2].ToNetworkText(Player.name)), selfDamage, 0, cooldownCounter: 5);
            }
        }
    }
}