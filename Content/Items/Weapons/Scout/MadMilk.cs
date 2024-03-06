using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.Projectiles.Scout;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Scout
{
    public class MadMilk : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Secondary, Unique, Craft);
            SetWeaponSize(21, 50);
            SetThrowableUseStyle();
            SetWeaponDamage(projectile: ModContent.ProjectileType<MadMilkProjectile>());
            SetWeaponAttackSpeed(0.5, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Voicelines/scout_taunts14");
            SetUtilityWeapon(itemUseGraphic: false);
            SetTimers(TF2.Time(20));
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNeutralAttribute(description);
            AddLore(description);
        }

        public override bool WeaponCanBeUsed(Player player) => timer[0] >= TF2.Time(20);

        protected override void WeaponPassiveUpdate(Player player)
        {
            if (timer[0] < TF2.Time(20))
                timer[0]++;
        }

        protected override bool? WeaponOnUse(Player player)
        {
            timer[0] = 0;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Jarate>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}