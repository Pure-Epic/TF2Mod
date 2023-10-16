﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Items.Currencies
{
    public class Australium : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Australium");
            Tooltip.SetDefault("Use it at Mann Co. Store");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

        }

        public override void SetDefaults()
        {
            Item.width = 25;
            Item.height = 18;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.buyPrice(platinum: 100);
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);
    }
}
