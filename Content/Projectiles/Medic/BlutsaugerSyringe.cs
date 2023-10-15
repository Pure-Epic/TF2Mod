using Terraria;
using Terraria.ID;

namespace TF2.Content.Projectiles.Medic
{
    public class BlutsaugerSyringe : Syringe
    {
        public override string Texture => "TF2/Content/Projectiles/Medic/Syringe";

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (!(player.statLife >= player.statLifeMax2) && target.type != NPCID.TargetDummy)
                player.Heal((int)(player.statLifeMax2 * 0.02f));
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Player player = Main.player[Projectile.owner];
            if (!info.PvP) return;
            if (!(player.statLife >= player.statLifeMax2))
                player.Heal((int)(player.statLifeMax2 * 0.02f));
        }
    }
}