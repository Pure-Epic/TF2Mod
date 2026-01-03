using Terraria.ModLoader;
using TF2.Content.Projectiles.Sniper;

namespace TF2.Content.Items.Weapons.Sniper
{
    public class Huntsman : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Sniper, Primary, Unique, Unlock);
            SetWeaponSize(11, 50);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 50, projectile: ModContent.ProjectileType<Arrow>(), projectileSpeed: 12.5f, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(1.94);
            SetWeaponAttackIntervals(maxReserve: 12, noAmmo: true);
            SetBow();
        }
    }
}