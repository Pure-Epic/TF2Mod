using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.MultiClass
{
    public class Pistol : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(MultiClass, Secondary, Stock, Starter);
            SetWeaponClass(new int[] { Scout, Engineer });
            SetWeaponSize(30, 23);
            SetGunUseStyle(focus: true, automatic: true);
            SetWeaponDamage(damage: 15, projectile: ModContent.ProjectileType<Bullet>());
            SetWeaponAttackSpeed(0.15);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/pistol_shoot");
            SetWeaponAttackIntervals(maxAmmo: 12, reloadTime: 1.005, usesMagazine: true, reloadSoundPath: "TF2/Content/Sounds/SFX/pistol_reload");
        }

        protected override void WeaponMultiClassUpdate(Player player)
        {
            switch (player.GetModPlayer<TF2Player>().classIconID)
            {
                default:
                case Scout:
                    reloadRate = Time(1.005);
                    displayReloadSpeed = 1.005;
                    break;
                case Engineer:
                    reloadRate = Time(1.035);
                    displayReloadSpeed = 1.035;
                    break;
            }
        }
    }
}