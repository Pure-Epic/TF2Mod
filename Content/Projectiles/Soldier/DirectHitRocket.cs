using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;

namespace TF2.Content.Projectiles.Soldier
{
    public class DirectHitRocket : Rocket
    {
        public override string Texture => "TF2/Content/Projectiles/Soldier/Rocket";

        public override void RocketJump(Vector2 velocity)
        {
            if (TF2.FindPlayer(Projectile, 50f))
            {
                velocity *= 5f;
                velocity.X = Utils.Clamp(velocity.X, -25f, 25f);
                velocity.Y = Utils.Clamp(velocity.Y, -25f, 25f);
                Player player = Main.player[Projectile.owner];
                player.velocity -= velocity;
                if (player.immuneNoBlink) return;
                int selfDamage = Convert.ToInt32(Math.Floor(player.statLifeMax2 * 0.1875f));
                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " blew themself to smithereens."), selfDamage, 0);
            }
        }
    }
}