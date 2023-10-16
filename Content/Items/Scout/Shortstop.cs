using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Scout;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Scout
{
    public class Shortstop : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Primary, Unique, Craft);
            SetWeaponSize(40, 40);
            SetGunUseStyle();
            SetWeaponDamage(damage: 12, projectile: ModContent.ProjectileType<Bullet>(), projectileCount: 4, shootAngle: 7.2f);
            SetWeaponAttackSpeed(0.36);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/scatter_gun_shoot");
            SetWeaponAttackIntervals(maxAmmo: 4, reloadTime: 1.52, usesMagazine: true, reloadSoundPath: "TF2/Content/Sounds/SFX/short_stop_reload");
            SetTimers(Time(1.5));
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override bool WeaponMagazineCanBeUsed(Player player) => !(currentAmmoClip <= 0 || currentAmmoClip < ammoCost || reload || player.altFunctionUse == 2);

        protected override void WeaponActiveUpdate(Player player)
        {
            if (player.controlUseTile && timer[0] >= Time(1.5) && WeaponCanAltClick(player) && !player.ItemAnimationActive)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, player.DirectionTo(Main.MouseWorld), ModContent.ProjectileType<ShoveHitbox>(), 1, 0f, player.whoAmI);
                timer[0] = 0;
            }
            timer[0]++;
            if (timer[0] >= Time(1.5))
                timer[0] = Time(1.5);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ForceANature>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class ShortstopPlayer : ModPlayer
    {
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (Player.HeldItem.ModItem is Shortstop)
                modifiers.Knockback *= 1.2f;
        }
    }
}