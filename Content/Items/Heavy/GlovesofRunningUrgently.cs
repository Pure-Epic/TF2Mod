using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Heavy;
using TF2.Common;

namespace TF2.Content.Items.Heavy
{
    public class GlovesofRunningUrgently : TF2WeaponMelee
    {
        public bool holdingItemForFirstTime;
        public int thisItem;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gloves of Running Urgently");
            Tooltip.SetDefault("Heavy's Crafted Melee");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SafeSetDefaults()
        {
            holsterSpeed = 45;

            Item.damage = 65;
            Item.useStyle = ItemUseStyleID.Rapier; // Makes the player do the proper arm motion
            Item.useAnimation = 48;
            Item.useTime = 48 + holsterSpeed;
            Item.width = 50;
            Item.height = 50;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/melee_swing");
            Item.autoReuse = true;
            Item.noUseGraphic = true; // The sword is actually a "projectile", so the item should not be visible when used
            Item.noMelee = true; // The projectile will do the damage and not the item

            Item.shoot = ModContent.ProjectileType<GlovesofRunningUrgentlyProjectile>(); // The projectile is what makes a shortsword work
            Item.shootSpeed = 2.1f; // This value bleeds into the behavior of the projectile as velocity, keep that in mind when tweaking values

            Item.value = Item.buyPrice(platinum: 1, gold: 4);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "+30% faster move speed on wearer")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "This weapon holsters 50% slower\n"
                + "Maximum health is drained while item is active")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
        }

        public override void HoldItem(Player player)
        {
            thisItem = player.selectedItem;
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].type == ModContent.ItemType<GlovesofRunningUrgently>() && !inHotbar)
                    inHotbar = true;
            }
            if (holdingItemForFirstTime)
            {
                if (inHotbar)
                    player.itemTime = holsterSpeed;
                holdingItemForFirstTime = false;
            }
            if (player.controlUseItem)
            {
                if (player.itemTime <= holsterSpeed)
                    player.itemTime = 0;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.ModItem is not GlovesofRunningUrgently)
            {
                if (!player.ItemTimeIsZero && !holdingItemForFirstTime)
                    player.selectedItem = thisItem;
                else if (player.ItemTimeIsZero && !player.cursorItemIconEnabled)
                {
                    holdingItemForFirstTime = true;
                    inHotbar = false;
                }
            }
            else
            {
                player.moveSpeed += 0.33f;
                player.GetModPlayer<TF2Player>().speedMultiplier += 0.33f;
            }
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<KillingGlovesofBoxing>()
                .AddIngredient<Materials.ScrapMetal>(2)
                .AddTile<Tiles.Crafting.CraftingAnvil>()
                .Register();
        }
    }

    public class GlovesofRunningUrgentlyPlayer : ModPlayer
    {
        public override void UpdateBadLifeRegen()
        {
            if (Player.HeldItem.ModItem is GlovesofRunningUrgently && Player.statLife > Player.statLifeMax2 / 3)
            {
                // These lines zero out any positive lifeRegen. This is expected for all bad life regeneration effects
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                // Player.lifeRegenTime uses to increase the speed at which the player reaches its maximum natural life regeneration
                // So we set it to 0, and while this debuff is active, it never reaches it
                Player.lifeRegenTime = 0;
                // lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 8 life lost per second
                Player.lifeRegen -= (int)MathF.Ceiling(Player.statLifeMax2 / 100f);
            }
        }
    }
}