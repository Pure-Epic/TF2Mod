using Terraria;
using Terraria.ID;
using TF2.Common;

namespace TF2.Content.Projectiles.Soldier
{
    public class BlackBoxRocket : Rocket
    {
        public override string Texture => "TF2/Content/Projectiles/Soldier/Rocket";

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (!TF2Player.IsHealthFull(Player) && target.whoAmI != Projectile.owner)
            {
                int amount = (int)(0.22222f * info.Damage / p.classMultiplier * TF2.GetHealth(Player, 1));
                amount = Utils.Clamp(amount, 0, TF2.GetHealth(Player, 20));
                Player.Heal(amount);
            }
            Projectile.timeLeft = 0;
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (!TF2Player.IsHealthFull(Player) && target.type != NPCID.TargetDummy)
            {
                int amount = (int)(0.22222f * damageDone / p.classMultiplier * TF2.GetHealth(Player, 1));
                amount = Utils.Clamp(amount, 0, TF2.GetHealth(Player, 20));
                Player.Heal(amount);
            }
            Projectile.timeLeft = 0;
        }
    }
}