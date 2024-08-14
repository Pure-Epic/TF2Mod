using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TF2.Content.Projectiles.Demoman
{
    public class LochnLoadGrenade : Grenade
    {
        public override string Texture => "TF2/Content/Projectiles/Demoman/Grenade";

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
            if (FuseTimer == fuseTime || Projectile.timeLeft == 0)
            {
                Projectile.position = Projectile.Center;
                Projectile.Size = new Vector2(113, 113);
                Projectile.hide = true;
                Projectile.tileCollide = false;
                Projectile.Center = Projectile.position;
                GrenadeJump(velocity);
            }
            SetRotation();
        }

        protected override bool ProjectileTileCollide(Vector2 oldVelocity)
        {
            Projectile.active = false;
            return false;
        }

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Temporary solution
            ModLoader.TryGetMod("Gensokyo", out Mod gensokyo);
            modifiers.SourceDamage *= ((gensokyo != null && target.ModNPC?.Mod == gensokyo && target.boss) || target.TypeName == "Byakuren Hijiri") ? 1.2f : 1f;
        }

        protected override void GrenadeJump(Vector2 velocity)
        {
            if (TF2.FindPlayer(Projectile, 50f))
            {
                velocity *= 10f;
                velocity.X = Utils.Clamp(velocity.X, -25f, 25f);
                Player.velocity -= velocity;
                if (Player.immuneNoBlink) return;
                int selfDamage = TF2.GetHealth(Player, 55.5);
                Player.Hurt(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[2].Format(Player.name)), selfDamage, 0, cooldownCounter: 5);
            }
            Projectile.timeLeft = 0;
        }
    }
}