using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Heavy;

namespace TF2.Content.Items.Heavy
{
    public class Natascha : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Primary, Unique, Unlock);
            SetWeaponSize(50, 31);
            SetWeaponOffset(-10f, 15f);
            SetGunUseStyle(minigun: true);
            SetWeaponDamage(damage: 6.75, projectile: ModContent.ProjectileType<NataschaBullet>(), shootAngle: 10f);
            SetWeaponAttackSpeed(0.105);
            SetWeaponAttackIntervals(altClick: true, noAmmo: true);
            SetMinigun(spinTime: 1.16, speed: 47);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            if (player.controlUseItem && spinTimer >= spinUpTime)
                player.GetModPlayer<MinigunDamageResistance>().minigunDamageResistance = true;
        }
    }

    public class MinigunDamageResistance : ModPlayer
    {
        public bool minigunDamageResistance;

        public override void ResetEffects() => minigunDamageResistance = false;

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (minigunDamageResistance && (modifiers.FinalDamage.Base >= Player.statLife - Player.statLifeMax2 / 2 || Player.statLife <= Player.statLifeMax2 / 2))
                modifiers.FinalDamage *= 0.8f;
        }
    }
}