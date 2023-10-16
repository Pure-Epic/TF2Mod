using Terraria.ModLoader;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.Spy
{
    public class Revolver : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Spy, Secondary, Stock, Starter);
            SetWeaponSize(50, 29);
            SetGunUseStyle(focus: true, automatic: true);
            SetWeaponDamage(damage: 40, projectile: ModContent.ProjectileType<Bullet>());
            SetWeaponAttackSpeed(0.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/revolver_shoot");
            SetWeaponAttackIntervals(maxAmmo: 6, reloadTime: 1.133, usesMagazine: true, reloadSoundPath: "TF2/Content/Sounds/SFX/revolver_reload");
        }
    }
}