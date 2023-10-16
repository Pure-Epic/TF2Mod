using Terraria.ModLoader;
using TF2.Content.Projectiles.Medic;

namespace TF2.Content.Items.Medic
{
    public class SyringeGun : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Primary, Stock, Starter);
            SetWeaponSize(50, 26);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 10, projectile: ModContent.ProjectileType<Syringe>(), projectileSpeed: 25f);
            SetWeaponAttackSpeed(0.105);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/syringegun_shoot");
            SetWeaponAttackIntervals(maxAmmo: 40, reloadTime: 1.305, usesMagazine: true, reloadSoundPath: "TF2/Content/Sounds/SFX/syringegun_reload");
        }
    }
}