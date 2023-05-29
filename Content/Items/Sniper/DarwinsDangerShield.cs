using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Sniper
{
    public class DarwinsDangerShield : TF2AccessorySecondary
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Darwin's Danger Shield");
            Tooltip.SetDefault("Sniper's Crafted Secondary");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.accessory = true;

            Item.value = Item.buyPrice(platinum: 1, gold: 6);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) // needs System.Linq
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "Immune to basic debuffs")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.sniperShield = true;
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                if (Main.debuff[i] && !BuffID.Sets.NurseCannotRemoveDebuff[i] && !Buffs.TF2BuffBase.cooldownBuff[i])
                    player.buffImmune[i] = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Razorback>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}