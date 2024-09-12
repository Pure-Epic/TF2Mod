using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TF2.Content.Projectiles.Soldier
{
    public class DirectHitRocket : Rocket
    {
        public override string Texture => "TF2/Content/Projectiles/Soldier/Rocket";

        protected override void ProjectileAI()
        {
            if (Projectile.timeLeft == 0)
            {
                Projectile.position = Projectile.Center;
                Projectile.Size = new Vector2(30, 30);
                Projectile.hide = true;
                Projectile.tileCollide = false;
                Projectile.Center = Projectile.position;
                RocketJump(Projectile.velocity);
            }
            SetRotation();
        }

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            ModLoader.TryGetMod("Gensokyo", out Mod gensokyo);
            if (target.ModNPC?.Mod == gensokyo && target.boss || target.TypeName == "Byakuren Hijiri")
                miniCrit = true;
        }

        public override void RocketJump(Vector2 velocity)
        {
            if (FindOwner(Projectile, 50f))
            {
                velocity *= 5f;
                velocity.X = Utils.Clamp(velocity.X, -25f, 25f);
                velocity.Y = Utils.Clamp(velocity.Y, -25f, 25f);
                Player.velocity -= velocity;
                if (Player.immuneNoBlink) return;
                int selfDamage = TF2.GetHealth(Player, 36.5);
                Player.Hurt(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[2].Format(Player.name)), selfDamage, 0, cooldownCounter: 5);
            }
        }
    }
}