using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Modules
{
    public class GravityModule : TF2Module
    {
        public override int ModuleBuff => ModContent.BuffType<GravityModuleBuff>();

        public override bool AutomaticActivation => true;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/Modules/gravity_module_activate");
            WeaponAddQuality(Unique);
            noThe = true;
            availability = Purchase;
        }

        protected override void WeaponDescription(List<TooltipLine> tooltips)
        {
            GravityModulePlayer p = Main.LocalPlayer.GetModPlayer<GravityModulePlayer>();
            TooltipLine line = new TooltipLine(Mod, "Positive Attributes", Language.GetText("Mods.TF2.UI.Items.GravityModule.Upsides").Format(p.gravityMultiplier[p.gravity] * 100))
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);
            List<string> gravityKey = KeybindSystem.ModulePower.GetAssignedKeys(0);
            TooltipLine line2 = new TooltipLine(Mod, "Gravity Keybind", Language.GetTextValue("Mods.TF2.UI.Items.GravityModule.InstructionUnassigned"));
            if (gravityKey.Count > 0)
                line2 = new TooltipLine(Mod, "Gravity Keybind", Language.GetText("Mods.TF2.UI.Items.GravityModule.Instruction").Format(gravityKey[0]));
            line2.OverrideColor = new Color(235, 226, 202);
            if (Main.hardMode)
                tooltips.Add(line2);
        }
    }

    public class GravityModulePlayer : ModPlayer
    {
        public int gravity;
        public float[] gravityMultiplier =
        [
            0.75f,
            0.85f,
            0.95f
        ];

        public override void PostUpdate()
        {
            if (Player.HasBuff<GravityModuleBuff>())
            {
                if (KeybindSystem.ModulePower.JustPressed)
                {
                    AdvancedPopupRequest gravityText = new()
                    {
                        Color = Color.LightSkyBlue,
                        DurationInFrames = 30,
                        Velocity = new Vector2(0, -5)
                    };
                    int maximumGravity;
                    if (NPC.downedMoonlord)
                        maximumGravity = 2;
                    else if (Main.hardMode)
                        maximumGravity = 1;
                    else
                        maximumGravity = 0;
                    if (Main.hardMode)
                    {
                        if (gravity == maximumGravity)
                            gravity = 0;
                        else
                            gravity++;
                        gravityText.Text = Language.GetText("Mods.TF2.UI.Items.GravityModule.Gravity").Format(gravityMultiplier[gravity] * 100);
                        PopupText.NewText(gravityText, Player.position);
                    }
                }
            }
        }
    }

    public class GravityModuleBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            GravityModulePlayer p = player.GetModPlayer<GravityModulePlayer>();
            player.noKnockback = true;
            player.gravity *= 1 - p.gravityMultiplier[p.gravity];
        }
    }
}