using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Pyro;

namespace TF2.Content.Items.Pyro
{
    public class FlareGun : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Secondary, Unique, Unlock);
            SetWeaponSize(40, 40);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 30, projectile: ModContent.ProjectileType<Flare>(), projectileSpeed: 25f, knockback: 5f);
            SetWeaponAttackSpeed(0.25, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/flaregun_shoot");
            SetWeaponAttackIntervals(noAmmo: true, customReloadTime: 2, reloadSoundPath: "TF2/Content/Sounds/SFX/flaregun_reload");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNeutralAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player) => cooldownTimer >= Time(2);

        protected override bool WeaponCanConsumeAmmo(Player player) => true;

        protected override void WeaponActiveUpdate(Player player)
        {
            if (!finishReloadSound && cooldownTimer == Time(0.3333))
            {
                SoundEngine.PlaySound(reloadSound, player.Center);
                finishReloadSound = true;
            }
            if (cooldownTimer >= Time(2))
                finishReloadSound = false;
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            cooldownTimer++;
            if (cooldownTimer > Time(2))
                cooldownTimer = Time(2);
        }

        protected override bool WeaponPreAttack(Player player)
        {
            cooldownTimer = 0;
            return base.WeaponPreAttack(player);
        }
    }
}