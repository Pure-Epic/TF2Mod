using Terraria;
using Terraria.ID;
using TF2.Common;

namespace TF2.Content.Projectiles.Medic
{
    public class BlutsaugerSyringe : Syringe
    {
        public override string Texture => "TF2/Content/Projectiles/Medic/Syringe";

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!info.PvP) return;
            if (!TF2Player.IsAtFullHealth(Player))
                Player.Heal(TF2.GetHealth(Player, 3));
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!TF2Player.IsAtFullHealth(Player) && target.type != NPCID.TargetDummy)
                Player.Heal(TF2.GetHealth(Player, 3));
        }
    }
}