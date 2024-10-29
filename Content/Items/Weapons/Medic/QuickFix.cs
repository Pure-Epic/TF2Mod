using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.Scout;
using TF2.Content.Projectiles.Medic;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Medic
{
    public class QuickFix : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Secondary, Unique, Craft);
            SetWeaponSize(50, 50);
            SetWeaponOffset(-10f);
            SetGunUseStyle(mediGun: true);
            SetWeaponDamage(projectile: ModContent.ProjectileType<HealingBeamQuickFix>());
            SetWeaponAttackSpeed(0.01666, hide: true);
            SetWeaponAttackIntervals(altClick: true, noAmmo: true);
            SetMediGun(buff: ModContent.BuffType<QuickFixUberCharge>(), duration: 8, healSound: "TF2/Content/Sounds/SFX/Weapons/quick_fix_heal");
            SetWeaponPrice(weapon: 2, reclaimed: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddHeader(description);
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<Kritzkrieg>().AddIngredient<MadMilk>().AddIngredient<ReclaimedMetal>().AddTile<AustraliumAnvil>().Register();
    }
}