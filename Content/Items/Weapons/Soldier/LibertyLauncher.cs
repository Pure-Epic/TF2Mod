using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Soldier;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class LibertyLauncher : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Primary, Unique, Craft);
            SetWeaponSize(50, 19);
            SetWeaponOffset(-19f);
            SetGunUseStyle(focus: true, rocketLauncher: true);
            SetWeaponDamage(damage: 68, projectile: ModContent.ProjectileType<LibertyLauncherRocket>(), projectileSpeed: 35f, knockback: 5f);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/rocket_liberty_launcher_shoot");
            SetWeaponAttackIntervals(maxAmmo: 5, maxReserve: 20, reloadTime: 0.8, initialReloadTime: 0.92, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/rocket_reload");
            SetWeaponPrice(weapon: 1, reclaimed: 3);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<TF2Player>().noRandomHealthKits = true;

        public override void AddRecipes() => CreateRecipe().AddIngredient<BlackBox>().AddIngredient<ReclaimedMetal>(2).AddTile<AustraliumAnvil>().Register();
    }
}