using Terraria.ModLoader;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.Sniper
{
    public class SMG : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Sniper, Secondary, Stock, Starter);
            SetWeaponSize(50, 31);
            SetGunUseStyle(focus: true, automatic: true);
            SetWeaponDamage(damage: 8, projectile: ModContent.ProjectileType<Bullet>());
            SetWeaponAttackSpeed(0.1);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/smg_shoot");
            SetWeaponAttackIntervals(maxAmmo: 25, reloadTime: 1.1, usesMagazine: true, reloadSoundPath: "TF2/Content/Sounds/SFX/smg_reload");
        }
    }
}