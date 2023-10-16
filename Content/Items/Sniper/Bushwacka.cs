using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Demoman;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Sniper
{
    public class Bushwacka : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Sniper, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/melee_swing");
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddHeader(description);
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponActiveBonus(Player player)
        {
            if (player.GetModPlayer<TF2Player>().miniCrit)
                player.GetModPlayer<TF2Player>().crit = true;
        }

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<BushwackaPlayer>().bushwackaEquipped = true;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Eyelander>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class BushwackaPlayer : ModPlayer
    {
        public bool bushwackaEquipped;

        public override void ResetEffects() => bushwackaEquipped = false;

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (bushwackaEquipped)
                modifiers.FinalDamage *= 1.2f;
        }
    }
}