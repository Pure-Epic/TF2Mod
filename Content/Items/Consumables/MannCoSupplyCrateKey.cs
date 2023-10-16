using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Items.Consumables
{
    public class MannCoSupplyCrateKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mann Co. Supply Crate Key");
            Tooltip.SetDefault("Used to open locked supply crates.");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 25;
            Item.height = 16;
            Item.rare = ModContent.RarityType<UniqueRarity>();
            Item.value = Item.buyPrice(platinum: 1);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Materials.RefinedMetal>(50)
                .Register();
        }
    }
}
