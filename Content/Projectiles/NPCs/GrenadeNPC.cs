using Microsoft.Xna.Framework;
using TF2.Content.Projectiles.Demoman;

namespace TF2.Content.Projectiles.NPCs
{
    public class GrenadeNPC : Grenade
    {
        public override string Texture => "TF2/Content/Projectiles/Demoman/Grenade";

        protected override void GrenadeJump(Vector2 velocity) => Projectile.timeLeft = 0;
    }
}