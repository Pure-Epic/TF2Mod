using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;


namespace TF2.Content.Items.Placeables.Crafting
{
    public class CraftingAnvilItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crafting Anvil");
            Tooltip.SetDefault("For years you have been able\n"
                + "to create weapons with your bare hands,\n"
                + "using raw steel, in real life.\n"
                + "What if we were to tell you\n"
                + "there’s now a way to SIMULATE that in-game?\n"
                + "Because we just did!");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Crafting.CraftingAnvil>(); // This sets the id of the tile that this item should place when used.

            Item.width = 50; // The item texture's width
            Item.height = 50; // The item texture's height

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