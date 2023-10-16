using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Items.Medic;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Scout
{
    public class CritaCola : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Secondary, Unique, Craft);
            SetWeaponSize(19, 40);
            SetDrinkUseStyle();
            SetWeaponAttackSpeed(1.2, hide: true);
            SetWeaponAttackSound(SoundID.Item3);
            SetUtilityWeapon();
            SetWeaponPrice(weapon: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        public override bool WeaponCanBeUsed(Player player) => !player.HasBuff<DrinkCooldown>() && !player.HasBuff<ScoutMiniCrit>();

        protected override bool? WeaponOnUse(Player player)
        {
            player.AddBuff(ModContent.BuffType<ScoutMiniCrit>(), 480);
            player.AddBuff(ModContent.BuffType<MarkedForDeath>(), 300);
            player.AddBuff(ModContent.BuffType<DrinkCooldown>(), 1320);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Kritzkrieg>()
                .AddIngredient<BonkAtomicPunch>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}