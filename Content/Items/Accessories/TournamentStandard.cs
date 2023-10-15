using Terraria;
using TF2.Common;

namespace TF2.Content.Items.Accessories
{
    public class TournamentStandard : RankToken
    {
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.classMultiplier = 1f;
        }
    }
}