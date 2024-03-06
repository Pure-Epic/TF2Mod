using Terraria.ModLoader;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.Weapons.Sniper
{
    public class SniperRifle : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Sniper, Primary, Stock, Starter);
            SetWeaponSize(50, 21);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 50, projectile: ModContent.ProjectileType<Bullet>(), noRandomCriticalHits: true);
            SetWeaponAttackSpeed(1.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/sniper_shoot");
            SetWeaponAttackIntervals(maxAmmo: 25, noAmmo: true);
            SetSniperRifle();
        }
    }
}