using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Items.Weapons.Spy
{
    public class CloakAndDagger : TF2Accessory
    {
        protected override void WeaponStatistics() => SetWeaponCategory(Spy, PDA, Unique, Unlock);

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddHeader(description);
            List<string> currentCloakKey = KeybindSystem.Cloak.GetAssignedKeys(0);
            if (currentCloakKey.Count <= 0 || currentCloakKey.Contains("None"))
                AddOtherAttribute(description, (string)this.GetLocalization("Header2"));
            else
                AddOtherAttribute(description, currentCloakKey[0] + (string)this.GetLocalization("Header3"));
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<CloakAndDaggerBuffPlayer>().cloakAndDaggerEquipped = true;
    }
}