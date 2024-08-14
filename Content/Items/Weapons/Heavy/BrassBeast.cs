using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Heavy
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
            SetWeaponAttackIntervals(maxAmmo: 200, altClick: true);
            SetMinigun(spinTime: 1.31, speed: 28.2, spinSound: "TF2/Content/Sounds/SFX/Weapons/brass_beast_spin", spinUpSound: "TF2/Content/Sounds/SFX/Weapons/brass_beast_wind_up", spinDownSound: "TF2/Content/Sounds/SFX/Weapons/brass_beast_wind_down", attackSound: "TF2/Content/Sounds/SFX/Weapons/brass_beast_shoot", emptySound: "TF2/Content/Sounds/SFX/Weapons/brass_beast_empty");
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
                TF2Player.SetPlayerSpeed(player, 40);
            else
                MinigunUpdate(player, speedPercentage);
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            if (player.controlUseItem && spinTimer >= spinUpTime)
                player.GetModPlayer<MinigunDamageResistance>().minigunDamageResistance = true;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<Natascha>().AddIngredient<ReclaimedMetal>().AddTile<CraftingAnvil>().Register();
    }
}