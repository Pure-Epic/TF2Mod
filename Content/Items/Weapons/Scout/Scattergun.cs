using Terraria.ModLoader;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.Weapons.Scout
{
    public class Scattergun : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Primary, Stock, Starter);
            SetWeaponSize(50, 14);
            SetGunUseStyle();
            SetWeaponDamage(damage: 6, projectile: ModContent.ProjectileType<Bullet>(), projectileCount: 10, shootAngle: 12f);
            SetWeaponAttackSpeed(0.625);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/scatter_gun_shoot");
            SetWeaponAttackIntervals(maxAmmo: 6, maxReserve: 32, reloadTime: 0.5, initialReloadTime: 0.7, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/scatter_gun_reload");
        }
    }
}