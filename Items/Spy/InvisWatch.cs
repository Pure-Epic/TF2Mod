using TF2.Items;
using Terraria;
using Terraria.ModLoader;

namespace TF2.Items.Spy
{
	public class InvisWatch : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Set key to cloak\n"
							 + "Cloaking gives temporary invincibility\n"
							 + "Hitting reduces cloak duration");

		}

		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			TFClass p = player.GetModPlayer<TFClass>();
			p.invisWatchEquipped = true;
		}
	}
}