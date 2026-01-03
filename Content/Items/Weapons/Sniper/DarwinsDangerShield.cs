using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Sniper
{
    public class DarwinsDangerShield : TF2Accessory
    {
        protected override string TorsoTexture => "TF2/Content/Textures/Items/Sniper/DarwinsDangerShieldTorso";

        protected override string TorsoTextureReverse => "TF2/Content/Textures/Items/Sniper/DarwinsDangerShieldTorsoReverse";

        protected override string BackTexture => "TF2/Content/Textures/Items/Sniper/DarwinsDangerShield";

        protected override string BackTextureReverse => "TF2/Content/Textures/Items/Sniper/DarwinsDangerShieldReverse";

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Sniper, Secondary, Unique, Craft);
            SetWeaponPrice(weapon: 1, reclaimed: 1);
            noThe = true;
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddPositiveAttribute(description);

        protected override bool WeaponAddTextureCondition(Player player) => player.GetModPlayer<DarwinsDangerShieldPlayer>().darwinsDangerShieldEquipped;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<DarwinsDangerShieldPlayer>().darwinsDangerShieldEquipped = true;
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                if (Main.debuff[i] && !BuffID.Sets.NurseCannotRemoveDebuff[i] && !TF2BuffBase.cooldownBuff[i])
                    player.buffImmune[i] = true;
            }
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<Razorback>().AddIngredient<ReclaimedMetal>().AddTile<AustraliumAnvil>().Register();
    }

    public class DarwinsDangerShieldPlayer : ModPlayer
    {
        public bool darwinsDangerShieldEquipped;

        public override void ResetEffects() => darwinsDangerShieldEquipped = false;
    }
}