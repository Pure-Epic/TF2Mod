using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Soldier;

namespace TF2.Content.Items.Soldier
{
    public class DirectHit : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Primary, Unlock, Unique);
            SetWeaponSize(50, 16);
            SetWeaponOffset(-25f);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 112, projectile: ModContent.ProjectileType<DirectHitRocket>(), projectileSpeed: 45f, knockback: 5f);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/rocket_directhit_shoot");
            SetWeaponAttackIntervals(maxAmmo: 4, reloadTime: 0.8, initialReloadTime: 0.92, reloadSoundPath: "TF2/Content/Sounds/SFX/rocket_reload");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }
    }
}