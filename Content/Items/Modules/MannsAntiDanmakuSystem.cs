using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Demoman;

namespace TF2.Content.Items.Modules
{
    public class MannsAntiDanmakuSystem : TF2Module
    {
        public override int ModuleBuff => ModContent.BuffType<MannsAntiDanmakuSystemBuff>();

        public override int ModuleTimeLimit => TF2.Time(10);

        public override bool AutomaticActivation => true;

        public override bool Unlocked => TF2.IsBossAlive();

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/Modules/manns_anti_danmaku_system_activate");
            WeaponAddQuality(Unique);
            availability = Starter;
        }

        protected override void WeaponDescription(List<TooltipLine> tooltips)
        {
            AddNegativeAttribute(tooltips);
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

        protected override bool WeaponModifyDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            TF2Player p = Main.LocalPlayer.GetModPlayer<TF2Player>();
            if (!p.ClassSelected || (p.moduleBuff != ModContent.BuffType<MannsAntiDanmakuSystemBuff>() && !Unlocked))
                spriteBatch.Draw(TextureAssets.Cd.Value, position - TextureAssets.InventoryBack9.Value.Size() / 4.225f * Main.inventoryScale, null, drawColor, 0f, new Vector2(0.5f, 0.5f), 0.8f * Main.inventoryScale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class MannsAntiDanmakuSystemPlayer : ModPlayer
    {
        public bool mannsAntiDanmakuSystemActive;
        private float speed;
        private bool idle;
        private readonly LocalizedText[] homingPowerText =
        [
            Language.GetText("Mods.TF2.UI.Items.MannsAntiDanmakuSystem.Low"),
            Language.GetText("Mods.TF2.UI.Items.MannsAntiDanmakuSystem.Medium"),
            Language.GetText("Mods.TF2.UI.Items.MannsAntiDanmakuSystem.High")
        ];
        private readonly LocalizedText[] moduleSpeedText =
        [
            Language.GetText("Mods.TF2.UI.Items.MannsAntiDanmakuSystem.Normal"),
            Language.GetText("Mods.TF2.UI.Items.MannsAntiDanmakuSystem.Fast")
        ];

        public enum moduleSpeed
        {
            Normal,
            Fast,
        }

        public enum HomingPower
        {
            Low,
            Medium,
            High
        }

        public override void PostUpdate()
        {
            if (mannsAntiDanmakuSystemActive)
            {
                if (!Player.ItemAnimationActive)
                    Player.bodyFrame.Location = new Point(0, 0);
                Player.legFrame.Location = new Point(0, 0);
                TF2Player p = Player.GetModPlayer<TF2Player>();
                if (KeybindSystem.ModulePower.JustPressed)
                {
                    AdvancedPopupRequest homingText = new()
                    {
                        Color = Color.MediumOrchid,
                        DurationInFrames = 30,
                        Velocity = new Vector2(0, -5)
                    };
                    int maximumPower;
                    if (NPC.downedMoonlord)
                        maximumPower = 2;
                    else if (Main.hardMode)
                        maximumPower = 1;
                    else
                        maximumPower = 0;
                    if (Main.hardMode)
                    {
                        if (p.homingPower == maximumPower)
                            p.homingPower = 0;
                        else
                            p.homingPower++;
                        homingText.Text = Language.GetText("Mods.TF2.UI.Items.MannsAntiDanmakuSystem.HomingPower").Format(homingPowerText[p.homingPower]);
                        PopupText.NewText(homingText, Player.position);
                    }
                }
                if (KeybindSystem.ModuleSecondaryPower.JustPressed)
                {
                    AdvancedPopupRequest speedText = new()
                    {
                        Color = Color.Aquamarine,
                        DurationInFrames = 30,
                        Velocity = new Vector2(0f, -5f)
                    };
                    if (NPC.downedMoonlord)
                    {
                        if (p.moduleSpeed == 1)
                            p.moduleSpeed = 0;
                        else
                            p.moduleSpeed++;
                        speedText.Text = Language.GetText("Mods.TF2.UI.Items.MannsAntiDanmakuSystem.Speed").Format(moduleSpeedText[p.moduleSpeed]);
                        PopupText.NewText(speedText, Player.position);
                    }
                }
                Player.noKnockback = true;
                if (Player.controlJump)
                {
                    if (!Player.controlDown)
                        Player.maxFallSpeed = 0f;
                    if (!p.disableFocusSlowdown)
                        speed = p.speedMultiplier * 0.5f;
                    else if (p.disableFocusSlowdown && p.moduleSpeed == 0)
                        speed = p.speedMultiplier;
                    else if (p.disableFocusSlowdown && p.moduleSpeed == 1)
                        speed = p.speedMultiplier * 2f;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        p.focus = true;
                }
                else if (p.moduleSpeed == 1)
                {
                    speed = p.speedMultiplier * 2f;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        p.focus = false;
                }
                else
                {
                    speed = p.speedMultiplier;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        p.focus = false;
                }
                ShieldPlayer shield = ShieldPlayer.GetShield(Player);
                if (Player.controlUp)
                {
                    if (!Player.controlLeft && !Player.controlRight && !p.backStab && !shield.chargeActive)
                        Player.velocity.X = 0f;
                    Player.velocity = new Vector2(Player.velocity.X, -(speed * 12.5f));
                    SendMountMessage(Player);
                }
                else if (Player.controlDown)
                {
                    if (!Player.controlLeft && !Player.controlRight && !p.backStab && !shield.chargeActive)
                        Player.velocity.X = 0f;
                    Player.velocity = new Vector2(Player.velocity.X, speed * 12.5f);
                    Player.maxFallSpeed = speed * 12.5f;
                    SendMountMessage(Player);
                }
                if (Player.controlLeft)
                {
                    if (!Player.controlUseItem || Player.HeldItem.useTurn)
                        Player.direction = -1;
                    if (!Player.controlUp && !Player.controlDown)
                        Player.velocity.Y = 0f;
                    Player.velocity = new Vector2(-(speed * 12.5f), Player.velocity.Y);
                    SendMountMessage(Player);
                }
                else if (Player.controlRight)
                {
                    if (!Player.controlUseItem || Player.HeldItem.useTurn)
                        Player.direction = 1;
                    if (!Player.controlUp && !Player.controlDown)
                        Player.velocity.Y = 0f;
                    Player.velocity = new Vector2(speed * 12.5f, Player.velocity.Y);
                    SendMountMessage(Player);
                }
                idle = !Player.controlUp && !Player.controlDown && !Player.controlLeft && !Player.controlRight! && !p.backStab && !shield.chargeActive;
                if (idle)
                {
                    Player.velocity = new Vector2(0f, 0f);
                    Player.maxFallSpeed = 0f;
                    SendMountMessage(Player);
                }
            }
        }

        public override void ResetEffects() => mannsAntiDanmakuSystemActive = false;

        public static void SendMountMessage(Player player)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
                packet.Write((byte)TF2.MessageType.SyncMount);
                packet.Write((byte)player.whoAmI);
                packet.Write(player.Center.X);
                packet.Write(player.Center.Y);
                packet.Write(player.direction);
                packet.Send(ignoreClient: player.whoAmI);
            }
        }
    }

    public class MannsAntiDanmakuSystemBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
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

    public class FocusMode : PlayerDrawLayer
    {
        private Asset<Texture2D> focusModeTexture;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Wings);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (!(drawInfo.drawPlayer.GetModPlayer<TF2Player>().focus && drawInfo.drawPlayer.HasBuff<MannsAntiDanmakuSystemBuff>())) return;
            focusModeTexture ??= ModContent.Request<Texture2D>("TF2/Content/Textures/Focus");
            Vector2 position = drawInfo.Center - Main.screenPosition;
            position = new Vector2((int)position.X, (int)position.Y);
            drawInfo.DrawDataCache.Add(new DrawData(focusModeTexture.Value, position, null, Color.White, 0f, focusModeTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0));
        }
    }

    public class FocusModeHitbox : PlayerDrawLayer
    {
        private Asset<Texture2D> focusModeHitboxTexture;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

        public override Position GetDefaultPosition() => new Between();

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (!(drawInfo.drawPlayer.GetModPlayer<TF2Player>().focus && drawInfo.drawPlayer.HasBuff<MannsAntiDanmakuSystemBuff>())) return;
            focusModeHitboxTexture ??= ModContent.Request<Texture2D>("TF2/Content/Textures/Hitbox");
            Vector2 position = drawInfo.Center - Main.screenPosition;
            position = new Vector2((int)position.X, (int)position.Y);
            Texture2D texture = focusModeHitboxTexture.Value;
            Rectangle rectangle = texture.Bounds;
            drawInfo.DrawDataCache.Add(new DrawData(focusModeHitboxTexture.Value, position, rectangle, Color.White, 0f, focusModeHitboxTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0));
        }
    }
}