using Terraria;
using Terraria.ID;
using TF2.Common;

namespace TF2.Content.Projectiles.Soldier
{
    public class BlackBoxRocket : Rocket
    {
        public override string Texture => "TF2/Content/Projectiles/Soldier/Rocket";

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (player.statLife < player.statLifeMax2 && target.type != NPCID.TargetDummy)
            {
                int amount = (int)(0.22222f * damageDone / p.classMultiplier * player.statLifeMax2 / 200f);
                amount = Utils.Clamp(amount, 0, (int)(player.statLifeMax2 * 0.1f));
                player.Heal(amount);
            }
            prime = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Player player = Main.player[Projectile.owner];
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (player.statLife < player.statLifeMax2 && target.whoAmI != Projectile.owner)
            {
                int amount = (int)(0.22222f * info.Damage / p.classMultiplier * player.statLifeMax2 / 200f);
                amount = Utils.Clamp(amount, 0, (int)(player.statLifeMax2 * 0.1f));
                player.Heal(amount);
            }
            prime = true;
        }
    }
}