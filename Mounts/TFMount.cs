using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace TF2.Mounts
{
    public class TFMount : ModMount
    {

        public override void SetStaticDefaults()
        {
            // Movement
            MountData.usesHover = true;
            MountData.jumpHeight = 10; // How high the mount can jump.
            MountData.jumpSpeed = 10f; // The rate at which the player and mount ascend towards (negative y velocity) the jump height when the jump button is presssed.
            MountData.acceleration = 10f; // The rate at which the mount speeds up.
            MountData.blockExtraJumps = false; // Determines whether or not you can use a double jump (like cloud in a bottle) while in the mount.
            MountData.heightBoost = 0; // Height between the mount and the ground
            MountData.fallDamage = 0f; // Fall damage multiplier.
            MountData.runSpeed = 10f; // The speed of the mount
            MountData.dashSpeed = 10f; // The speed the mount moves when in the state of dashing.
            MountData.flightTimeMax = 1000; // The amount of time in frames a mount can be in the state of flying.
            MountData.fatigueMax = 1000;
            MountData.totalFrames = 1;

            MountData.buff = ModContent.BuffType<TFMountBuff>(); // The ID number of the buff assigned to the mount.

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

        public override void UpdateEffects(Player player)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            switch (p.currentClass)
            {
                case 1:
                    MountData.acceleration = player.moveSpeed * 15f;
                    MountData.runSpeed = player.moveSpeed * 15f;
                    MountData.dashSpeed = player.moveSpeed * 15f;
                    break;
                case 5:
                    MountData.acceleration = player.moveSpeed * 10f;
                    MountData.runSpeed = player.moveSpeed * 10f;
                    MountData.dashSpeed = player.moveSpeed * 10f;
                    break;
                default:
                    MountData.acceleration = player.moveSpeed * 12.5f;
                    MountData.runSpeed = player.moveSpeed * 12.5f;
                    MountData.dashSpeed = player.moveSpeed * 12.5f;
                    break;
            }

            if (player.TryingToHoverUp)
            {
                player.velocity = p.currentClass switch
                {
                    1 => new Vector2(0f, player.moveSpeed * 15f),
                    5 => new Vector2(0f, player.moveSpeed * 5f),
                    _ => new Vector2(0f, player.moveSpeed * 10f),
                };
            }
            else if (player.TryingToHoverDown)
            {
                player.velocity = p.currentClass switch
                {
                    1 => new Vector2(0f, -(player.moveSpeed * 15f)),
                    5 => new Vector2(0f, -(player.moveSpeed * 5f)),
                    _ => new Vector2(0f, -(player.moveSpeed * 10f)),
                };
            }

            if (!p.backStab)
            {
                
                if (player.TryingToHoverUp && p.currentClass == 1)
                {
                    MountData.usesHover = false;
                    player.velocity = new Vector2(0f, -15f);
                }
                else if (player.TryingToHoverDown && p.currentClass == 1)
                {
                    MountData.usesHover = false;
                    player.velocity = new Vector2(0f, 15f);
                }
                else
                {
                    MountData.usesHover = true;
                    player.velocity = new Vector2(0f, 0f);
                }
            }
            else
            {
                MountData.usesHover = false;
                if (player.TryingToHoverUp)
                {
                    player.velocity = new Vector2(0f, -25f);
                }
                if (player.TryingToHoverDown)
                {
                    player.velocity = new Vector2(0f, 25f);
                }
            }
        }

        public override void SetMount(Player player, ref bool skipDust)
        {
            skipDust = true;
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            // by returning true, the regular drawing will still happen.
            return false;
        }
    }

    public class TFMountItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mann's Anti Danmaku System");
            Tooltip.SetDefault("Dodge danmaku like you dodge child support!");
        }

        public override void SetDefaults()
        {
            Item.width = 1000;
            Item.height = 1000;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing; // how the player's arm moves when using the item
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(gold: 1);
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/horror"); // What sound should play when using the item
            Item.noMelee = true; // this item doesn't do any melee damage
            Item.noUseGraphic = true;
            Item.mountType = ModContent.MountType<TFMount>();
        }
    }

    public class TFMountBuff : ModBuff
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
            player.mount.SetMount(ModContent.MountType<Mounts.TFMount>(), player);
            player.buffTime[buffIndex] = 10; // reset buff time
        }
    }
}