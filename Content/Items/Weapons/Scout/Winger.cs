using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Scout
{
    public class Winger : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Secondary, Unique, Craft);
            SetWeaponSize(30, 26);
            SetGunUseStyle(focus: true, automatic: true);
            SetWeaponDamage(damage: 17, projectile: ModContent.ProjectileType<Bullet>());
            SetWeaponAttackSpeed(0.15);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/winger_shoot");
            SetWeaponAttackIntervals(maxAmmo: 5, maxReserve: 36, reloadTime: 1.005, usesMagazine: true, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/pistol_reload");
            SetWeaponPrice(weapon: 1, reclaimed: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponActiveBonus(Player player) => Player.jumpSpeed *= 1.25f;

        public override void AddRecipes() => CreateRecipe().AddIngredient<Shortstop>().AddIngredient<ReclaimedMetal>().AddTile<CraftingAnvil>().Register();
    }
}