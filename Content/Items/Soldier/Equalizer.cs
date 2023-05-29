using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Soldier
{
    public class Equalizer : TF2WeaponMelee
    {
        public int trueDamage;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Soldier's Unlocked Melee\n"
                + "When weapon is active:");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 48;
            Item.useAnimation = 48;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/shovel_swing");
            Item.autoReuse = true;

            Item.damage = 33;

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) // needs System.Linq
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            if (tt != null)
            {
                trueDamage = Convert.ToInt32((107.25f - 0.37295f * Main.LocalPlayer.statLife / Main.LocalPlayer.statLifeMax2 * 200f) * Main.LocalPlayer.GetModPlayer<TF2Player>().classMultiplier);
                tt.Text = Language.GetTextValue(trueDamage + " mercenary damage");
            }
            TooltipLine tt2 = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt2);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "Damage increases as the user becomes injured")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "-90% less healing from Medic sources")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
        }

        public override void HoldItem(Player player)
        {
            player.GetModPlayer<TF2Player>().equalizer = true;
            Item.damage = Convert.ToInt32(107.25f - 0.37295f * Main.LocalPlayer.statLife / Main.LocalPlayer.statLifeMax2 * 200f);
        }
    }
}