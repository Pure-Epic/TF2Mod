using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Spy
{
    public class InvisWatch : TF2Accessory
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Invis Watch");
            Tooltip.SetDefault("Set key to cloak\n"
                             + "Cloaking gives temporary invincibility\n"
                             + "Hitting reduces cloak duration");
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.invisWatchEquipped = true;
        }
    }
}