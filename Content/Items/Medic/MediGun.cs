using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Projectiles.Medic;

namespace TF2.Content.Items.Medic
{
    public class MediGun : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Secondary, Stock, Starter);
            SetWeaponSize(50, 50);
            SetWeaponOffset(-10f);
            SetGunUseStyle(mediGun: true);
            SetWeaponDamage(projectile: ModContent.ProjectileType<HealingBeam>());
            SetWeaponAttackSpeed(0.01666, hide: true);
            SetWeaponAttackSound(SoundID.Item15);
            SetWeaponAttackIntervals(altClick: true, noAmmo: true);
            SetMediGun(buff: ModContent.BuffType<UberCharge>(), duration: 8);
        }
    }
}