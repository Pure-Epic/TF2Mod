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
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Pyro
{
    public class Powerjack : TF2WeaponMelee
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Pyro's Crafted Melee");

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
                "+15% faster move speed on wearer.\n"
                + "+14% of maximum health restored on critical hits")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line2);

            var line3 = new TooltipLine(Mod, "Negative Attributes",
                "20% damage vulnerability on wearer")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line3);
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.ModItem is Powerjack)
            {
                player.moveSpeed += 0.15f;
                player.GetModPlayer<TF2Player>().speedMultiplier += 0.15f;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (crit && target.type != NPCID.TargetDummy && player.statLife >= player.statLifeMax2)
            {
                int healingAmount = (int)(player.statLifeMax2 * 0.14285714285f);
                player.statLife += healingAmount;
                player.HealEffect(healingAmount, true);
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (crit && player.statLife >= player.statLifeMax2)
            {
                int healingAmount = (int)(player.statLifeMax2 * 0.14285714285f);
                player.statLife += healingAmount;
                player.HealEffect(healingAmount, true);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Axtinguisher>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class PowerjackPlayer : ModPlayer
    {
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (Player.HeldItem.ModItem is Powerjack)
                damage = (int)(damage * 1.2f);
            return true;
        }
    }
}