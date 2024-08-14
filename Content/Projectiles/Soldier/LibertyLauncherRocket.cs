using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace TF2.Content.Projectiles.Soldier
{
    public class LibertyLauncherRocket : Rocket
    {
        public override string Texture => "TF2/Content/Projectiles/Soldier/Rocket";

        public override void RocketJump(Vector2 velocity)
        {
            if (TF2.FindPlayer(Projectile, 50f))
            {
                velocity *= 5f;
                velocity.X = Utils.Clamp(velocity.X, -25f, 25f);
                velocity.Y = Utils.Clamp(velocity.Y, -25f, 25f);
                Player.velocity -= velocity;
                if (Player.immuneNoBlink) return;
                int selfDamage = TF2.GetHealth(Player, 27.5);
                Player.Hurt(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[2].Format(Player.name)), selfDamage, 0, cooldownCounter: 5);
            }
        }
    }
}