using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Items.Placeables.Crafting
{
    public class CraftingAnvilItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Crafting.CraftingAnvil>();

            Item.width = 50;
            Item.height = 50;

            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;

            Item.consumable = true;

            Item.rare = ItemRarityID.Expert;
            Item.value = Item.buyPrice(platinum: 1);
        }
    }
}