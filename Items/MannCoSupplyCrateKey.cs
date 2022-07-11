using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Items;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Creative;

namespace TF2.Items
{
    public class MannCoSupplyCrateKey : ModItem
    {
        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Mann Co. Supply Crate Key");
			Tooltip.SetDefault("I wasted you!");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

		}

		public override void SetDefaults()
		{
			Item.width = 25;
			Item.height = 16;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(platinum: 1);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.Lerp(lightColor, Color.White, 0.4f);
		}
	}
}
