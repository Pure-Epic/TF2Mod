using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Content.Items;
using TF2.Gensokyo.Content.Projectiles.Pyro;

namespace TF2.Gensokyo.Content.Items.Pyro
{
    [ExtendsFromMod("Gensokyo")]
    public class ManualInferno : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Primary, Unique, Exclusive);
            SetWeaponSize(55, 22);
            SetWeaponOffset(-7.5f);
            SetGunUseStyle();
            SetWeaponDamage(damage: 78, projectile: ModContent.ProjectileType<HellFire>());
            SetWeaponAttackSpeed(0.1, 0.5, hide: true);
            SetWeaponAttackIntervals(maxAmmo: 65, reloadTime: 1.5, reloadSoundPath: "TF2/Gensokyo/Content/Sounds/SFX/manualinferno_reload", altClick: true, usesMagazine: true);
            SetWeaponPrice(weapon: 10);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
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