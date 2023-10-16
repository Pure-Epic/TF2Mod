using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items;
using TF2.Gensokyo.Content.Projectiles.Engineer;

namespace TF2.Gensokyo.Content.Items.Engineer
{
    [ExtendsFromMod("Gensokyo")]
    public class PhotonShotgun : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Engineer, Primary, Unique, Exclusive);
            SetWeaponSize(56, 19);
            SetWeaponOffset(-8.5f);
            SetGunUseStyle();
            SetWeaponDamage(damage: 6, projectile: ModContent.ProjectileType<PhotonShotgunBullet>(), projectileCount: 10, shootAngle: 12f, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.625);
            SetWeaponAttackSound(SoundID.Item74);
            SetWeaponAttackIntervals(maxAmmo: 8, reloadTime: 0.51, initialReloadTime: 0.87, reloadSoundPath: "TF2/Content/Sounds/SFX/shotgun_reload");
            SetWeaponPrice(weapon: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }
    }
}