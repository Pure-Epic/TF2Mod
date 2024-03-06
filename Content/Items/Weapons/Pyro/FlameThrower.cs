using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Pyro;

namespace TF2.Content.Items.Weapons.Pyro
{
    public class FlameThrower : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Primary, Stock, Starter);
            SetWeaponSize(50, 14);
            SetGunUseStyle();
            SetWeaponDamage(damage: 78, projectile: ModContent.ProjectileType<Fire>());
            SetWeaponAttackSpeed(0.105, hide: true);
            SetWeaponAttackIntervals(maxAmmo: 200, altClick: true, noAmmo: true);
            SetFlamethrower();
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNeutralAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player)
        {
            if (player.altFunctionUse == 2 && currentAmmoClip >= airblastCost && airblastTimer >= airblastCooldown)
                player.itemAnimation = player.itemAnimationMax = airblastCooldown;
            return ((player.altFunctionUse != 2 && currentAmmoClip > 0) || (player.altFunctionUse == 2 && currentAmmoClip >= airblastCost)) && airblastTimer >= airblastCooldown;
        }

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<TF2Player>().noRandomAmmoBoxes = true;

        protected override void WeaponActiveUpdate(Player player)
        {
            airblastTimer++;
            if (airblastTimer >= airblastCooldown)
                airblastTimer = airblastCooldown;
        }

        public override void WeaponDistanceModifier(Player player, Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (projectile.ModProjectile is TF2Projectile tf2Projectile && !tf2Projectile.crit && !tf2Projectile.miniCrit)
                modifiers.FinalDamage *= Utils.Clamp((float)projectile.timeLeft / TF2.Time(1), 0.5f, 1f);
        }
    }
}