using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace TF2.Content.Projectiles.Soldier
{
    public class LibertyLauncherRocket : Rocket
    {
        public override string Texture => "TF2/Content/Projectiles/Soldier/Rocket";

        protected override void RocketJump(Vector2 velocity)
        {
            if (FindOwner(Projectile, 100f))
            {
                velocity.X = Utils.Clamp(velocity.X, -15f, 15f);
                velocity.Y = Utils.Clamp(velocity.Y, -15f, 15f);
                Player.velocity -= velocity;
                QuickFixMirror();
                if (Player.immuneNoBlink) return;
                int selfDamage = TF2.GetHealth(Player, 27.5);
                Player.Hurt(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[2].ToNetworkText(Player.name)), selfDamage, 0, cooldownCounter: 5);
            }
        }
    }
}