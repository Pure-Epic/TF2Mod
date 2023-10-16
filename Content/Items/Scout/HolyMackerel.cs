using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Scout
{
    public class HolyMackerel : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 35);
            SetWeaponAttackSpeed(0.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/melee_swing");
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Sandman>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}