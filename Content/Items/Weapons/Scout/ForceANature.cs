using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Scout;

namespace TF2.Content.Items.Weapons.Scout
{
    public class ForceANature : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Primary, Unique, Unlock);
            SetWeaponSize(50, 17);
            SetGunUseStyle();
            SetWeaponDamage(damage: 5.4, projectile: ModContent.ProjectileType<ForceANatureBullet>(), projectileCount: 12, shootAngle: 12f, knockback: 10f);
            SetWeaponAttackSpeed(0.3125);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/scatter_gun_shoot");
            SetWeaponAttackIntervals(maxAmmo: 2, maxReserve: 32, reloadTime: 1.4333, usesMagazine: true, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/scatter_gun_double_tube_reload");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }
    }
}