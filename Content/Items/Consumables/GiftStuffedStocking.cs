using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace TF2.Content.Items.Consumables
{
    public class GiftStuffedStocking : TF2Item
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 50;
            Item.consumable = true;
            Item.knockBack = 0f;
            Item.rare = ModContent.RarityType<UniqueRarity>();
            noThe = true;
            qualityHashSet.Add(Unique);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            for (int i = 0; i < 3; i++)
            {
                Item item = new Item();
                item.SetDefaults(MannCoSupplyCrate.GetPossibleDrop);
                (item.ModItem as TF2Item).availability = Uncrate;
                player.QuickSpawnItem(player.GetSource_GiftOrReward(), item);
            }
            TF2.AddMoney(player, 5f);
        }
    }
}