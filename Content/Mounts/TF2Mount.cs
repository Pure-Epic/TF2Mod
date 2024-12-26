using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.Items.Weapons.Demoman;

namespace TF2.Content.Mounts
{
    public class TF2Mount : ModMount
    {
        public float speed;
        public bool idle;
        private readonly LocalizedText[] homingPowerText =
        [
            Language.GetText("Mods.TF2.UI.Items.Mount.Low"),
            Language.GetText("Mods.TF2.UI.Items.Mount.Medium"),
            Language.GetText("Mods.TF2.UI.Items.Mount.High")
        ];
        private readonly LocalizedText[] mountSpeedText =
        [
            Language.GetText("Mods.TF2.UI.Items.Mount.Normal"),
            Language.GetText("Mods.TF2.UI.Items.Mount.Fast")
        ];

        public override void SetStaticDefaults()
        {
            speed = 1;
            idle = true;
            MountData.usesHover = true;
            MountData.flightTimeMax = 1000;
            MountData.fatigueMax = 1000;
            MountData.totalFrames = 1;
            MountData.buff = ModContent.BuffType<TF2MountBuff>();
            MountData.playerYOffsets = Enumerable.Repeat(0, MountData.totalFrames).ToArray();
            MountData.standingFrameCount = 1;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;
            MountData.runningFrameCount = 1;
            MountData.runningFrameDelay = 12;
            MountData.runningFrameStart = 0;
            MountData.flyingFrameCount = 0;
            MountData.flyingFrameDelay = 0;
            MountData.flyingFrameStart = 0;
            MountData.inAirFrameCount = 1;
            MountData.inAirFrameDelay = 12;
            MountData.inAirFrameStart = 0;
            MountData.idleFrameCount = 1;
            MountData.idleFrameDelay = 12;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;
            if (!Main.dedServ)
            {
                MountData.textureWidth = MountData.backTexture.Width() + 20;
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }

        public enum MountSpeed
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

        public override void UpdateEffects(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                player.legFrame.Y = player.legFrame.Height;
                TF2Player p = player.GetModPlayer<TF2Player>();
                if (KeybindSystem.HomingPower.JustPressed)
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
                        {
                            p.homingPower = 0;
                            homingText.Text = Language.GetText("Mods.TF2.UI.Items.Mount.HomingPower").Format(homingPowerText[p.homingPower]);
                            PopupText.NewText(homingText, player.position);
                        }
                        else
                        {
                            p.homingPower++;
                            homingText.Text = Language.GetText("Mods.TF2.UI.Items.Mount.HomingPower").Format(homingPowerText[p.homingPower]);
                            PopupText.NewText(homingText, player.position);
                        }
                    }
                }
                if (KeybindSystem.MountSpeed.JustPressed)
                {
                    AdvancedPopupRequest speedText = new()
                    {
                        Color = Color.Aquamarine,
                        DurationInFrames = 30,
                        Velocity = new Vector2(0f, -5f)
                    };
                    if (NPC.downedMoonlord)
                    {
                        if (p.mountSpeed == 1)
                        {
                            p.mountSpeed = 0;
                            speedText.Text = Language.GetText("Mods.TF2.UI.Items.Mount.Speed").Format(mountSpeedText[p.mountSpeed]);
                            PopupText.NewText(speedText, player.position);
                        }
                        else
                        {
                            p.mountSpeed++;
                            speedText.Text = Language.GetText("Mods.TF2.UI.Items.Mount.Speed").Format(mountSpeedText[p.mountSpeed]);
                            PopupText.NewText(speedText, player.position);
                        }
                    }
                }
                player.controlMount = true;
                player.noKnockback = true;
                if (player.controlJump)
                {
                    if (!player.controlDown)
                        player.maxFallSpeed = 0f;
                    if (!p.disableFocusSlowdown)
                        speed = p.speedMultiplier * 0.5f;
                    else if (p.disableFocusSlowdown && p.mountSpeed == 0)
                        speed = p.speedMultiplier;
                    else if (p.disableFocusSlowdown && p.mountSpeed == 1)
                        speed = p.speedMultiplier * 2f;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        p.focus = true;
                }
                else if (p.mountSpeed == 1)
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
                ShieldPlayer shield = ShieldPlayer.GetShield(player);
                if (player.controlUp)
                {
                    if (!player.controlLeft && !player.controlRight && !p.backStab && !shield.chargeActive)
                        player.velocity.X = 0f;
                    player.velocity = new Vector2(player.velocity.X, -(speed * 12.5f));
                    SendMountMessage(player);
                }
                else if (player.controlDown)
                {
                    if (!player.controlLeft && !player.controlRight && !p.backStab && !shield.chargeActive)
                        player.velocity.X = 0f;
                    player.velocity = new Vector2(player.velocity.X, speed * 12.5f);
                    player.maxFallSpeed = speed * 12.5f;
                    SendMountMessage(player);
                }
                if (player.controlLeft)
                {
                    if (!player.controlUseItem || player.HeldItem.useTurn)
                        player.direction = -1;
                    if (!player.controlUp && !player.controlDown)
                        player.velocity.Y = 0f;
                    player.velocity = new Vector2(-(speed * 12.5f), player.velocity.Y);
                    SendMountMessage(player);
                }
                else if (player.controlRight)
                {
                    if (!player.controlUseItem || player.HeldItem.useTurn)
                        player.direction = 1;
                    if (!player.controlUp && !player.controlDown)
                        player.velocity.Y = 0f;
                    player.velocity = new Vector2(speed * 12.5f, player.velocity.Y);
                    SendMountMessage(player);
                }
                idle = !player.controlUp && !player.controlDown && !player.controlLeft && !player.controlRight! && !p.backStab && !shield.chargeActive;
                if (idle)
                {
                    player.velocity = new Vector2(0f, 0f);
                    player.maxFallSpeed = 0f;
                    SendMountMessage(player);
                }
            }
        }

        public override void SetMount(Player player, ref bool skipDust) => skipDust = true;

        public override void Dismount(Player player, ref bool skipDust)
        {
            skipDust = true;
            player.controlMount = false;
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow) => false;

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

    public class TF2MountItem : TF2Item
    {
        public override void SetDefaults()
        {
            Item.width = 1000;
            Item.height = 1000;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/manns_anti_danmaku_system_activate");
            Item.noUseGraphic = true;
            Item.mountType = ModContent.MountType<TF2Mount>();
            Item.rare = ModContent.RarityType<NormalRarity>();
            qualityHashSet.Add(Stock);
            availability = Starter;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine nameTooltip = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria");
            AddName(tooltips);
            tooltips.Remove(nameTooltip);
            RemoveDefaultTooltips(tooltips);
            TooltipLine category = new TooltipLine(Mod, "Weapon Category", Language.GetText("Mods.TF2.UI.Items.Mount.Description").Format(availabilityNames[availability], Language.GetTextValue("Mods.TF2.UI.Items.Module")))
            {
                OverrideColor = new Color(117, 107, 94)
            };
            tooltips.Insert(tooltips.FindLastIndex(x => x.Name == "Name" && x.Mod == "TF2") + 1, category);
            AddNeutralAttribute(tooltips);
            List<string> currentHomingPowerKey = KeybindSystem.HomingPower.GetAssignedKeys(0);
            TooltipLine line = new TooltipLine(Mod, "Homing Attribute", Language.GetText("Mods.TF2.UI.Items.Mount.MountInstruction").Format(currentHomingPowerKey[0]));
            if (currentHomingPowerKey.Count <= 0 || currentHomingPowerKey.Contains("None"))
                line = new TooltipLine(Mod, "Homing Attribute", Language.GetTextValue("Mods.TF2.UI.Items.Mount.MountInstructionUnassigned"));
            line.OverrideColor = new Color(235, 226, 202);
            if (Main.hardMode)
                tooltips.Add(line);
            List<string> currentMountSpeedKey = KeybindSystem.MountSpeed.GetAssignedKeys(0);
            TooltipLine line2 = new TooltipLine(Mod, "Speed Attribute", Language.GetText("Mods.TF2.UI.Items.Mount.MountInstruction2").Format(currentMountSpeedKey[0]));
            if (currentMountSpeedKey.Count <= 0 || currentMountSpeedKey.Contains("None"))
                line2 = new TooltipLine(Mod, "Speed Attribute", Language.GetTextValue("Mods.TF2.UI.Items.Mount.MountInstruction2Unassigned"));
            line2.OverrideColor = new Color(235, 226, 202);
            if (NPC.downedMoonlord)
                tooltips.Add(line2);
            if (Item.favorited)
            {
                TooltipLine favorite = new TooltipLine(Mod, "Favorite", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[56].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202)
                };
                tooltips.Add(favorite);
                TooltipLine favoriteDescription = new TooltipLine(Mod, "Favorite Description", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[57].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202)
                };
                tooltips.Add(favoriteDescription);
                if (Main.LocalPlayer.chest > -1)
                {
                    ChestUI.GetContainerUsageInfo(out bool sync, out Item[] chestinv);
                    if (ChestUI.IsBlockedFromTransferIntoChest(Item, chestinv))
                    {
                        TooltipLine noTransfer = new TooltipLine(Mod, "No Transfer", FontAssets.MouseText.Value.CreateWrappedText(Language.GetTextValue("UI.ItemCannotBePlacedInsideItself"), 350f))
                        {
                            OverrideColor = new Color(235, 226, 202)
                        };
                        tooltips.Add(favorite);
                    }
                }
            }
            TooltipLine priceTooltip = tooltips.FirstOrDefault(x => x.Name == "Price" && x.Mod == "Terraria");
            TooltipLine price = priceTooltip;
            tooltips.Add(price);
            tooltips.Remove(priceTooltip);
            TooltipLine specialPriceTooltip = tooltips.FirstOrDefault(x => x.Name == "SpecialPrice" && x.Mod == "Terraria");
            TooltipLine specialPrice = specialPriceTooltip;
            tooltips.Add(specialPrice);
            tooltips.Remove(specialPriceTooltip);
            TooltipLine journeyResearchTooltip = tooltips.FirstOrDefault(x => x.Name == "JourneyResearch" && x.Mod == "Terraria");
            TooltipLine journeyModeTooltip = journeyResearchTooltip;
            tooltips.Add(journeyModeTooltip);
            tooltips.Remove(journeyResearchTooltip);
        }

        protected override bool WeaponModifyDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!Main.LocalPlayer.GetModPlayer<TF2Player>().ClassSelected)
                spriteBatch.Draw(TextureAssets.Cd.Value, position - TextureAssets.InventoryBack9.Value.Size() / 4.225f * Main.inventoryScale, null, drawColor, 0f, new Vector2(0.5f, 0.5f), 0.8f * Main.inventoryScale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool CanUseItem(Player player) => Main.LocalPlayer.GetModPlayer<TF2Player>().ClassSelected;
    }

    public class TF2MountBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<TF2Mount>(), player);
            player.noKnockback = true;
            player.gravity = 0f;
            player.buffTime[buffIndex] = 10;
        }
    }

    public class FocusMode : PlayerDrawLayer
    {
        private Asset<Texture2D> focusModeTexture;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Wings);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (!drawInfo.drawPlayer.GetModPlayer<TF2Player>().focus) return;
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
            if (!drawInfo.drawPlayer.GetModPlayer<TF2Player>().focus) return;
            focusModeHitboxTexture ??= ModContent.Request<Texture2D>("TF2/Content/Textures/Hitbox");
            Vector2 position = drawInfo.Center - Main.screenPosition;
            position = new Vector2((int)position.X, (int)position.Y);
            Texture2D texture = focusModeHitboxTexture.Value;
            Rectangle rectangle = texture.Bounds;
            drawInfo.DrawDataCache.Add(new DrawData(focusModeHitboxTexture.Value, position, rectangle, Color.White, 0f, focusModeHitboxTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0));
        }
    }
}