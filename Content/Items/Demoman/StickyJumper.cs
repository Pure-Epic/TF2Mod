using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Demoman;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Demoman
{
    public class StickyJumper : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Secondary, Unique, Craft);
            SetWeaponSize(50, 35);
            SetGunUseStyle(stickybombLauncher: true);
            SetWeaponDamage(projectile: ModContent.ProjectileType<StickyJumperStickybomb>(), knockback: 5f);
            SetWeaponAttackSpeed(0.6);
            SetWeaponAttackIntervals(maxAmmo: 8, reloadTime: 0.67, initialReloadTime: 1.09, reloadSoundPath: "TF2/Content/Sounds/SFX/stickybomblauncher_reload");
            SetStickybombLauncher(capacity: 2, detonationTime: 0.7, maxCharge: 4, attackSound: "TF2/Content/Sounds/SFX/sticky_jumper_shoot");
            SetWeaponPrice(weapon: 3, reclaimed: 1, scrap: 9);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<UllapoolCaber>(3)
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}