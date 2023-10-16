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
using TF2.Content.Projectiles.Spy;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Spy
{
    public class YourEternalReward : TF2WeaponMelee
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Your Eternal Reward");
            Tooltip.SetDefault("Spy's Crafted Melee");

            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 48;
            Item.useAnimation = 48;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/knife_swing");
            Item.noUseGraphic = true;
            Item.autoReuse = true;

            Item.damage = 40;
            Item.shoot = ModContent.ProjectileType<YourEternalRewardProjectile>();
            Item.shootSpeed = 2.1f;
            Item.GetGlobalItem<TF2ItemBase>().noRandomCrits = true;

            Item.value = Item.buyPrice(platinum: 1, gold: 6);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "Basic attacks fire a projectile")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "+33% cloak drain rate\n"
                + "Backstabs require (and consume) a full cloak meter")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
        }


        public override void HoldItem(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.yourEternalRewardEquipped = true;
            if (!p.invisWatchEquipped && !p.cloakandDaggerEquipped && !p.deadRingerEquipped)
                Item.GetGlobalItem<TF2ItemBase>().allowBackstab = false;
        }

        public override void UpdateInventory(Player player)
        {
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].type == Type && !inHotbar)
                    inHotbar = true;
            }
            if (!inHotbar && !ModContent.GetInstance<TF2ConfigClient>().InventoryStats) return;
            player.GetModPlayer<TF2Player>().yourEternalRewardEquipped = true;
        }

        public override bool CanUseItem(Player player) => player.altFunctionUse != 2 || Item.GetGlobalItem<TF2ItemBase>().allowBackstab;

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (player.altFunctionUse == 2)
            {
                damage = (int)(120 * p.classMultiplier);
                p.backStab = true;
                player.velocity = velocity * 12.5f;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                player.immuneTime += 24;

                if (p.invisWatchEquipped)
                    player.GetModPlayer<Buffs.CloakPlayer>().cloakMeter = 0;
                if (p.cloakandDaggerEquipped)
                    player.GetModPlayer<Buffs.CloakandDaggerPlayer>().cloakMeter = 0;
                if (p.deadRingerEquipped)
                    player.GetModPlayer<FeignDeathPlayer>().cloakMeter = 0;
                return false;
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity * 10f, ModContent.ProjectileType<YourEternalRewardBeam>(), damage, knockback, player.whoAmI);
                return true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CloakandDagger>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}