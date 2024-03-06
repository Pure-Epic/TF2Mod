using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Demoman;

namespace TF2.Content.Items.Weapons.Demoman
{
    public class StickybombLauncher : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Secondary, Stock, Starter);
            SetWeaponSize(50, 28);
            SetGunUseStyle(stickybombLauncher: true);
            SetWeaponDamage(damage: 120, projectile: ModContent.ProjectileType<Stickybomb>(), knockback: 5f);
            SetWeaponAttackSpeed(0.6);
            SetWeaponAttackIntervals(maxAmmo: 8, maxReserve: 24, reloadTime: 0.67, initialReloadTime: 1.09, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/stickybomblauncher_reload");
            SetStickybombLauncher(capacity: 8, detonationTime: 0.7, maxCharge: 4);
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);
    }
}