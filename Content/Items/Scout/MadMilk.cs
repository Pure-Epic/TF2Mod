using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Sniper;
using TF2.Content.Projectiles.Scout;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Scout
{
    public class MadMilk : TF2WeaponNoAmmo
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mad Milk");
            Tooltip.SetDefault("Scout's Crafted Secondary");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 21;
            Item.height = 50;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/scout_taunts14");
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.damage = 1;
            Item.shoot = ModContent.ProjectileType<MadMilkProjectile>();
            Item.shootSpeed = 10f;

            Item.value = Item.buyPrice(platinum: 1, gold: 6);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) // needs System.Linq
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            tooltips.Remove(tt);
            TooltipLine tt2 = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt2);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "Extinguishing teammates reduces cooldown by -20%")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Neutral Attributes",
                "Players heal 60% of the damage done to an enemy covered with milk.\n"
                + "\"The non-milk substance is cursed flames mixed with Shrek's bathwater.\"")
            {
                OverrideColor = new Color(255, 255, 255)
            };
            tooltips.Add(line2);
        }

        public override bool CanUseItem(Player player) => !player.GetModPlayer<MadMilkPlayer>().madMilkCooldown;

        public override bool? UseItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<MadMilkCooldown>(), 1200);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Jarate>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}