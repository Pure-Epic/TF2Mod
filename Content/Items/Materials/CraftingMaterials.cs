using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Consumables;

namespace TF2.Content.Items.Materials
{
    public class ScrapMetal : TF2Item
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 100;

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 38;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ModContent.RarityType<UniqueRarity>();
            noThe = true;
            qualityHashSet.Add(Unique);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void AddRecipes() => CreateRecipe(3)
                .AddIngredient<ReclaimedMetal>()
                .Register();
    }

    public class ReclaimedMetal : TF2Item
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 100;

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 38;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(gold: 6);
            Item.rare = ModContent.RarityType<UniqueRarity>();
            noThe = true;
            qualityHashSet.Add(Unique);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ScrapMetal>(3)
                .Register();
            CreateRecipe(3)
                .AddIngredient<RefinedMetal>()
                .Register();
        }
    }

    public class RefinedMetal : TF2Item
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 100;

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 38;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(gold: 18);
            Item.rare = ModContent.RarityType<UniqueRarity>();
            noThe = true;
            qualityHashSet.Add(Unique);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ReclaimedMetal>(3)
                .Register();
            CreateRecipe(50)
                .AddIngredient<MannCoSupplyCrateKey>()
                .Register();
        }
    }

    public class HauntedMetalScrap : TF2Item
    {
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 29;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(platinum: 10);
            Item.rare = ModContent.RarityType<UnusualRarity>();
            qualityHashSet.Add(Unusual);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);
    }
}