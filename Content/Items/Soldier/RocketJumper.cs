using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Soldier;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Soldier
{
    public class RocketJumper : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Primary, Unique, Craft);
            SetWeaponSize(50, 24);
            SetWeaponOffset(-25f, -5f);
            SetGunUseStyle();
            SetWeaponDamage(projectile: ModContent.ProjectileType<RocketJumperRocket>(), projectileSpeed: 25f);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/rocket_jumper_shoot");
            SetWeaponAttackIntervals(maxAmmo: 4, reloadTime: 0.8, initialReloadTime: 0.92, reloadSoundPath: "TF2/Content/Sounds/SFX/rocket_reload");
            SetWeaponPrice(weapon: 6, refined: 3, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override bool WeaponCanConsumeAmmo(Player player) => Main.rand.NextFloat() >= 0.5f;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Gunboats>(3)
                .AddIngredient<RefinedMetal>(3)
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}