using Terraria;
using TF2.Common;

namespace TF2.Content.Items.Accessories
{
    public class TournamentStandard : RankToken
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tournament Standard");
            Tooltip.SetDefault("100% health\n"
                             + "100% weapon damage\n"
                             + "100% npc efficiency");
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.classMultiplier = 1f;
        }
    }
}
