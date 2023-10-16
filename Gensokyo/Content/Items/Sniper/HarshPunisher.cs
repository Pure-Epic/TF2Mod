using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Items;
using TF2.Content.Projectiles;

namespace TF2.Gensokyo.Content.Items.Sniper
{
    [ExtendsFromMod("Gensokyo")]
    public class HarshPunisher : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Sniper, Primary, Unique, Exclusive);
            SetWeaponSize(59, 21);
            SetWeaponOffset(-5f);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 100, projectile: ModContent.ProjectileType<Bullet>(), noRandomCriticalHits: true);
            SetWeaponAttackSpeed(1);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/sniper_shoot");
            SetWeaponAttackIntervals(maxAmmo: 7, reloadTime: 2, reloadSoundPath: "TF2/Gensokyo/Content/Sounds/SFX/harshpunisher_reload", usesMagazine: true);
            SetSniperRifle(chargeDamage: 20, maxChargeTime: 1, zoomDelay: 0);
            SetWeaponPrice(weapon: 10);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }
    }
}