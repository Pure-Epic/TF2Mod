using Microsoft.Xna.Framework;
using Terraria;
using TF2.Content.Projectiles.Soldier;

namespace TF2.Content.Projectiles.NPCs
{
    public class SentryRocket : Rocket
    {
        public override string Texture => "TF2/Content/Projectiles/Soldier/Rocket";

        public override void RocketJump(Vector2 velocity) => Projectile.timeLeft = 0;
    }
}