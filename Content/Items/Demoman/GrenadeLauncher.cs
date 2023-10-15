using Terraria.ModLoader;
using TF2.Content.Projectiles.Demoman;

namespace TF2.Content.Items.Demoman
{
    public class GrenadeLauncher : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Primary, Stock, Starter);
            SetWeaponSize(50, 28);
            SetGunUseStyle(focus: true, grenadeLauncher: true);
            SetWeaponDamage(damage: 100, projectile: ModContent.ProjectileType<Grenade>(), projectileSpeed: 12.5f, knockback: 5f);
            SetWeaponAttackSpeed(0.6);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/grenade_launcher_shoot");
            SetWeaponAttackIntervals(maxAmmo: 4, reloadTime: 0.6, initialReloadTime: 1.24, reloadSoundPath: "TF2/Content/Sounds/SFX/grenade_launcher_reload");
        }
    }
}