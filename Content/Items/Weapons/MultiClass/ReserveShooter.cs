using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.Engineer;
using TF2.Content.Projectiles;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.MultiClass
{
    public class ReserveShooter : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(MultiClass, Secondary, Unique, Craft);
            SetWeaponClass([Soldier, Pyro]);
            SetWeaponSize(50, 15);
            SetGunUseStyle();
            SetWeaponDamage(damage: 6, projectile: ModContent.ProjectileType<Bullet>(), projectileCount: 10, shootAngle: 12f);
            SetWeaponAttackSpeed(0.625);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/reserve_shooter_shoot");
            SetWeaponAttackIntervals(deploy: 0.4, maxAmmo: 4, maxReserve: 32, reloadTime: 0.5, initialReloadTime: 1, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/shotgun_reload");
            SetWeaponPrice(weapon: 1, reclaimed: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPostFireProjectile(Player player, int projectile) => (Main.projectile[projectile].ModProjectile as TF2Projectile).reserveShooterProjectile = true;

        public override void AddRecipes() => CreateRecipe().AddIngredient<FrontierJustice>().AddIngredient<ReclaimedMetal>(2).AddTile<AustraliumAnvil>().Register();
    }
}