using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Spy;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Spy
{
    public class YourEternalReward : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Spy, Melee, Unique, Craft);
            SetLungeUseStyle(knife: true);
            SetWeaponDamage(damage: 40, projectile: ModContent.ProjectileType<YourEternalRewardProjectile>(), projectileSpeed: 2f, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/knife_swing");
            SetWeaponAttackIntervals(altClick: true);
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override bool WeaponCanAltClick(Player player) => player.GetModPlayer<CloakPlayer>().fullCloak || player.GetModPlayer<CloakAndDaggerPlayer>().fullCloak || player.GetModPlayer<FeignDeathPlayer>().fullCloak;

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<YourEternalRewardPlayer>().yourEternalRewardEquipped = true;

        protected override void WeaponAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            int newDamage = !player.controlUseTile ? damage : (int)Math.Round(damage * p.classMultiplier * 3);
            Projectile.NewProjectile(source, position, velocity, type, newDamage, knockback, player.whoAmI);
            if (player.altFunctionUse == 2)
            {
                p.backStab = true;
                player.velocity = velocity * 12.5f;
                player.immuneTime += 24;
                if (player.GetModPlayer<CloakPlayer>().invisWatchEquipped)
                    player.GetModPlayer<CloakPlayer>().cloakMeter = 0;
                if (player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped)
                    player.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter = 0;
                if (player.GetModPlayer<FeignDeathPlayer>().deadRingerEquipped)
                    player.GetModPlayer<FeignDeathPlayer>().cloakMeter = 0;
            }
            else
                Projectile.NewProjectile(source, position, velocity * 10f, ModContent.ProjectileType<YourEternalRewardBeam>(), damage, knockback, player.whoAmI);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CloakAndDagger>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class YourEternalRewardPlayer : ModPlayer
    {
        public bool yourEternalRewardEquipped;

        public override void ResetEffects() => yourEternalRewardEquipped = false;
    }
}