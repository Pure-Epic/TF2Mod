using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Items.Consumables
{
    // This is the Item used to summon a boss, in this case the vanilla Plantera boss.
    public class PlanteraItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
            Item.DefaultToPlaceableTile(TileID.PlanteraBulb);
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useTime = Item.useAnimation = TF2.Time(0.125);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.autoReuse = true;
            Item.createTile = TileID.PlanteraBulb;

            Item.value = Item.buyPrice(gold: 35);
            Item.rare = ItemRarityID.Orange;
        }
    }
}