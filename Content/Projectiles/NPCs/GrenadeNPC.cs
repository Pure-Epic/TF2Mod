using TF2.Content.Projectiles.Demoman;

namespace TF2.Content.Projectiles.NPCs
{
    public class GrenadeNPC : Grenade
    {
        public override string Texture => "TF2/Content/Projectiles/Demoman/Grenade";

        public override void OnKill(int timeLeft) => base.OnKill(timeLeft);
    }
}