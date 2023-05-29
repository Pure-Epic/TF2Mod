using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Items.Consumables
{
    // This is the Item used to summon a boss, in this case the vanilla Plantera boss.
    public class PlanteraItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plantera's Bulb");
            Tooltip.SetDefault("Now portable!");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; // This helps sort inventory know that this is a boss summoning Item.

            Item.DefaultToPlaceableTile(TileID.PlanteraBulb);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = Item.buyPrice(gold: 35);
            Item.rare = ItemRarityID.Blue;
            Item.createTile = TileID.PlanteraBulb;
        }
    }
}