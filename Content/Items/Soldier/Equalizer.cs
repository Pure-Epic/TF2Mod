using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Soldier
{
    public class Equalizer : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Melee, Unlock, Unique);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 33);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/shovel_swing");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddHeader(description);
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        /*
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            if (tt != null)
            {
                trueDamage = Convert.ToInt32((107.25f - 0.37295f * Main.LocalPlayer.statLife / Main.LocalPlayer.statLifeMax2 * 200f) * Main.LocalPlayer.GetModPlayer<TF2Player>().classMultiplier);
                tt.Text = Language.GetTextValue(trueDamage + " mercenary damage");
            }
            TooltipLine tt2 = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt2);

            TooltipLine line = new TooltipLine(Mod, "Neutral Attributes",
                "When weapon is active:")
            {
                OverrideColor = new Color(235, 226, 202)
            };
            tooltips.Add(line);

            TooltipLine line2 = new TooltipLine(Mod, "Positive Attributes",
                "Damage increases as the user becomes injured")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line2);

            TooltipLine line3 = new TooltipLine(Mod, "Negative Attributes",
                "-90% less healing from Medic sources")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line3);
        }
        */

        protected override void WeaponActiveUpdate(Player player) => Item.damage = Convert.ToInt32(107.25f - 0.37295f * player.statLife / player.statLifeMax2 * 200f);

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<EqualizerPlayer>().equalizerEquipped = true;
    }

    public class EqualizerPlayer : ModPlayer
    {
        public bool equalizerEquipped;

        public override void ResetEffects() => equalizerEquipped = false;

        public override void PostUpdate()
        {
            if (equalizerEquipped)
                Player.GetModPlayer<TF2Player>().healReduction *= 0.1f;
        }
    }
}