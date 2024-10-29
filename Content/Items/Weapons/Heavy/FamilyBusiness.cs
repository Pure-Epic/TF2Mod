using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.Engineer;
using TF2.Content.Items.Weapons.Pyro;
using TF2.Content.Projectiles;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Heavy
{
    public class FamilyBusiness : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Secondary, Unique, Craft);
            SetWeaponSize(50, 10);
            SetGunUseStyle();
            SetWeaponDamage(damage: 5.1, projectile: ModContent.ProjectileType<Bullet>(), projectileCount: 10, shootAngle: 12f);
            SetWeaponAttackSpeed(0.531);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/family_business_shoot");
            SetWeaponAttackIntervals(maxAmmo: 8, maxReserve: 32, reloadTime: 0.5, initialReloadTime: 1, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/shotgun_reload");
            SetWeaponPrice(weapon: 2, reclaimed: 1, scrap: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<FrontierJustice>().AddIngredient<Homewrecker>().AddIngredient<ReclaimedMetal>().AddTile<AustraliumAnvil>().Register();
    }
}