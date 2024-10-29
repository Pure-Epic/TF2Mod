using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Heavy;
using TF2.Content.Items.Weapons.Soldier;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Demoman
{
    public class AliBabasWeeBooties : TF2Accessory
    {
        protected override string LegTexture => "TF2/Content/Textures/Items/Demoman/AliBabasWeeBooties";

        protected override int HealthBoost => 25;

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Primary, Unique, Craft);
            SetWeaponPrice(weapon: 2, refined: 1, scrap: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddPositiveAttribute(description);

        protected override bool WeaponAddTextureCondition(Player player) => player.GetModPlayer<AliBabasWeeBootiesPlayer>().aliBabasWeeBootiesEquipped;

        protected override bool WeaponModifyHealthCondition(Player player) => player.GetModPlayer<AliBabasWeeBootiesPlayer>().aliBabasWeeBootiesEquipped;

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<AliBabasWeeBootiesPlayer>().aliBabasWeeBootiesEquipped = true;

        public override void AddRecipes() => CreateRecipe().AddIngredient<Mantreads>().AddIngredient<GlovesOfRunningUrgently>().AddTile<AustraliumAnvil>().Register();
    }

    public class AliBabasWeeBootiesPlayer : ModPlayer
    {
        public bool aliBabasWeeBootiesEquipped;

        public override void ResetEffects() => aliBabasWeeBootiesEquipped = false;

        public override void UpdateEquips()
        {
            if (aliBabasWeeBootiesEquipped && Player.GetModPlayer<TF2Player>().HasShield)
                TF2Player.SetPlayerSpeed(Player, 110);
        }
    }
}