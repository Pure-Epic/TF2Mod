using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.Heavy;
using TF2.Content.Projectiles.Medic;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Medic
{
    public class Overdose : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Primary, Unique, Craft);
            SetWeaponSize(50, 33);
            SetGunUseStyle(focus: true, syringeGun: true);
            SetWeaponDamage(damage: 9, projectile: ModContent.ProjectileType<Syringe>(), projectileSpeed: 25f);
            SetWeaponAttackSpeed(0.105);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/overdose_shoot");
            SetWeaponAttackIntervals(maxAmmo: 40, maxReserve: 150, reloadTime: 1.305, usesMagazine: true, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/syringegun_reload");
            SetWeaponPrice(weapon: 2, reclaimed: 1, scrap: 4);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override void WeaponActiveBonus(Player player)
        {
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item.ModItem is TF2Weapon weapon && weapon.equipped && weapon.GetWeaponMechanic("Medi Gun"))
                {
                    float percentage = weapon.uberCharge / weapon.uberChargeCapacity * 100;
                    TF2Player.SetPlayerSpeed(player, 100 + 20 * TF2.RoundByMultiple(percentage, 10) / 100);
                    break;
                }
            }
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<CrusadersCrossbow>().AddIngredient<GlovesOfRunningUrgently>().AddIngredient<ReclaimedMetal>().AddTile<CraftingAnvil>().Register();
    }
}