using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles.Sniper;

namespace TF2.Content.Items.Sniper
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
            SetWeaponAttackIntervals(noAmmo: true);
            SetSniperRifle(chargeDamage: 70, maxChargeTime: 1, zoomDelay: 0, speed: 45);
        }

        protected override bool WeaponCanConsumeAmmo(Player player) => ModContent.GetInstance<TF2ConfigClient>().Channel;

        protected override void WeaponActiveUpdate(Player player)
        {
            Item.UseSound = ModContent.GetInstance<TF2ConfigClient>().Channel ? new SoundStyle("TF2/Content/Sounds/SFX/bow_shoot") : null;
            maxChargeUp = !ModContent.GetInstance<TF2ConfigClient>().Channel ? 60f : 60f + Item.useTime;
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.sniperCharge = chargeTime;
            p.sniperMaxCharge = maxChargeUp;
            chargeTime = (int)Utils.Clamp(chargeTime, 0, maxChargeUp);
            if (ModContent.GetInstance<TF2ConfigClient>().Channel)
                SniperRifleCharge();
            if (chargeTime == maxChargeUp)
            {
                p.crit = true;
                timer[0]++;
            }

            if (!player.controlUseItem && isCharging && !ModContent.GetInstance<TF2ConfigClient>().Channel && !player.dead)
            {
                Vector2 shootDirection = timer[0] >= 300 ? player.DirectionTo(Main.MouseWorld).RotatedByRandom(MathHelper.ToRadians(60f)) : player.DirectionTo(Main.MouseWorld);
                FocusShot(player, player.GetSource_ItemUse(Item), player.Center, shootDirection * Item.shootSpeed, ModContent.ProjectileType<Arrow>(), (int)Math.Round(chargeUpDamage * player.GetModPlayer<TF2Player>().classMultiplier), 0f);

                SetCustomItemTime(player);
                chargeUpDamage = Item.damage;
                chargeTime = 0f;
                timer[0] = 0;
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/bow_shoot"), player.Center);
                isCharging = false;
                ChargeWeaponConsumeAmmo(player);
            }

            if (player.controlUseTile && timer[0] > 0)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/bow_shoot_pull_reverse"), player.Center);
                timer[0] = 0;
                chargeTime = 0f;
                isCharging = false;
                player.itemTime = 116;
            }

            if (player.dead)
                isCharging = false;
        }

        protected override void WeaponAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!ModContent.GetInstance<TF2ConfigClient>().Channel) return;
            Vector2 newVelocity = timer[0] >= 240 ? velocity.RotatedByRandom(MathHelper.ToRadians(60f)) : velocity;
            WeaponFireProjectile(player, source, position, newVelocity, ModContent.ProjectileType<Arrow>(), (int)Math.Round(chargeUpDamage * player.GetModPlayer<TF2Player>().classMultiplier), knockback);

            chargeUpDamage = Item.damage;
            chargeTime = 0f;
            timer[0] = 0;
        }

        protected override bool? WeaponOnUse(Player player)
        {
            if (player.controlUseItem && GetCustomItemTime(player) == 0 && !ModContent.GetInstance<TF2ConfigClient>().Channel)
            {
                if (!isCharging)
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/bow_shoot_pull"), player.Center);
                isCharging = true;
                SniperRifleCharge();
                return false;
            }
            return base.WeaponOnUse(player);
        }
    }
}