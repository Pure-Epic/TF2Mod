using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Modules
{
    public class PhoenixModule : TF2Module
    {
        public override int ModuleBuff => ModContent.BuffType<PhoenixModuleBuff>();

        public override bool AutomaticActivation => true;

        public static int wingSlot = -1;

        protected override void WeaponLoad() => wingSlot = EquipLoader.AddEquipTexture(Mod, "TF2/Content/Textures/Items/Modules/PhoenixModuleWings", EquipType.Wings, name: "PhoenixModuleWings");

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/Modules/phoenix_module_activate");
            WeaponAddQuality(Unique);
            noThe = true;
            availability = Purchase;
            ArmorIDs.Wing.Sets.Stats[wingSlot] = new WingStats(TF2.Time(10));
        }

        protected override void WeaponDescription(List<TooltipLine> tooltips)
        {
            AddPositiveAttribute(tooltips);
            List<string> phoenixKey = KeybindSystem.ModulePower.GetAssignedKeys(0);
            TooltipLine line2 = new TooltipLine(Mod, "Phoenix Keybind", Language.GetTextValue("Mods.TF2.UI.Items.PhoenixModule.InstructionUnassigned"));
            if (phoenixKey.Count > 0)
                line2 = new TooltipLine(Mod, "Phoenix Keybind", Language.GetText("Mods.TF2.UI.Items.PhoenixModule.Instruction").Format(phoenixKey[0]));
            line2.OverrideColor = new Color(235, 226, 202);
            if (Main.hardMode)
                tooltips.Add(line2);
        }

        protected override void ModuleUpdate(Player player)
        {
            if (player.HasBuff<PhoenixModuleBuff>())
            {
                player.wingsLogic = wingSlot;
                player.equippedWings = Item;
            }
        }
    }

    public class PhoenixModulePlayer : ModPlayer
    {
        public int flightMode;
        private readonly LocalizedText[] flightModeText =
        [
            Language.GetText("Mods.TF2.UI.Items.PhoenixModule.Low"),
            Language.GetText("Mods.TF2.UI.Items.PhoenixModule.Medium"),
            Language.GetText("Mods.TF2.UI.Items.PhoenixModule.High")
        ];
        public float[] flightTime =
        [
            0.5f,
            3f,
            0.5f
        ];

        public enum FlightMode
        {
            Low,
            Medium,
            High
        }

        public override void UpdateEquips()
        {
            if (Player.HasBuff<PhoenixModuleBuff>())
            {
                Player.wingTimeMax = TF2.Time(flightTime[flightMode]);
                if (flightMode == 2)
                    Player.wingTime = Player.wingTimeMax;
            }
        }

        public override void PostUpdate()
        {
            if (Player.HasBuff<PhoenixModuleBuff>())
            {
                if (KeybindSystem.ModulePower.JustPressed)
                {
                    AdvancedPopupRequest flightText = new()
                    {
                        Color = Color.Orange,
                        DurationInFrames = 30,
                        Velocity = new Vector2(0, -5)
                    };
                    int maximumFlight;
                    if (NPC.downedMoonlord)
                        maximumFlight = 2;
                    else if (Main.hardMode)
                        maximumFlight = 1;
                    else
                        maximumFlight = 0;
                    if (Main.hardMode)
                    {
                        if (flightMode == maximumFlight)
                            flightMode = 0;
                        else
                            flightMode++;
                        flightText.Text = Language.GetText("Mods.TF2.UI.Items.PhoenixModule.FlightMode").Format(flightModeText[flightMode]);
                        PopupText.NewText(flightText, Player.position);
                    }
                }
                Player.wings = PhoenixModule.wingSlot;
            }
        }
    }

    public class PhoenixModuleBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
    }
}