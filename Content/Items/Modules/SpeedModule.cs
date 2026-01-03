using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Modules
{
    public class SpeedModule : TF2Module
    {
        public override int ModuleBuff => ModContent.BuffType<SpeedModuleBuff>();

        public override bool AutomaticActivation => true;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/Modules/speed_module_activate");
            WeaponAddQuality(Unique);
            noThe = true;
            availability = Purchase;
        }

        protected override void WeaponDescription(List<TooltipLine> tooltips)
        {
            SpeedModulePlayer p = Main.LocalPlayer.GetModPlayer<SpeedModulePlayer>();
            TooltipLine line = new TooltipLine(Mod, "Positive Attributes", Language.GetText("Mods.TF2.UI.Items.SpeedModule.Upsides").Format(p.speedMultiplier[p.speed]))
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);
            List<string> speedKey = KeybindSystem.ModulePower.GetAssignedKeys(0);
            TooltipLine line2 = new TooltipLine(Mod, "Speed Keybind", Language.GetTextValue("Mods.TF2.UI.Items.SpeedModule.InstructionUnassigned"));
            if (speedKey.Count > 0)
                line2 = new TooltipLine(Mod, "Speed Keybind", Language.GetText("Mods.TF2.UI.Items.SpeedModule.Instruction").Format(speedKey[0]));
            line2.OverrideColor = new Color(235, 226, 202);
            if (Main.hardMode)
                tooltips.Add(line2);
        }
    }

    public class SpeedModulePlayer : ModPlayer
    {
        public int speed;
        public int[] speedMultiplier =
        [
            200,
            400,
            800
        ];

        public override void PostUpdate()
        {
            if (Player.HasBuff<SpeedModuleBuff>())
            {
                if (KeybindSystem.ModulePower.JustPressed)
                {
                    AdvancedPopupRequest speedText = new()
                    {
                        Color = Color.LightCoral,
                        DurationInFrames = 30,
                        Velocity = new Vector2(0, -5)
                    };
                    int maximumSpeed;
                    if (NPC.downedMoonlord)
                        maximumSpeed = 2;
                    else if (Main.hardMode)
                        maximumSpeed = 1;
                    else
                        maximumSpeed = 0;
                    if (Main.hardMode)
                    {
                        if (speed == maximumSpeed)
                            speed = 0;
                        else
                            speed++;
                        speedText.Text = Language.GetText("Mods.TF2.UI.Items.SpeedModule.Speed").Format(speedMultiplier[speed]);
                        PopupText.NewText(speedText, Player.position);
                    }
                }
            }
        }
    }

    public class SpeedModuleBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            SpeedModulePlayer p = Main.LocalPlayer.GetModPlayer<SpeedModulePlayer>();
            TF2Player.SetPlayerSpeed(player, p.speedMultiplier[p.speed]);
        }
    }
}