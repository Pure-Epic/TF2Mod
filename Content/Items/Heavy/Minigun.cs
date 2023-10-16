using Terraria.ModLoader;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.Heavy
{
    public class Minigun : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Primary, Stock, Starter);
            SetWeaponSize(50, 33);
            SetWeaponOffset(-10f, 15f);
            SetGunUseStyle(minigun: true);
            SetWeaponDamage(damage: 9, projectile: ModContent.ProjectileType<Bullet>(), shootAngle: 10f);
            SetWeaponAttackSpeed(0.105);
            SetWeaponAttackIntervals(altClick: true, noAmmo: true);
            SetMinigun(spinTime: 0.87, speed: 47);
        }
    }
}