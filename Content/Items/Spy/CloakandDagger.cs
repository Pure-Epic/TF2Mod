using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Spy
{
    public class CloakandDagger : TF2Accessory
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cloak and Dagger");
            Tooltip.SetDefault("Set key to cloak\n"
                             + "Cloak Type: Motion Sensitive\n"
                             + "Alt-Fire: Turn invisible.\n"
                             + "Cloak drain rate based on movement speed");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.accessory = true;

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "+100% cloak regen rate")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "No cloak meter from ammo boxes when invisible\n"
                + "-35% cloak meter from ammo boxes\n"
                + "Cloak breaks upon hitting an enemy when invisible")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.cloakandDaggerEquipped = true;
        }
    }
}