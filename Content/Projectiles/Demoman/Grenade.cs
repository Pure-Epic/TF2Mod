using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace TF2.Content.Projectiles.Demoman
{
    public class Grenade : TF2Projectile
    {
        public bool StartTimer
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value ? 1f : 0f;
        }

        public int FuseTimer
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        public readonly int maxPower = 10;
        public readonly int fuseTime = TF2.Time(2.3);

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(24, 14);
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override bool ProjectilePreAI()
        {
            if (Timer >= maxPower)
            {
                Projectile.velocity.Y += 0.2f;
                Timer = maxPower;
            }
            if (!projectileInitialized)
            {
                velocity = Projectile.velocity;
                projectileInitialized = true;
                Projectile.netUpdate = true;
            }
            return true;
        }

        protected override void ProjectileAI()
        {
            if (StartTimer)
                FuseTimer++;
            if (FuseTimer == fuseTime)
                DetonateProjectile();
            if (ProjectileDetonation)
            {
                Projectile.position = Projectile.Center;
                Projectile.Size = new Vector2(150, 150);
                Projectile.hide = true;
                Projectile.tileCollide = false;
                Projectile.Center = Projectile.position;
                GrenadeJump(velocity);
            }
            SetRotation();
        }

        protected override bool ProjectileTileCollide(Vector2 oldVelocity)
        {
            StartTimer = true;
            if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f)
                Projectile.velocity.X = oldVelocity.X * -0.5f;
            if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
                Projectile.velocity.Y = oldVelocity.Y * -0.5f;
            return false;
        }

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info) => DetonateProjectile();

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => DetonateProjectile();

        protected override void ProjectileDestroy(int timeLeft) => TF2.Explode(Projectile, new SoundStyle("TF2/Content/Sounds/SFX/explode"));

        protected virtual void GrenadeJump(Vector2 velocity)
        {
            if (FindOwner(Projectile, 150f))
            {
                velocity.X = Utils.Clamp(velocity.X, -15f, 15f);
                velocity.Y = Math.Abs(Utils.Clamp(velocity.Y, -15f, 15f));
                Player.velocity -= velocity;
                QuickFixMirror();
                if (Player.immuneNoBlink) return;
                int selfDamage = TF2.GetHealth(Player, 55.5);
                Player.Hurt(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[2].ToNetworkText(Player.name)), selfDamage, 0, cooldownCounter: 5);
            }
        }
    }
}