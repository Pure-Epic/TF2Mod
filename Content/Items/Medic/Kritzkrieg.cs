using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Projectiles.Medic;

namespace TF2.Content.Items.Medic
{
    public class Kritzkrieg : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Secondary, Unique, Unlock);
            SetWeaponSize(50, 50);
            SetWeaponOffset(-10f);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(projectile: ModContent.ProjectileType<HealingBeamKritzkrieg>());
            SetWeaponAttackSpeed(0.01666, hide: true);
            SetWeaponAttackSound(SoundID.Item15);
            SetWeaponAttackIntervals(altClick: true, noAmmo: true);
            SetMediGun(buff: ModContent.BuffType<KritzkriegUberCharge>(), duration: 8);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddHeader(description);
            AddPositiveAttribute(description);
        }
    }
}