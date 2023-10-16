using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Pyro;
using TF2.Content.Items.Sniper;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Demoman
{
    public class ScotsmansSkullcutter : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Melee, Unique, Craft);
            SetSwingUseStyle(sword: true);
            SetWeaponDamage(damage: 78, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/demo_sword_swing1");
            SetWeaponAttackIntervals(deploy: 0.875, holster: 0.875);
            SetWeaponPrice(weapon: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddSwordAttribute(description);
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override bool WeaponResetHolsterTimeCondition(Player player) => player.controlUseItem;

        protected override void WeaponPassiveUpdate(Player player) => SetPlayerSpeed(player, 85);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Jarate>()
                .AddIngredient<Axtinguisher>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}