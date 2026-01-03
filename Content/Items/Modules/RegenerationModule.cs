using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Modules
{
    public class RegenerationModule : TF2Module
    {
        public override bool Passive => true;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            WeaponAddQuality(Unique);
            noThe = true;
            availability = Purchase;
        }

        protected override void WeaponDescription(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(Mod, "Positive Attributes", Language.GetText("Mods.TF2.UI.Items.RegenerationModule").Format(TF2.GetHealth(Main.LocalPlayer, 3)))
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);
            AddNeutralAttribute(tooltips);
        }

        protected override void ModuleUpdate(Player player)
        {
            RegenerationModulePlayer p = player.GetModPlayer<RegenerationModulePlayer>();
            if (player.GetModPlayer<TF2Player>().ClassSelected && !TF2Player.IsAtFullHealth(player))
            {
                p.healTimer++;
                if (p.healTimer >= TF2.Time(1))
                {
                    player.Heal(TF2.GetHealth(player, 3));
                    p.healTimer = 0;
                }
            }
            else
                p.healTimer = 0;
        }
    }

    public class RegenerationModulePlayer : ModPlayer
    {
        public int healTimer;
    }
}