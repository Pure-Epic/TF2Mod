using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles.Demoman;
using TF2.Content.Projectiles.Pyro;

namespace TF2.Content.Items.Weapons.Pyro
{
    public class Detonator : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Secondary, Unique, Craft);
            SetWeaponSize(40, 27);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 23, projectile: ModContent.ProjectileType<DetonatorFlare>(), projectileSpeed: 25f, knockback: 5f, distanceModifier: false);
            SetWeaponAttackSpeed(0.25, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/detonator_shoot");
            SetWeaponAttackIntervals(maxAmmo: 16, altClick: true, noAmmo: true, customReloadTime: 2, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/flaregun_reload");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player)
        {
            Item.UseSound = player.altFunctionUse != 2 ? new SoundStyle("TF2/Content/Sounds/SFX/Weapons/detonator_shoot") : null;
            return (cooldownTimer >= TF2.Time(2) && currentAmmoClip > 0) || player.controlUseTile;
        }

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
            TF2.Maximum(ref cooldownTimer, TF2.Time(2));
        }

        protected override bool WeaponPreAttack(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (projectile.ModProjectile is DetonatorFlare flareProjectile && projectile.active && projectile.owner == player.whoAmI && flareProjectile.weapon == this)
                        projectile.timeLeft = 0;
                }
                return false;
            }
            cooldownTimer = 0;
            if (!ModContent.GetInstance<TF2ConfigClient>().InfiniteAmmo)
                currentAmmoClip--;
            return base.WeaponPreAttack(player);
        }
    }
}