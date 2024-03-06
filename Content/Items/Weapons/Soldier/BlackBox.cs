using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Soldier;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class BlackBox : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Primary, Unique, Craft);
            SetWeaponSize(50, 27);
            SetWeaponOffset(-20f);
            SetGunUseStyle(focus: true, rocketLauncher: true);
            SetWeaponDamage(damage: 90, projectile: ModContent.ProjectileType<BlackBoxRocket>(), projectileSpeed: 25f, knockback: 5f);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/rocket_blackbox_shoot");
            SetWeaponAttackIntervals(maxAmmo: 3, maxReserve: 20, reloadTime: 0.8, initialReloadTime: 0.92, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/rocket_reload");
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<TF2Player>().noRandomHealthKits = true;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DirectHit>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}