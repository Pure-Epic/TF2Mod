using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Spy
{
    public class Enforcer : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Spy, Secondary, Unique, Craft);
            SetWeaponSize(38, 28);
            SetGunUseStyle(focus: true, automatic: true);
            SetWeaponDamage(damage: 40, projectile: ModContent.ProjectileType<SuperBullet>(), noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.6);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/enforcer_shoot");
            SetWeaponAttackIntervals(maxAmmo: 6, maxReserve: 24, reloadTime: 1.133, usesMagazine: true, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/revolver_reload");
            SetWeaponPrice(weapon: 1, reclaimed: 2, scrap: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPostFireProjectile(Player player, int projectile)
        {
            if (player.GetModPlayer<TF2Player>().Cloaked)
            Main.projectile[projectile].damage = TF2.Round(Main.projectile[projectile].damage * 1.2f);
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<LEtranger>().AddIngredient<ReclaimedMetal>().AddIngredient<ScrapMetal>().AddTile<AustraliumAnvil>().Register();
    }
}