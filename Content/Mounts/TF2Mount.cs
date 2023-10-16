using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Demoman;

namespace TF2.Content.Mounts
{
    public class TF2Mount : ModMount
    {
        public float speed;
        public bool idle;

        public override void SetStaticDefaults()
        {
            speed = 1;
            idle = true;
            MountData.usesHover = true;
            MountData.flightTimeMax = 1000;
            MountData.fatigueMax = 1000;
            MountData.totalFrames = 1;
            MountData.buff = ModContent.BuffType<TF2MountBuff>(); // The ID number of the buff assigned to the mount.
            MountData.playerYOffsets = Enumerable.Repeat(0, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code
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
                        homingText.Text = "Homing Power: " + (HomingPower)p.homingPower;
                        PopupText.NewText(homingText, player.position);
                    }
                    else
                    {
                        p.homingPower++;
                        homingText.Text = "Homing Power: " + (HomingPower)p.homingPower;
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
                        speedText.Text = "Speed: " + (MountSpeed)p.mountSpeed;
                        PopupText.NewText(speedText, player.position);
                    }
                    else
                    {
                        p.mountSpeed++;
                        speedText.Text = "Speed: " + (MountSpeed)p.mountSpeed;
                        PopupText.NewText(speedText, player.position);
                    }
                }
            }

            player.controlMount = true;
            if (player.controlJump)
            {
                if (!player.controlDown)
                    player.maxFallSpeed = 0f;
                if (!player.GetModPlayer<TF2Player>().disableFocusSlowdown)
                    speed = 0.5f * p.speedMultiplier;
                else if (player.GetModPlayer<TF2Player>().disableFocusSlowdown && p.mountSpeed == 0)
                    speed = p.speedMultiplier;
                else if (player.GetModPlayer<TF2Player>().disableFocusSlowdown && p.mountSpeed == 1)
                    speed = p.currentClass switch
                    {
                        1 => 1.5f * p.speedMultiplier,
                        _ => 2f * p.speedMultiplier
                    };
                p.focus = true;
            }
            else if (p.mountSpeed == 1)
            {
                speed = p.currentClass switch
                {
                    1 => 1.5f * p.speedMultiplier,
                    _ => 2f * p.speedMultiplier
                };
                p.focus = false;
            }
            else
            {
                speed = p.speedMultiplier;
                p.focus = false;
            }

            ShieldPlayer shield = player.GetModPlayer<TF2Player>().shieldType switch
            {
                1 => player.GetModPlayer<CharginTargePlayer>(),
                _ => player.GetModPlayer<CharginTargePlayer>()
            };

            if (player.controlUp)
            {
                if (!player.controlLeft && !player.controlRight && !p.backStab && !shield.chargeActive)
                    player.velocity.X = 0f;
                player.velocity = p.currentClass switch
                {
                    1 => new Vector2(player.velocity.X, -(speed * 15f)),
                    5 => new Vector2(player.velocity.X, -(speed * 10f)),
                    _ => new Vector2(player.velocity.X, -(speed * 12.5f))
                };
            }
            else if (player.controlDown)
            {
                if (!player.controlLeft && !player.controlRight && !p.backStab && !shield.chargeActive)
                    player.velocity.X = 0f;
                player.maxFallSpeed = p.currentClass switch
                {
                    1 => speed * 15f,
                    5 => speed * 10f,
                    _ => speed * 12.5f
                };
                player.velocity = p.currentClass switch
                {
                    1 => new Vector2(player.velocity.X, speed * 15f),
                    5 => new Vector2(player.velocity.X, speed * 10f),
                    _ => new Vector2(player.velocity.X, speed * 12.5f)
                };
            }
            if (player.controlLeft)
            {
                if (!player.controlUseItem || player.HeldItem.useTurn)
                    player.direction = -1;
                if (!player.controlUp && !player.controlDown)
                    player.velocity.Y = 0f;
                player.velocity = p.currentClass switch
                {
                    1 => new Vector2(-(speed * 15f), player.velocity.Y),
                    5 => new Vector2(-(speed * 10f), player.velocity.Y),
                    _ => new Vector2(-(speed * 12.5f), player.velocity.Y)
                };
            }
            else if (player.controlRight)
            {
                if (!player.controlUseItem || player.HeldItem.useTurn)
                    player.direction = 1;
                if (!player.controlUp && !player.controlDown)
                    player.velocity.Y = 0f;
                player.velocity = p.currentClass switch
                {
                    1 => new Vector2(speed * 15f, player.velocity.Y),
                    5 => new Vector2(speed * 10f, player.velocity.Y),
                    _ => new Vector2(speed * 12.5f, player.velocity.Y)
                };
            }
            idle = !player.controlUp && !player.controlDown && !player.controlLeft && !player.controlRight! && !p.backStab && !shield.chargeActive;

