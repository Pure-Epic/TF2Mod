using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.MultiClass
{
    public class Shotgun : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(MultiClass, Primary, Stock, Starter);
            SetWeaponClass(new int[] { Soldier, Pyro, Heavy, Engineer });
            SetWeaponSize(50, 12);
            SetGunUseStyle();
            SetWeaponDamage(damage: 6, projectile: ModContent.ProjectileType<Bullet>(), projectileCount: 10, shootAngle: 12f);
            SetWeaponAttackSpeed(0.625);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/shotgun_shoot");
            SetWeaponAttackIntervals(maxAmmo: 6, reloadTime: 0.51, initialReloadTime: 0.87, reloadSoundPath: "TF2/Content/Sounds/SFX/shotgun_reload");
        }

        protected override void WeaponMultiClassUpdate(Player player)
        {
            switch (player.GetModPlayer<TF2Player>().classIconID)
            {
                case Soldier:
                    SetWeaponSlot(Secondary);
                    initialReloadRate = Time(1.005);
                    break;
                case Pyro:
                    SetWeaponSlot(Secondary);
                    initialReloadRate = Time(1.035);
                    break;
                case Heavy:
                    SetWeaponSlot(Secondary);
                    initialReloadRate = Time(0.87);
                    break;
                case Engineer:
                default:
                    SetWeaponSlot(Primary);
                    initialReloadRate = Time(0.87);
                    break;
            }
        }
    }
}