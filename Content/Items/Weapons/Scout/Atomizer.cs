using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Scout
{
    public class Atomizer : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 30);
            SetWeaponAttackSpeed(0.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponAttackIntervals(deploy: 0.75);
            SetWeaponPrice(weapon: 2, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponActiveUpdate(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (player.velocity.Y != 0f)
                p.miniCrit = true;
        }

        protected override void WeaponActiveBonus(Player player) => player.GetModPlayer<TF2Player>().extraJumps += 1;

        public override void AddRecipes() => CreateRecipe().AddIngredient<Sandman>().AddIngredient<BonkAtomicPunch>().AddIngredient<ReclaimedMetal>().AddTile<AustraliumAnvil>().Register();
    }
}