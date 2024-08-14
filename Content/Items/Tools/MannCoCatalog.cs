using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.UI.MannCoStore;

namespace TF2.Content.Items.Tools
{
    public class MannCoCatalog : TF2Item
    {
		public override void SetDefaults()
		{
			Item.width = 56;
			Item.height = 30;
			Item.useTime = Item.useAnimation = TF2.Time(1);
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.UseSound = SoundID.MenuOpen;
			Item.rare = ModContent.RarityType<NormalRarity>();
			qualityHashSet.Add(Stock);
			availability = Starter;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

		public override bool? UseItem(Player player)
        {
			TF2.MannCoStore.SetState(new MannCoStoreUI());
			return true;
        }
    }
}
