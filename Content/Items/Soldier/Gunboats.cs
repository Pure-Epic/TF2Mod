using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Demoman;
using TF2.Content.Items.Sniper;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Soldier
{
    public class Gunboats : TF2AccessorySecondary
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Soldier's Crafted Secondary");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) // needs System.Linq
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "-60% damage vulnerability")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.accessory = true;

            Item.value = Item.buyPrice(platinum: 2);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.gunboats = true;
            player.GetModPlayer<TF2Player>().damageReduction += 0.6f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Razorback>()
                .AddIngredient<CharginTarge>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}