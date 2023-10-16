using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Pyro;

namespace TF2.Content.Items.Pyro
{
    public class FlameThrower : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Primary, Stock, Starter);
            SetWeaponSize(50, 14);
            SetGunUseStyle();
            SetWeaponDamage(damage: 78, projectile: ModContent.ProjectileType<Fire>());
            SetWeaponAttackSpeed(0.1, 0.5, hide: true);
            SetWeaponAttackIntervals(altClick: true, noAmmo: true);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNeutralAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player)
        {
            if (player.controlUseTile && !(airblastCooldown >= 45 && player.statMana >= 20)) return false;
            Item.useTime = player.altFunctionUse != 2 ? 6 : 30;
            return base.WeaponCanBeUsed(player);
        }

        protected override bool WeaponCanConsumeAmmo(Player player) => player.altFunctionUse != 2 && player.itemAnimation >= player.itemAnimationMax - 5;

        protected override void WeaponActiveUpdate(Player player)
        {
            airblastCooldown++;
            if (airblastCooldown >= 45)
                airblastCooldown = 45;
        }

        protected override void WeaponFireProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => FlamethrowerProjectile(player, source, position, velocity, Item.shoot, damage, knockback);
    }
}