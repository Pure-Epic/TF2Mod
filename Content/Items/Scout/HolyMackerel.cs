using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Items.Scout
{
    public class HolyMackerel : TF2WeaponMelee
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Mackerel");
            Tooltip.SetDefault("Scout's Crafted Melee\n"
                + "Getting hit by a fish has got to be humiliating.");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/melee_swing");
            Item.autoReuse = true;

            Item.damage = 35;

            Item.value = Item.buyPrice(platinum: 1, gold: 6);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Sandman>()
                .AddIngredient<Materials.ReclaimedMetal>()
                .AddTile<Tiles.Crafting.CraftingAnvil>()
                .Register();
        }
    }
}