using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Items.Materials
{
    public class ScrapMetal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scrap Metal");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 38;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<UniqueRarity>();
            Item.value = Item.buyPrice(gold: 2);
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void AddRecipes()
        {
            CreateRecipe(3)
                .AddIngredient<ReclaimedMetal>()
                .Register();
        }
    }

    public class ReclaimedMetal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reclaimed Metal");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 38;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<UniqueRarity>();
            Item.value = Item.buyPrice(gold: 6);
        }

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

    public class RefinedMetal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Refined Metal");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 38;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<UniqueRarity>();
            Item.value = Item.buyPrice(gold: 18);
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ReclaimedMetal>(3)
                .Register();
            CreateRecipe(50)
                .AddIngredient<Consumables.MannCoSupplyCrateKey>()
                .Register();
        }
    }
}