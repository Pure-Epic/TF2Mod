using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Heavy
{
    public class BrassBeast : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Primary, Unique, Craft);
            SetWeaponSize(50, 30);
            SetWeaponOffset(-8.5f, 12.5f);
            SetGunUseStyle(minigun: true);
            SetWeaponDamage(damage: 10.8, projectile: ModContent.ProjectileType<Bullet>(), shootAngle: 10f);
            SetWeaponAttackSpeed(0.105);
            SetWeaponAttackIntervals(altClick: true, noAmmo: true);
            SetMinigun(spinTime: 1.31, speed: 18.8);
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponActiveBonus(Player player)
        {
            if (player.itemAnimation == 0)
                SetPlayerSpeed(player, 40);
            else
                MinigunUpdate(player, speedPercentage);
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            if (player.controlUseItem && spinTimer >= spinUpTime)
                player.GetModPlayer<MinigunDamageResistance>().minigunDamageResistance = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ReclaimedMetal>()
                .AddIngredient<Natascha>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}