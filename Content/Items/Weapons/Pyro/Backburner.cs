using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Pyro;

namespace TF2.Content.Items.Weapons.Pyro
{
    public class Backburner : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Primary, Unique, Unlock);
            SetWeaponSize(50, 16);
            SetGunUseStyle();
            SetWeaponDamage(damage: 18, projectile: ModContent.ProjectileType<Fire>(), noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.105, hide: true);
            SetWeaponAttackIntervals(maxAmmo: 200, altClick: true, noAmmo: true);
            SetFlamethrower(cost: 50, attackSound: "TF2/Content/Sounds/SFX/Weapons/backburner_loop");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
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

        protected override void WeaponPostFireProjectile(Player player, int projectile)
        {
            if (TF2Player.IsHealthFull(player) && player.altFunctionUse != 2)
                (Main.projectile[projectile].ModProjectile as TF2Projectile).crit = true;
        }
    }
}