using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Items.Weapons.Spy
{
    public class InvisWatch : TF2Accessory
    {
        protected override void WeaponStatistics() => SetWeaponCategory(Spy, PDA, Stock, Starter);

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            List<string> currentCloakKey = KeybindSystem.Cloak.GetAssignedKeys(0);
            if (currentCloakKey.Count <= 0 || currentCloakKey.Contains("None"))
                AddNeutralAttribute(description);
            else
                AddOtherAttribute(description, currentCloakKey[0] + (string)this.GetLocalization("Notes2"));
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<CloakPlayer>().invisWatchEquipped = true;
    }
}