using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Placeables.Crafting
{
    public class AustraliumAnvilItem : TF2Item
    {
        protected override string CustomCategory => Language.GetTextValue("Mods.TF2.UI.Items.AustraliumAnvil");

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 16;
            Item.useTime = TF2.Time(0.16667);
            Item.useAnimation = TF2.Time(0.25);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<AustraliumAnvil>();
            Item.rare = ModContent.RarityType<UniqueRarity>();
            Item.value = Item.buyPrice(platinum: 1);
            noThe = true;
            qualityHashSet.Add(Unique);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);
    }
}