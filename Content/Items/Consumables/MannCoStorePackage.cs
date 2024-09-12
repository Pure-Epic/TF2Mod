using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace TF2.Content.Items.Consumables
{
    public class MannCoStorePackage : TF2Item
    {
        public bool keyFound;

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.width = 35;
            Item.height = 35;
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
            Item item = new Item();
            item.SetDefaults(MannCoSupplyCrate.GetPossibleDrop);
            (item.ModItem as TF2Item).availability = Uncrate;
            player.QuickSpawnItem(player.GetSource_GiftOrReward(), item);
        }
    }
}