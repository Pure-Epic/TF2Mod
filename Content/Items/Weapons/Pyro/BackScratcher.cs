using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Demoman;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Pyro
{
    public class BackScratcher : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 81);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponPrice(weapon: 3);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<BackScratcherPlayer>().backScratcherEquipped = true;

        public override void AddRecipes() => CreateRecipe().AddIngredient<Axtinguisher>().AddIngredient<ScotsmansSkullcutter>().AddTile<CraftingAnvil>().Register();
    }

    public class BackScratcherPlayer : ModPlayer
    {
        public bool backScratcherEquipped;

        public override void ResetEffects() => backScratcherEquipped = false;

        public override void PostUpdate()
        {
            if (backScratcherEquipped && Main.netMode == NetmodeID.SinglePlayer)
                Player.GetModPlayer<TF2Player>().healReduction *= 0.25f;
        }
    }
}