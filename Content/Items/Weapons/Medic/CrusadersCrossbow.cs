using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Medic;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Medic
{
    public class CrusadersCrossbow : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Primary, Unique, Craft);
            SetWeaponSize(40, 40);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 75, projectile: ModContent.ProjectileType<CrusadersCrossbowSyringe>(), projectileSpeed: 25f);
            SetWeaponAttackSpeed(0.24);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/crusaders_crossbow_shoot");
            SetWeaponAttackIntervals(maxReserve: 38, noAmmo: true, customReloadTime: 1.75, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/syringegun_reload");
            SetWeaponPrice(weapon: 1, scrap: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player) => cooldownTimer >= TF2.Time(1.75);

        protected override bool WeaponCanConsumeAmmo(Player player) => true;

        protected override void WeaponActiveUpdate(Player player)
        {
            if (!finishReloadSound && cooldownTimer == TF2.Time(0.24))
            {
                SoundEngine.PlaySound(reloadSound, player.Center);
                finishReloadSound = true;
            }
            if (cooldownTimer >= TF2.Time(1.51) && finishReloadSound)
            {
                if (!ModContent.GetInstance<TF2ConfigClient>().InfiniteAmmo)
                    currentAmmoReserve--;
                finishReloadSound = false;
            }
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            if (currentAmmoReserve > 0 || cooldownTimer > 0)
                cooldownTimer++;
            if (cooldownTimer > TF2.Time(1.75))
                cooldownTimer = TF2.Time(1.75);
        }

        protected override bool WeaponPreAttack(Player player)
        {
            cooldownTimer = 0;
            return base.WeaponPreAttack(player);
        }

        public override void WeaponDistanceModifier(Player player, Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (projectile.ModProjectile is TF2Projectile tf2Projectile && !tf2Projectile.crit && !tf2Projectile.miniCrit)
                modifiers.FinalDamage *= 0.5f + Utils.Clamp(Vector2.Distance(player.Center, target.Center) / 500f, 0f, 0.5f);
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<Huntsman>().AddIngredient<ScrapMetal>(2).AddTile<CraftingAnvil>().Register();
    }
}