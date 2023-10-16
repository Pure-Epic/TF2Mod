using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Demoman;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items
{
    public class HorselessHeadlessHorsemannsHeadtaker : Eyelander
    {
        public override string Texture => "TF2/Content/Textures/HorselessHeadlessHorsemannsHeadtaker";

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Melee, Unusual, Craft);
            SetSwingUseStyle(sword: true);
            SetWeaponDamage(damage: 65, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/demo_sword_swing1");
            SetWeaponAttackIntervals(deploy: 0.875, holster: 0.875);
            SetWeaponPrice(weapon: 12, refined: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddSwordAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RefinedMetal>(2)
                .AddIngredient<HauntedMetalScrap>()
                .AddIngredient<ScotsmansSkullcutter>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}