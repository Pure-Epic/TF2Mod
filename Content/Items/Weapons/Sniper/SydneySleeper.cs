using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Sniper;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Sniper
{
    public class SydneySleeper : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Sniper, Primary, Unique, Craft);
            SetWeaponSize(50, 13);
            SetWeaponOffset(-5f);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 50, projectile: ModContent.ProjectileType<SydneySleeperDart>(), noRandomCriticalHits: true);
            SetWeaponAttackSpeed(1.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/sniper_shoot");
            SetWeaponAttackIntervals(maxAmmo: 25, noAmmo: true);
            SetSniperRifle(maxChargeDuration: 1.4);
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPostFireProjectile(Player player, int projectile)
        {
            if (chargeTime > 0)
                (Main.projectile[projectile].ModProjectile as SydneySleeperDart).jarateDuration = TF2.Time(2) + (int)(TF2.Time(3) * (chargeTime / maxChargeTime));
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<Huntsman>().AddIngredient<ReclaimedMetal>().AddTile<CraftingAnvil>().Register();
    }
}