using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Pyro;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Pyro
{
    public class Degreaser : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Primary, Unique, Craft);
            SetWeaponSize(50, 16);
            SetGunUseStyle();
            SetWeaponDamage(damage: 78, projectile: ModContent.ProjectileType<DegreaserFire>(), projectileSpeed: 15f, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.1, 0.5, hide: true);
            SetWeaponAttackIntervals(altClick: true, noAmmo: true);
            SetWeaponPrice(weapon: 1, reclaimed: 1);
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

        protected override void WeaponFireProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                Vector2 muzzleOffset = Vector2.Normalize(velocity) * 54f;
                if (Collision.CanHit(position, 6, 6, position + muzzleOffset, 6, 6))
                {
                    position += muzzleOffset;
                    SoundEngine.PlaySound(SoundID.Item34, player.Center);
                    Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
                }
            }
            else
            {
                player.itemAnimationMax = Item.useTime;
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/flame_thrower_airblast"), player.Center);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Airblast>(), 1, knockback, player.whoAmI);
                player.statMana -= 25;
                player.manaRegenDelay = 125;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Backburner>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}