using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.Medic;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Sniper
{
    public class Shahanshah : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Sniper, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponPrice(weapon: 2, reclaimed: 1, scrap: 3);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player) => Item.damage = TF2.Round(65 * ((player.statLife >= TF2Player.TotalHealth(player) / 2) ? 0.75f : 1.25f));

        public override void AddRecipes() => CreateRecipe().AddIngredient<Bushwacka>().AddIngredient<Amputator>().AddIngredient<ReclaimedMetal>().AddTile<AustraliumAnvil>().Register();
    }
}