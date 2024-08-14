using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.MultiClass;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class MarketGardener : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.96);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponPrice(weapon: 3, reclaimed: 1, scrap: 1);
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
                p.crit = true;
            p.noRandomHealthKits = true;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<PainTrain>().AddIngredient<Gunboats>().AddIngredient<ReclaimedMetal>().AddTile<CraftingAnvil>().Register();
    }
}