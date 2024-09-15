using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons;
using TF2.Gensokyo.Content.Projectiles.Pyro;

namespace TF2.Gensokyo.Content.Items.Weapons.Pyro
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
            SetWeaponDamage(damage: 18, projectile: ModContent.ProjectileType<HellFire>());
            SetWeaponAttackSpeed(0.1, hide: true);
            SetWeaponAttackIntervals(maxAmmo: 65, maxReserve: 130, reloadTime: 1.5, reloadSoundPath: "TF2/Gensokyo/Content/Sounds/SFX/manualinferno_reload", altClick: true, usesMagazine: true);
            SetFlamethrower();
            SetWeaponPrice(weapon: 10);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
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

        protected override void WeaponFireProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => FlamethrowerProjectile(player, source, position, velocity, Item.shoot, damage, knockback);
    }
}