using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Demoman;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Sniper
{
    public class Bushwacka : TF2WeaponMelee
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Sniper's Crafted Melee");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 48;
            Item.useAnimation = 48;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/melee_swing");
            Item.autoReuse = true;

            Item.damage = 65;
            Item.GetGlobalItem<TF2ItemBase>().noRandomCrits = true;

            Item.value = Item.buyPrice(platinum: 1, gold: 6);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Neutral Attributes",
                "When weapon is active:")
            {
                OverrideColor = new Color(255, 255, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Positive Attributes",
                "Crits whenever it would normally mini-crit")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line2);

            var line3 = new TooltipLine(Mod, "Negative Attributes",
                "No random critical hits\n"
                + "20% damage vulnerability on wearer")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line3);
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.ModItem is Bushwacka)
            {
                if (player.GetModPlayer<TF2Player>().miniCrit)
                    player.GetModPlayer<TF2Player>().crit = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Eyelander>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class BushwackaPlayer : ModPlayer
    {
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (Player.HeldItem.ModItem is Bushwacka)
                damage = (int)(damage * 1.2f);
            return true;
        }
    }
}