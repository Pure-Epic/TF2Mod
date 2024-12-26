using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.Demoman;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons
{
    public class HorselessHeadlessHorsemannsHeadtaker : Eyelander
    {
        protected override bool Reskin => true;

        public override string Texture => "TF2/Content/Textures/HorselessHeadlessHorsemannsHeadtaker";

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Melee, Unusual, Craft);
            SetSwingUseStyle(sword: true);
            SetWeaponDamage(damage: 65, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/demo_sword_swing1");
            SetWeaponAttackIntervals(deploy: 1, holster: 1);
            SetWeaponPrice(weapon: 12, refined: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddSwordAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<ScotsmansSkullcutter>().AddIngredient<HauntedMetalScrap>().AddIngredient<RefinedMetal>(2).AddTile<AustraliumAnvil>().Register();
    }
}