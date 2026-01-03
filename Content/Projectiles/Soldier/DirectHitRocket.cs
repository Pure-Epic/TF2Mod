using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace TF2.Content.Projectiles.Soldier
{
    public class DirectHitRocket : Rocket
    {
        public override string Texture => "TF2/Content/Projectiles/Soldier/Rocket";

        protected override void ProjectileAI()
        {
            if (ProjectileDetonation)
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

        protected override void ProjectileHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (target.velocity.Y != 0)
                miniCrit = true;
        }

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.velocity.Y != 0 || target.boss)
                miniCrit = true;
        }

        protected override void RocketJump(Vector2 velocity)
        {
            if (FindOwner(Projectile, 30f))
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