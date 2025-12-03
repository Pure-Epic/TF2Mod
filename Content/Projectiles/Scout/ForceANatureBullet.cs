using Microsoft.Xna.Framework;
using Terraria;

namespace TF2.Content.Projectiles.Scout
{
    public class ForceANatureBullet : Bullet
    {
        public override string Texture => "TF2/Content/Projectiles/Bullet";

        protected override bool ProjectileTileCollide(Vector2 oldVelocity)
        {
            if (FindOwner(Projectile, 50f))
            {
                oldVelocity.X = Utils.Clamp(oldVelocity.X, -1.5f, 1.5f);
                oldVelocity.Y = Utils.Clamp(oldVelocity.Y, -1.5f, 1.5f);
                Player.velocity -= oldVelocity;
            }
            return true;
        }

        protected override void ProjectileHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (target.hostile && target.whoAmI != Main.myPlayer)
                KnockbackPlayer(target, ref modifiers, 10f);
        }

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!target.friendly && target.boss)
                KnockbackNPC(target, ref modifiers, 10f);
        }
    }
}