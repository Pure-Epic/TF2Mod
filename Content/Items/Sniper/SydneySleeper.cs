using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Sniper;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Sniper
{
    public class SydneySleeper : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Sniper, Primary, Unique, Craft);
            SetWeaponSize(50, 13);
            SetWeaponOffset(-5f);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 50, projectile: ModContent.ProjectileType<SydneySleeperDart>(), noRandomCriticalHits: true);
            SetWeaponAttackSpeed(1.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/sniper_shoot");
            SetWeaponAttackIntervals(noAmmo: true);
            SetSniperRifle(maxChargeTime: 1.4);
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPostFireProjectile(Player player, int projectile)
        {
            if (chargeTime > 0)
            {
                SydneySleeperDart dart = (SydneySleeperDart)Main.projectile[projectile].ModProjectile;
                dart.jarateDuration = Time(2) + (int)(Time(3) * (chargeTime / maxChargeUp));
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Huntsman>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}