            if (idle)
            {
                player.velocity = new Vector2(0f, 0f);
                player.maxFallSpeed = 0f;
            }
            else if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);
        }

        public override void SetMount(Player player, ref bool skipDust) => skipDust = true;

        public override void Dismount(Player player, ref bool skipDust) => player.controlMount = false;

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow) => false;
    }

    public class TF2MountItem : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            List<string> currentHomingPowerKey = KeybindSystem.HomingPower.GetAssignedKeys(0);
            if (currentHomingPowerKey.Count <= 0 || currentHomingPowerKey.Contains("None"))
            {
                TooltipLine line = new TooltipLine(Mod, "Homing Attribute",
                "Assign keybind to change homing power");
                if (Main.hardMode)
                    tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new TooltipLine(Mod, "Homing Attribute",
                "Press " + currentHomingPowerKey[0] + " to change homing power");
                if (Main.hardMode)
                    tooltips.Add(line);
            }

            List<string> currentMountSpeedKey = KeybindSystem.MountSpeed.GetAssignedKeys(0);
            if (currentMountSpeedKey.Count <= 0 || currentMountSpeedKey.Contains("None"))
            {
                TooltipLine line2 = new TooltipLine(Mod, "Speed Attributes",
                    "Assign keybind to change to change speed");
                if (NPC.downedMoonlord)
                    tooltips.Add(line2);
            }
            else
            {
                TooltipLine line2 = new TooltipLine(Mod, "Speed Attributes",
                    "Press " + currentMountSpeedKey[0] + " to change speed");
                if (NPC.downedMoonlord)
                    tooltips.Add(line2);
            }
        }

        public override void SetDefaults()
        {
            Item.width = 1000;
            Item.height = 1000;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/horror");
            Item.noUseGraphic = true;
            Item.mountType = ModContent.MountType<TF2Mount>();
            Item.rare = ItemRarityID.Master;
            Item.value = Item.sellPrice(gold: 1);
        }
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
            player.buffTime[buffIndex] = 10;
        }
    }

    public class FocusMode : PlayerDrawLayer
    {
        private Asset<Texture2D> focusModeTexture;

        // Returning true in this property makes this layer appear on the minimap player head icon.
        // public override bool IsHeadLayer => true;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<TF2Player>().focus;

        // This layer will be a 'child' of the head layer, and draw before (beneath) it.
        // If the Head layer is hidden, this layer will also be hidden.
        // If the Head layer is moved, this layer will move with it.
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Wings);

        // If you want to make a layer which isn't a child of another layer, use `new Between(Layer1, Layer2)` to specify the position.
        // If you want to make a 'mobile' layer which can render in different locations depending on the drawInfo, use a `Multiple` position.

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (focusModeTexture == null)
                focusModeTexture = ModContent.Request<Texture2D>("TF2/Content/Textures/Focus");

            Vector2 position = drawInfo.Center - Main.screenPosition;
            position = new Vector2((int)position.X, (int)position.Y); // You'll sometimes want to do this, to avoid quivering.

            // Queues a drawing of a sprite. Do not use SpriteBatch in drawlayers!
            drawInfo.DrawDataCache.Add(new DrawData(
                focusModeTexture.Value, // The texture to render.
                position, // Position to render at.
                null, // Source rectangle.
                Color.White, // Color.
                0f, // Rotation.
                focusModeTexture.Size() * 0.5f, // Origin. Uses the texture's center.
                1f, // Scale.
                SpriteEffects.None, // SpriteEffects.
                0 // 'Layer'. This is always 0 in Terraria.
            ));
        }
    }

    public class FocusModeHitbox : PlayerDrawLayer
    {
        private Asset<Texture2D> focusModeHitboxTexture;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<TF2Player>().focus;

        public override Position GetDefaultPosition() => new Between();

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (focusModeHitboxTexture == null)
                focusModeHitboxTexture = ModContent.Request<Texture2D>("TF2/Content/Textures/Hitbox");

            Vector2 position = drawInfo.Center - Main.screenPosition;
            position = new Vector2((int)position.X, (int)position.Y);
            Texture2D texture = focusModeHitboxTexture.Value;
            Rectangle rectangle = texture.Bounds;

            drawInfo.DrawDataCache.Add(new DrawData(
                focusModeHitboxTexture.Value,
                position,
                rectangle,
                Color.White,
                0f,
                focusModeHitboxTexture.Size() * 0.5f,
                1f,
                SpriteEffects.None,
                0
            ));
        }
    }
}