using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Demoman;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Demoman
{
    public class LochnLoad : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Primary, Unique, Craft);
            SetWeaponSize(50, 24);
            SetGunUseStyle(focus: true, grenadeLauncher: true);
            SetWeaponDamage(damage: 100, projectile: ModContent.ProjectileType<LochnLoadGrenade>(), projectileSpeed: 15.625f, knockback: 5f);
            SetWeaponAttackSpeed(0.6);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/loch_n_load_shoot");
            SetWeaponAttackIntervals(maxAmmo: 3, maxReserve: 16, reloadTime: 0.6, initialReloadTime: 1.24, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/grenade_launcher_reload");
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<ScottishResistance>().AddIngredient<ReclaimedMetal>().AddTile<CraftingAnvil>().Register();
    }
}