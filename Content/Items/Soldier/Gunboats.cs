using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Demoman;
using TF2.Content.Items.Sniper;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Soldier
{
    public class Gunboats : TF2AccessorySecondary
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Secondary, Unique, Craft);
            SetWeaponPrice(weapon: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddPositiveAttribute(description);

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<GunboatsPlayer>().gunboatsEquipped = true;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Razorback>()
                .AddIngredient<CharginTarge>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class GunboatsPlayer : ModPlayer
    {
        public bool gunboatsEquipped;

        public override void ResetEffects() => gunboatsEquipped = false;

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (gunboatsEquipped)
                modifiers.FinalDamage *= 0.4f;
        }
    }
}