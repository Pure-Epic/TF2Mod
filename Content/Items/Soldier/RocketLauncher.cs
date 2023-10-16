using Terraria.ModLoader;
using TF2.Content.Projectiles.Soldier;

namespace TF2.Content.Items.Soldier
{
    public class RocketLauncher : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Primary, Stock, Starter);
            SetWeaponSize(50, 21);
            SetWeaponOffset(-25f);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 90, projectile: ModContent.ProjectileType<Rocket>(), projectileSpeed: 25f, knockback: 5f);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/rocket_shoot");
            SetWeaponAttackIntervals(maxAmmo: 4, reloadTime: 0.8, initialReloadTime: 0.92, reloadSoundPath: "TF2/Content/Sounds/SFX/rocket_reload");
        }
    }
}