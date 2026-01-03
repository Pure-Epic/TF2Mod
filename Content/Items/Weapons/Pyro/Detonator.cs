using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Pyro;
using TF2.Content.Tiles.Crafting;

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
            SetWeaponAttackIntervals(maxAmmo: 16, altClick: true, noAmmo: true, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/flaregun_reload");
            SetFlareGun();
            noThe = true;
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
            return base.WeaponCanBeUsed(player) || (player.controlUseTile && currentAmmoClip > 0);
        }

        protected override bool WeaponPreAttack(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (projectile.ModProjectile is DetonatorFlare flareProjectile && projectile.active && projectile.owner == player.whoAmI && flareProjectile.weapon == this)
                        flareProjectile.DetonateProjectile();
                }
                return false;
            }
            return base.WeaponPreAttack(player);
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<FlareGun>().AddIngredient<ReclaimedMetal>(2).AddTile<AustraliumAnvil>().Register();
    }
}