using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Heavy
{
    public class Tomislav : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Primary, Unique, Craft);
            SetWeaponSize(50, 32);
            SetWeaponOffset(-5f, 15f);
            SetGunUseStyle(focus: true, minigun: true);
            SetWeaponDamage(damage: 9, projectile: ModContent.ProjectileType<Bullet>(), shootAngle: 8f);
            SetWeaponAttackSpeed(0.12);
            SetWeaponAttackIntervals(maxAmmo: 200, altClick: true);
            SetMinigun(spinTime: 0.696, speed: 47, spinSound: "TF2/Content/Sounds/SFX/empty", spinUpSound: "TF2/Content/Sounds/SFX/Weapons/tomislav_wind_up", spinDownSound: "TF2/Content/Sounds/SFX/Weapons/tomislav_wind_down", attackSound: "TF2/Content/Sounds/SFX/Weapons/tomislav_shoot", emptySound: "TF2/Content/Sounds/SFX/empty");
            SetWeaponPrice(weapon: 1, reclaimed: 3);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<BrassBeast>().AddIngredient<ReclaimedMetal>(2).AddTile<CraftingAnvil>().Register();
    }
}