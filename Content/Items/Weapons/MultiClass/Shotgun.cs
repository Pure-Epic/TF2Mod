using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.Weapons.MultiClass
{
    public class Shotgun : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(MultiClass, Primary, Stock, Starter);
            SetWeaponClass([Soldier, Pyro, Heavy, Engineer]);
            SetWeaponSize(50, 12);
            SetGunUseStyle();
            SetWeaponDamage(damage: 6, projectile: ModContent.ProjectileType<Bullet>(), projectileCount: 10, shootAngle: 12f);
            SetWeaponAttackSpeed(0.625);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/shotgun_shoot");
            SetWeaponAttackIntervals(maxAmmo: 6, maxReserve: 32, reloadTime: 0.51, initialReloadTime: 0.87, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/shotgun_reload");
        }

        protected override void WeaponEarlyUpdate(Player player)
        {
            switch (player.GetModPlayer<TF2Player>().currentClass)
            {
                case Soldier:
                    SetWeaponSlot(Secondary);
                    initialReloadRate = TF2.Time(1.005);
                    break;
                case Pyro:
                    SetWeaponSlot(Secondary);
                    initialReloadRate = TF2.Time(1.035);
                    break;
                case Heavy:
                    SetWeaponSlot(Secondary);
                    initialReloadRate = TF2.Time(0.87);
                    break;
                case Engineer:
                default:
                    SetWeaponSlot(Primary);
                    initialReloadRate = TF2.Time(0.87);
                    break;
            }
        }

        protected override void WeaponEquip(Player player)
        {
            int type = player.GetModPlayer<TF2Player>().currentClass switch
            {
                Engineer => Primary,
                Soldier or Pyro or Heavy => Secondary,
                _ => Primary
            };
            SetWeaponSlot(type);
            equipped = Item == GetWeapon(player, type);
        }
    }
}