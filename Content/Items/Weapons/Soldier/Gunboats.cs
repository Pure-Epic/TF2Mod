﻿using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Demoman;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class Gunboats : TF2Accessory
    {
        protected override string LegTexture => "TF2/Content/Textures/Items/Soldier/Gunboats";

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Secondary, Unique, Craft);
            SetWeaponPrice(weapon: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override bool WeaponAddTextureCondition(Player player) => player.GetModPlayer<GunboatsPlayer>().gunboatsEquipped;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GunboatsPlayer>().gunboatsEquipped = true;
            player.GetModPlayer<TF2Player>().noRandomHealthKits = true;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<Razorback>().AddIngredient<CharginTarge>().AddTile<AustraliumAnvil>().Register();
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