using Microsoft.Xna.Framework;
using Terraria;

namespace TF2.Content.Projectiles.Scout
{
    public class ForceANatureBullet : Bullet
    {
        public override string Texture => "TF2/Content/Projectiles/Bullet";

        protected override bool ProjectileTileCollide(Vector2 oldVelocity)
        {
            if (TF2.FindPlayer(Projectile, 50f))
            {
                oldVelocity.X = Utils.Clamp(oldVelocity.X, -2f, 2f);
                oldVelocity.Y = Utils.Clamp(oldVelocity.Y, -2f, 2f);
                Player.velocity -= oldVelocity;
            }
            return true;
        }

        protected override void ProjectileHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            crit = false;
            // Modified from Terraria's source code
            float knockbackPower = 12.5f;
            if (target.hostile && target.whoAmI != Main.myPlayer)
            {
                if (modifiers.HitDirection < 0 && target.velocity.X > 0f - knockbackPower)
                {
                    if (target.velocity.X > 0f)
                    {
                        target.velocity.X -= knockbackPower;
                    }
                    target.velocity.X -= knockbackPower;
                    if (target.velocity.X < 0f - knockbackPower)
                    {
                        target.velocity.X = 0f - knockbackPower;
                    }
                }
                else if (modifiers.HitDirection > 0 && target.velocity.X < knockbackPower)
                {
                    if (target.velocity.X < 0f)
                    {
                        target.velocity.X += knockbackPower;
                    }
                    target.velocity.X += knockbackPower;
                    if (target.velocity.X > knockbackPower)
                    {
                        target.velocity.X = knockbackPower;
                    }
                }
                knockbackPower *= -0.5f;
                if (target.velocity.Y > knockbackPower)
                {
                    target.velocity.Y += knockbackPower;
                    if (target.velocity.Y < knockbackPower)
                    {
                        target.velocity.Y = knockbackPower;
                    }
                }
            }
        }

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Modified from Terraria's source code
            float knockbackPower = 10f;
            if (!target.friendly && target.boss)
            {
                if (modifiers.HitDirection < 0 && target.velocity.X > 0f - knockbackPower)
                {
                    if (target.velocity.X > 0f)
                    {
                        target.velocity.X -= knockbackPower;
                    }
                    target.velocity.X -= knockbackPower;
                    if (target.velocity.X < 0f - knockbackPower)
                    {
                        target.velocity.X = 0f - knockbackPower;
                    }
                }
                else if (modifiers.HitDirection > 0 && target.velocity.X < knockbackPower)
                {
                    if (target.velocity.X < 0f)
                    {
                        target.velocity.X += knockbackPower;
                    }
                    target.velocity.X += knockbackPower;
                    if (target.velocity.X > knockbackPower)
                    {
                        target.velocity.X = knockbackPower;
                    }
                }
                knockbackPower = target.noGravity ? (knockbackPower * -0.5f) : (knockbackPower * -0.75f);
                if (target.velocity.Y > knockbackPower)
                {
                    target.velocity.Y += knockbackPower;
                    if (target.velocity.Y < knockbackPower)
                    {
                        target.velocity.Y = knockbackPower;
                    }
                }
            }
        }
    }
}