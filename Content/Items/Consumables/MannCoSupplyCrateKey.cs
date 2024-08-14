using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;

namespace TF2.Content.Items.Consumables
{
    public class MannCoSupplyCrateKey : TF2Item
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 10;

        public override void SetDefaults()
        {
            Item.width = 25;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<UniqueRarity>();
            Item.value = Item.buyPrice(platinum: 1);
            qualityHashSet.Add(Unique);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void AddRecipes() => CreateRecipe()
                .AddIngredient<RefinedMetal>(50)
                .Register();
    }
}