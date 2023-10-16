using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Mounts
{
    public class TF2Mount : ModMount
    {
        public float speed;
        public bool idle;

        public override void SetStaticDefaults()
        {
            speed = 1;

            // Movement
            MountData.usesHover = true;
            idle = true;
            MountData.jumpHeight = 0; // How high the mount can jump.
            MountData.jumpSpeed = 0f; // The rate at which the player and mount ascend towards (negative y velocity) the jump height when the jump button is presssed.
            MountData.acceleration = 0f; // The rate at which the mount speeds up.
            MountData.blockExtraJumps = false; // Determines whether or not you can use a double jump (like cloud in a bottle) while in the mount.
            MountData.heightBoost = 0; // Height between the mount and the ground
            MountData.fallDamage = 0f; // Fall damage multiplier.
            MountData.runSpeed = 0f; // The speed of the mount
            MountData.dashSpeed = 0f; // The speed the mount moves when in the state of dashing.
            MountData.flightTimeMax = 1000; // The amount of time in frames a mount can be in the state of flying.
            MountData.fatigueMax = 1000;
            MountData.totalFrames = 1;

            MountData.buff = ModContent.BuffType<TF2MountBuff>(); // The ID number of the buff assigned to the mount.

            MountData.playerYOffsets = Enumerable.Repeat(0, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code
            MountData.xOffset = 0;
            MountData.yOffset = 0;
            MountData.playerHeadOffset = 0;
            MountData.bodyFrame = 0;
            // Standing
            MountData.standingFrameCount = 1;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;
            // Running
            MountData.runningFrameCount = 1;
            MountData.runningFrameDelay = 12;
            MountData.runningFrameStart = 0;
            // Flying
            MountData.flyingFrameCount = 0;
            MountData.flyingFrameDelay = 0;
            MountData.flyingFrameStart = 0;
            // In-air
            MountData.inAirFrameCount = 1;
            MountData.inAirFrameDelay = 12;
            MountData.inAirFrameStart = 0;
            // Idle
            MountData.idleFrameCount = 1;
            MountData.idleFrameDelay = 12;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;
            // Swim
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
                    Velocity = new Vector2(0, -5)
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
                        _ => 2f * p.speedMultiplier,
                    };
                player.statDefense += (int)(player.statLifeMax2 / 50f);
                p.focus = true;
            }
            else if (p.mountSpeed == 1)
            {
                speed = p.currentClass switch
                {
                    1 => 1.5f * p.speedMultiplier,
                    _ => 2f * p.speedMultiplier,
                };
                p.focus = false;
            }
            else
            {
                speed = p.speedMultiplier;
                p.focus = false;
            }

            if (idle && !(p.backStab || player.GetModPlayer<Items.Demoman.ShieldPlayer>().chargeActive))
            {
                player.velocity = new Vector2(0f, 0f);
                player.maxFallSpeed = 0f;
            }

            if (player.controlUp)
            {
                if (!player.controlLeft && !player.controlRight && !p.backStab && !player.GetModPlayer<Items.Demoman.ShieldPlayer>().chargeActive)
                    player.velocity.X = 0f;
                player.velocity = p.currentClass switch
                {
                    1 => new Vector2(player.velocity.X, -(speed * 15f)),
                    5 => new Vector2(player.velocity.X, -(speed * 10f)),
                    _ => new Vector2(player.velocity.X, -(speed * 12.5f)),
                };
            }
            else if (player.controlDown)
            {
                if (!player.controlLeft && !player.controlRight && !p.backStab && !player.GetModPlayer<Items.Demoman.ShieldPlayer>().chargeActive)
                    player.velocity.X = 0f;
                player.maxFallSpeed = p.currentClass switch
                {
                    1 => speed * 15f,
                    5 => speed * 10f,
                    _ => speed * 12.5f,
                };
                player.velocity = p.currentClass switch
                {
                    1 => new Vector2(player.velocity.X, speed * 15f),
                    5 => new Vector2(player.velocity.X, speed * 10f),
                    _ => new Vector2(player.velocity.X, speed * 12.5f),
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
                    _ => new Vector2(-(speed * 12.5f), player.velocity.Y),
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
                    _ => new Vector2(speed * 12.5f, player.velocity.Y),
                };
            }
            if (!player.controlUp && !player.controlDown && !player.controlLeft && !player.controlRight! && !p.backStab && !player.GetModPlayer<Items.Demoman.ShieldPlayer>().chargeActive)
                idle = true;
            else
                idle = false;
        }

        public override void SetMount(Player player, ref bool skipDust) => skipDust = true;

        public override void Dismount(Player player, ref bool skipDust) => player.controlMount = false;

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow) => false;
    }

    public class TF2MountItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mann's Anti Danmaku System");
            Tooltip.SetDefault("Dodge danmaku like you dodge child support!\n"
                + "Jump to focus, slowing movement, increasing defense, and enabling homing shots for some weapons.");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            List<string> currentHomingPowerKey = KeybindSystem.HomingPower.GetAssignedKeys(0);
            if (currentHomingPowerKey.Count <= 0 || currentHomingPowerKey.Contains("None"))
            {
                var line = new TooltipLine(Mod, "Homing Attribute",
                "Assign keybind to change homing power");
                if (Main.hardMode)
                    tooltips.Add(line);
            }
            else
            {
                var line = new TooltipLine(Mod, "Homing Attribute",
                "Press " + currentHomingPowerKey[0] + " to change homing power");
                if (Main.hardMode)
                    tooltips.Add(line);
            }

            List<string> currentMountSpeedKey = KeybindSystem.MountSpeed.GetAssignedKeys(0);
            if (currentMountSpeedKey.Count <= 0 || currentMountSpeedKey.Contains("None"))
            {
                var line2 = new TooltipLine(Mod, "Speed Attributes",
                    "Assign keybind to change to change speed");
                if (NPC.downedMoonlord)
                    tooltips.Add(line2);
            }
            else
            {
                var line2 = new TooltipLine(Mod, "Speed Attributes",
                    "Press " + currentMountSpeedKey[0] + " to change speed");
                if (NPC.downedMoonlord)
                    tooltips.Add(line2);
            }
        }

        public override void SetDefaults()
        {
            Item.width = 1000;
            Item.height = 1000;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing; // how the player's arm moves when using the item
            Item.rare = ItemRarityID.Master;
            Item.value = Item.sellPrice(gold: 1);
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/horror"); // What sound should play when using the item
            Item.noMelee = true; // this item doesn't do any melee damage
            Item.noUseGraphic = true;
            Item.mountType = ModContent.MountType<TF2Mount>();
        }
    }

    public class TF2MountBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mann's Anti Danmaku System");
            Description.SetDefault("Nice try, pal!");
            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<TF2Mount>(), player);
            player.buffTime[buffIndex] = 10; // reset buff time
        }
    }

    public class FocusMode : PlayerDrawLayer
    {
        private Asset<Texture2D> focusspeedTexture;

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
            // The following code draws ExampleItem's texture behind the player's head.

            if (focusspeedTexture == null)
                focusspeedTexture = ModContent.Request<Texture2D>("TF2/Content/Mounts/Focus");

            var position = drawInfo.Center - Main.screenPosition;
            position = new Vector2((int)position.X, (int)position.Y); // You'll sometimes want to do this, to avoid quivering.

            // Queues a drawing of a sprite. Do not use SpriteBatch in drawlayers!
            drawInfo.DrawDataCache.Add(new DrawData(
                focusspeedTexture.Value, // The texture to render.
                position, // Position to render at.
                null, // Source rectangle.
                Color.White, // Color.
                0f, // Rotation.
                focusspeedTexture.Size() * 0.5f, // Origin. Uses the texture's center.
                1f, // Scale.
                SpriteEffects.None, // SpriteEffects.
                0 // 'Layer'. This is always 0 in Terraria.
            ));
        }
    }
}