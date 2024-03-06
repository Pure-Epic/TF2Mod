using Terraria.ModLoader;
using TF2.Content.Projectiles.Medic;

namespace TF2.Content.Items.Weapons.Medic
{
    public class SyringeGun : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Primary, Stock, Starter);
            SetWeaponSize(50, 26);
            SetGunUseStyle(focus: true, syringeGun: true);
            SetWeaponDamage(damage: 10, projectile: ModContent.ProjectileType<Syringe>(), projectileSpeed: 25f);
            SetWeaponAttackSpeed(0.105);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/syringegun_shoot");
            SetWeaponAttackIntervals(maxAmmo: 40, maxReserve: 150, reloadTime: 1.305, usesMagazine: true, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/syringegun_reload");
        }
    }
}