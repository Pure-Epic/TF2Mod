using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles.Pyro;

namespace TF2.Content.Items.Weapons.Pyro
{
    public class FlareGun : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Secondary, Unique, Unlock);
            SetWeaponSize(40, 40);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 30, projectile: ModContent.ProjectileType<Flare>(), projectileSpeed: 25f, knockback: 5f, distanceModifier: false);
            SetWeaponAttackSpeed(0.25, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/flaregun_shoot");
            SetWeaponAttackIntervals(maxAmmo: 16, noAmmo: true, customReloadTime: 2, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/flaregun_reload");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNeutralAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player) => cooldownTimer >= TF2.Time(2) && currentAmmoClip > 0;

        protected override void WeaponActiveUpdate(Player player)
        {
            if (!finishReloadSound && cooldownTimer == TF2.Time(0.3333))
            {
                SoundEngine.PlaySound(reloadSound, player.Center);
                finishReloadSound = true;
            }
            if (cooldownTimer >= TF2.Time(2))
                finishReloadSound = false;
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            if (currentAmmoClip > 0)
                cooldownTimer++;
            if (cooldownTimer > TF2.Time(2))
                cooldownTimer = TF2.Time(2);
        }

        protected override bool WeaponPreAttack(Player player)
        {
            cooldownTimer = 0;
            if (!ModContent.GetInstance<TF2ConfigClient>().InfiniteAmmo)
                currentAmmoClip--;
            return base.WeaponPreAttack(player);
        }
    }
}