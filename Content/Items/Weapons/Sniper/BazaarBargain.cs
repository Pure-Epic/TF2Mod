using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.Demoman;
using TF2.Content.Projectiles;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Sniper
{
    public class BazaarBargain : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Sniper, Primary, Unique, Craft);
            SetWeaponSize(50, 15);
            SetWeaponOffset(-5f);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 50, projectile: ModContent.ProjectileType<Bullet>(), noRandomCriticalHits: true);
            SetWeaponAttackSpeed(1.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/bazaar_bargain_shoot");
            SetWeaponAttackIntervals(maxAmmo: 25, noAmmo: true);
            SetSniperRifle(maxChargeDuration: 4, zoomDelay: 2.3);
            SetWeaponPrice(weapon: 2, reclaimed: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            int heads = player.GetModPlayer<BazaarBargainPlayer>().heads;
            maxChargeTime = heads switch
            {
                0 => TF2.Time(4f),
                1 => TF2.Time(2.66666f),
                2 => TF2.Time(2f),
                3 => TF2.Time(1.6f),
                4 => TF2.Time(1.33333f),
                5 => TF2.Time(1.13333f),
                6 => TF2.Time(1f),
                _ => 0f
            };
            chargeUpDelay = TF2.Time(0.3f) + heads switch
            {
                0 => TF2.Time(2f),
                1 => TF2.Time(1.33333f),
                2 => TF2.Time(1f),
                3 => TF2.Time(0.8f),
                4 => TF2.Time(0.66666f),
                5 => TF2.Time(0.56666f),
                6 => TF2.Time(0.5f),
                _ => 0f
            };
        }

        protected override void WeaponPostFireProjectile(Player player, int projectile)
        {
            if (chargeTime == maxChargeTime)
                (Main.projectile[projectile].ModProjectile as TF2Projectile).bazaarBargainProjectile = true;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<SydneySleeper>().AddIngredient<Eyelander>().AddIngredient<ReclaimedMetal>().AddTile<CraftingAnvil>().Register();
    }

    public class BazaarBargainPlayer : ModPlayer
    {
        public int heads;

        public override void OnRespawn() => heads = 0;
    }
}