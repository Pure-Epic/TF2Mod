using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Pyro;

namespace TF2.Content.Items.Weapons.Pyro
{
    public class FlareGun : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Secondary, Unique, Unlock);
            SetWeaponSize(40, 24);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 30, projectile: ModContent.ProjectileType<Flare>(), projectileSpeed: 25f, distanceModifier: false);
            SetWeaponAttackSpeed(0.25, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/flaregun_shoot");
            SetWeaponAttackIntervals(maxAmmo: 16, noAmmo: true, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/flaregun_reload");
            SetFlareGun();
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNeutralAttribute(description);
        }
    }
}