using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Soldier;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class DirectHit : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Primary, Unique, Unlock);
            SetWeaponSize(50, 16);
            SetWeaponOffset(-25f);
            SetGunUseStyle(rocketLauncher: true);
            SetWeaponDamage(damage: 112, projectile: ModContent.ProjectileType<DirectHitRocket>(), projectileSpeed: 45f, knockback: 5f);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/rocket_directhit_shoot");
            SetWeaponAttackIntervals(maxAmmo: 4, maxReserve: 20, reloadTime: 0.8, initialReloadTime: 0.92, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/rocket_reload");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }
    }
}