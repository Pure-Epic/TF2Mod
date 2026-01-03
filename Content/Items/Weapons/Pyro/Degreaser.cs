using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Pyro;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Pyro
{
    public class Degreaser : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Primary, Unique, Craft);
            SetWeaponSize(50, 16);
            SetGunUseStyle();
            SetWeaponDamage(damage: 17, projectile: ModContent.ProjectileType<DegreaserFire>());
            SetWeaponAttackSpeed(0.105, hide: true);
            SetWeaponAttackIntervals(deploy: 0.2, holster: 0.35, maxAmmo: 200, altClick: true, noAmmo: true);
            SetFlamethrower(cost: 25, attackSound: "TF2/Content/Sounds/SFX/Weapons/degreaser_loop");
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player)
        {
            if (player.altFunctionUse == 2 && currentAmmoClip >= airblastCost && airblastTimer >= airblastCooldown)
                player.itemAnimation = player.itemAnimationMax = airblastCooldown;
            return ((player.altFunctionUse != 2 && currentAmmoClip > 0) || (player.altFunctionUse == 2 && currentAmmoClip >= airblastCost)) && airblastTimer >= airblastCooldown;
        }

        protected override void WeaponActiveUpdate(Player player)
        {
            airblastTimer++;
            if (airblastTimer >= airblastCooldown)
                airblastTimer = airblastCooldown;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<Backburner>().AddIngredient<ReclaimedMetal>().AddTile<AustraliumAnvil>().Register();
    }
}