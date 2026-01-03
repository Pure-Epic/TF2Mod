using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Spy;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Spy
{
    public class BigEarner : TF2Weapon
    {
        protected override int HealthBoost => -25;

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Spy, Melee, Unique, Craft);
            SetLungeUseStyle(knife: true);
            SetWeaponDamage(damage: 40, projectile: ModContent.ProjectileType<BigEarnerProjectile>(), projectileSpeed: 2f, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/knife_swing");
            SetKnife();
            SetWeaponPrice(weapon: 2, reclaimed: 3, scrap: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<ConniversKunai>().AddIngredient<LEtranger>().AddIngredient<ReclaimedMetal>().AddTile<AustraliumAnvil>().Register();
    }
}