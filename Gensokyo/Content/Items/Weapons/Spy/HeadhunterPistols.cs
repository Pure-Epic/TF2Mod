using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Items.Weapons;
using TF2.Content.Projectiles;

namespace TF2.Gensokyo.Content.Items.Weapons.Spy
{
    [ExtendsFromMod("Gensokyo")]
    public class HeadhunterPistols : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Spy, Secondary, Unique, Exclusive);
            SetWeaponSize(29, 19);
            SetWeaponOffset(2.5f);
            SetGunUseStyle(focus: true, automatic: true);
            SetWeaponDamage(damage: 45, projectile: ModContent.ProjectileType<Bullet>());
            SetWeaponAttackSpeed(0.3);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/pistol_shoot");
            SetWeaponAttackIntervals(maxAmmo: 20, maxReserve: 20, reloadTime: 2.26666, usesMagazine: true, reloadSoundPath: "TF2/Gensokyo/Content/Sounds/SFX/headhunterpistols_reload");
            SetWeaponPrice(weapon: 10);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }
    }
}