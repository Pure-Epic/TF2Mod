using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Spy;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Spy
{
    public class BigEarner : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Spy, Melee, Unique, Craft);
            SetLungeUseStyle(knife: true);
            SetWeaponDamage(damage: 40, projectile: ModContent.ProjectileType<BigEarnerProjectile>(), projectileSpeed: 2f, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/knife_swing");
            SetWeaponAttackIntervals(altClick: true);
            SetWeaponPrice(weapon: 2, reclaimed: 3, scrap: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player) => TF2Player.SetPlayerHealth(player, -25);

        public override void AddRecipes() => CreateRecipe().AddIngredient<ConniversKunai>().AddIngredient<LEtranger>().AddIngredient<ReclaimedMetal>().AddTile<CraftingAnvil>().Register();
    }
}