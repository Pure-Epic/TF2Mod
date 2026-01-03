using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Pyro;

namespace TF2.Content.Items.Weapons.Pyro
{
    public class FlameThrower : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Primary, Stock, Starter);
            SetWeaponSize(50, 14);
            SetGunUseStyle();
            SetWeaponDamage(damage: 17, projectile: ModContent.ProjectileType<Fire>());
            SetWeaponAttackSpeed(0.105, hide: true);
            SetWeaponAttackIntervals(maxAmmo: 200, altClick: true, noAmmo: true);
            SetFlamethrower();
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNeutralAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player)
        {
            if (player.altFunctionUse == 2 && currentAmmoClip >= airblastCost && airblastTimer >= airblastCooldown)
                player.itemAnimation = player.itemAnimationMax = airblastCooldown;
            return ((player.altFunctionUse != 2 && currentAmmoClip > 0) || (player.altFunctionUse == 2 && currentAmmoClip >= airblastCost)) && airblastTimer >= airblastCooldown;
        }

        protected override void WeaponActiveUpdate(Player player)
        {
            airblastTimer++;
            if (airblastTimer >= airblastCooldown)
                airblastTimer = airblastCooldown;
        }
    }
}