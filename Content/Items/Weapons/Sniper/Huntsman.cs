using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles.Sniper;

namespace TF2.Content.Items.Weapons.Sniper
{
    public class Huntsman : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Sniper, Primary, Unique, Unlock);
            SetWeaponSize(11, 50);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 50, projectile: ModContent.ProjectileType<Arrow>(), projectileSpeed: 12.5f, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(1.94);
            SetWeaponAttackIntervals(maxReserve: 12, noAmmo: true, customReloadTime: 1.94);
            SetSniperRifle(chargeDamage: 70, maxChargeDuration: 1, zoomDelay: 0, speed: 45);
        }

        public override bool WeaponCanBeUsed(Player player) => cooldownTimer >= TF2.Time(1.94);

        protected override void WeaponActiveUpdate(Player player)
        {
            chargeTime = (int)Utils.Clamp(chargeTime, 0, maxChargeTime);
            if (chargeTime == maxChargeTime)
            {
                player.GetModPlayer<TF2Player>().crit = true;
                timer[0]++;
            }
            if (!player.controlUseItem && isCharging && !player.dead)
            {
                Vector2 shootDirection = timer[0] >= TF2.Time(5) ? player.DirectionTo(Main.MouseWorld).RotatedByRandom(MathHelper.ToRadians(60f)) : player.DirectionTo(Main.MouseWorld);
                Shoot(player, player.GetSource_ItemUse(Item), player.Center, shootDirection * Item.shootSpeed, ModContent.ProjectileType<Arrow>(), (int)Math.Round(chargeUpDamage * player.GetModPlayer<TF2Player>().damageMultiplier), 0f);
                SetCustomItemTime(player);
                sniperReload = true;
                chargeUpDamage = Item.damage;
                chargeTime = 0f;
                cooldownTimer = 0;
                timer[0] = 0;
                TF2.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/bow_shoot"), player.Center);
                isCharging = false;
            }
            if (player.controlUseTile && timer[0] > 0)
            {
                TF2.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/bow_shoot_pull_reverse"), player.Center);
                timer[0] = 0;
                chargeTime = 0f;
                isCharging = false;
                player.itemTime = 116;
            }
            if (player.dead)
                isCharging = false;
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            if (currentAmmoReserve > 0 || cooldownTimer > 0)
                cooldownTimer++;
            if (cooldownTimer > TF2.Time(1.94))
                cooldownTimer = TF2.Time(1.94);
            if (cooldownTimer == TF2.Time(1.94) && sniperReload)
            {
                if (!ModContent.GetInstance<TF2ConfigClient>().InfiniteAmmo)
                    currentAmmoReserve--;
                sniperReload = false;
            }
        }

        protected override bool? WeaponOnUse(Player player)
        {
            if (player.controlUseItem && GetCustomItemTime(player) == 0)
            {
                if (!isCharging)
                    TF2.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/bow_shoot_pull"), player.Center);
                isCharging = true;
                SniperRifleCharge();
                return false;
            }
            return base.WeaponOnUse(player);
        }
    }
}