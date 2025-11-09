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
    [LegacyName("TF2MountItem")]
    public class DarkAntiDanmakuSystem : TF2Module
    {
        public override int ModuleBuff => ModContent.BuffType<DarkAntiDanmakuSystemBuff>();

        public override bool AutomaticActivation => true;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/Modules/dark_anti_danmaku_system_activate");
            WeaponAddQuality(Unique);
            noThe = true;
            availability = Exclusive;
        }

        protected override void WeaponDescription(List<TooltipLine> tooltips)
        {
            AddNeutralAttribute(tooltips);
            List<string> homingPowerKey = KeybindSystem.ModulePower.GetAssignedKeys(0);
            TooltipLine line = new TooltipLine(Mod, "Homing Attribute", Language.GetTextValue("Mods.TF2.UI.Items.MannsAntiDanmakuSystem.InstructionUnassigned"));
            if (homingPowerKey.Count > 0)
                line = new TooltipLine(Mod, "Homing Attribute", Language.GetText("Mods.TF2.UI.Items.MannsAntiDanmakuSystem.Instruction").Format(homingPowerKey[0]));
            line.OverrideColor = new Color(235, 226, 202);
            if (Main.hardMode)
                tooltips.Add(line);
            List<string> currentSpeedKey = KeybindSystem.ModuleSecondaryPower.GetAssignedKeys(0);
            TooltipLine line2 = new TooltipLine(Mod, "Speed Attribute", Language.GetTextValue("Mods.TF2.UI.Items.MannsAntiDanmakuSystem.Instruction2Unassigned"));
            if (currentSpeedKey.Count > 0)
                line2 = new TooltipLine(Mod, "Speed Attribute", Language.GetText("Mods.TF2.UI.Items.MannsAntiDanmakuSystem.Instruction2").Format(currentSpeedKey[0]));
            line2.OverrideColor = new Color(235, 226, 202);
            if (NPC.downedMoonlord)
                tooltips.Add(line2);
        }
    }

    public class DarkAntiDanmakuSystemBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<MannsAntiDanmakuSystemPlayer>().mannsAntiDanmakuSystemActive = true;
            player.noKnockback = true;
            player.gravity = 0f;
        }

        public override bool RightClick(int buffIndex)
        {
            Main.LocalPlayer.GetModPlayer<TF2Player>().moduleActivated = false;
            return base.RightClick(buffIndex);
        }
    }

    public class DarkFocusMode : PlayerDrawLayer
    {
        private Asset<Texture2D> focusModeTexture;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Wings);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (!(drawInfo.drawPlayer.GetModPlayer<TF2Player>().focus && drawInfo.drawPlayer.HasBuff<DarkAntiDanmakuSystemBuff>())) return;
            focusModeTexture ??= ModContent.Request<Texture2D>("TF2/Content/Textures/DarkFocus");
            Vector2 position = drawInfo.Center - Main.screenPosition;
            position = new Vector2((int)position.X, (int)position.Y);
            drawInfo.DrawDataCache.Add(new DrawData(focusModeTexture.Value, position, null, Color.White, 0f, focusModeTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0));
        }
    }

    public class DarkFocusModeHitbox : PlayerDrawLayer
    {
        private Asset<Texture2D> focusModeHitboxTexture;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

        public override Position GetDefaultPosition() => new Between();

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (!(drawInfo.drawPlayer.GetModPlayer<TF2Player>().focus && drawInfo.drawPlayer.HasBuff<DarkAntiDanmakuSystemBuff>())) return;
            focusModeHitboxTexture ??= ModContent.Request<Texture2D>("TF2/Content/Textures/DarkHitbox");
            Vector2 position = drawInfo.Center - Main.screenPosition;
            position = new Vector2((int)position.X, (int)position.Y);
            Texture2D texture = focusModeHitboxTexture.Value;
            Rectangle rectangle = texture.Bounds;
            drawInfo.DrawDataCache.Add(new DrawData(focusModeHitboxTexture.Value, position, rectangle, Color.White, 0f, focusModeHitboxTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0));
        }
    }
}