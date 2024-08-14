using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.NPCs.Buildings.Teleporter;
using TF2.Content.UI.Inventory;

namespace TF2.Content.UI.HUD.Engineer
{
    [Autoload(Side = ModSide.Client)]
    internal class TeleporterEntranceHUD : TF2HUD
    {
        protected override bool CanDisplay => Player.GetModPlayer<TF2Player>().currentClass == TF2Item.Engineer;

        protected override string Texture => "TF2/Content/Textures/UI/HUD/TeleporterEntranceHUD";

        private UIText level;

        protected override void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement
            {
                Left = StyleDimension.FromPixelsAndPercent(-750f, 1f),
                Top = StyleDimension.FromPixelsAndPercent(291f, 0f),
                Width = StyleDimension.FromPixels(180f),
                Height = StyleDimension.FromPixels(56f),
                IgnoresMouseInteraction = true
            };
            _texture = null;
            level = new UIText("", 0.5f)
            {
                HAlign = 0.95f,
                VAlign = 0.1f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(level);

        protected override void HUDDraw(SpriteBatch spriteBatch)
        {
            int teleporterID = Player.GetModPlayer<TF2Player>().teleporterEntranceWhoAmI;
            TeleporterStatistics carriedTeleporter = Player.GetModPlayer<TF2Player>().carriedTeleporter;
            if (teleporterID > -1 && Main.npc[teleporterID].ModNPC is TeleporterEntrance entrance && Main.npc[teleporterID].active)
            {
                Texture2D newTexture = HUDTextures.TeleporterEntranceHUD;
                if (!entrance.Initialized && entrance is TeleporterEntranceLevel1)
                    newTexture = HUDTextures.TeleporterEntranceHUDInitial;
                else if (entrance is TeleporterEntranceLevel1 || entrance is TeleporterEntranceLevel2)
                    newTexture = HUDTextures.TeleporterEntranceLevel1HUD;
                else if (entrance is TeleporterEntranceLevel3)
                    newTexture = HUDTextures.TeleporterEntranceLevel3HUD;
                spriteBatch.Draw(newTexture, new Vector2(Main.screenWidth + TF2Inventory.MapMargin, 291f), Color.White);
                Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
                if (!entrance.Initialized && entrance is TeleporterEntranceLevel1)
                {
                    hitbox.X += 80;
                    hitbox.Y += 22;
                    hitbox.Width = 94;
                    hitbox.Height = 10;
                    int buildDuration = !entrance.hauled ? entrance.BuildingCooldown : entrance.BuildingCooldownHauled;
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * (float)(buildDuration - entrance.UpgradeCooldown) / buildDuration), hitbox.Height), Color.White);
                    hitbox = area.GetInnerDimensions().ToRectangle();
                }
                hitbox.X += 22;
                hitbox.Y += 2;
                hitbox.Width = 14;
                hitbox.Height = 52;
                int left = hitbox.Left;
                int height = TF2.Round(hitbox.Height * ((float)entrance.NPC.life / entrance.NPC.lifeMax));
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left, hitbox.Bottom - height, hitbox.Width, height), ((float)entrance.NPC.life / entrance.NPC.lifeMax) >= 0.2f ? new Color(243, 243, 187) : Color.Red);
                if (!entrance.Initialized || entrance is TeleporterEntranceLevel3) return;
                hitbox = area.GetInnerDimensions().ToRectangle();
                hitbox.X += 106;
                hitbox.Y += 22;
                hitbox.Width = 68;
                hitbox.Height = 10;
                int metal = Utils.Clamp(entrance.Metal, 0, 200);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left, hitbox.Y, TF2.Round(hitbox.Width * (float)metal / 200), hitbox.Height), Color.White);
            }
            else if (carriedTeleporter == null || carriedTeleporter.Type == ModContent.NPCType<TeleporterExitLevel1>() || carriedTeleporter.Type == ModContent.NPCType<TeleporterExitLevel2>() || carriedTeleporter.Type == ModContent.NPCType<TeleporterExitLevel3>())
                spriteBatch.Draw(HUDTextures.TeleporterEntranceHUD, new Vector2(Main.screenWidth + TF2Inventory.MapMargin, 291f), Color.White);
            else if (carriedTeleporter.Type == ModContent.NPCType<TeleporterEntranceLevel1>() || carriedTeleporter.Type == ModContent.NPCType<TeleporterEntranceLevel2>() || carriedTeleporter.Type == ModContent.NPCType<TeleporterEntranceLevel3>())
                spriteBatch.Draw(HUDTextures.TeleporterEntranceLevel1HUD, new Vector2(Main.screenWidth + TF2Inventory.MapMargin, 291f), Color.White);
        }

        protected override void HUDUpdate(GameTime gameTime)
        {
            area.Left = StyleDimension.FromPixelsAndPercent(TF2Inventory.MapMargin, 1f);
            string currentLevel = "";
            int teleporterID = Player.GetModPlayer<TF2Player>().teleporterEntranceWhoAmI;
            if (teleporterID > -1 && Main.npc[teleporterID].ModNPC is TeleporterEntrance entrance && Main.npc[teleporterID].active)
            {
                if (entrance is TeleporterEntranceLevel1)
                    currentLevel = "1";
                else if (entrance is TeleporterEntranceLevel2)
                    currentLevel = "2";
                else if (entrance is TeleporterEntranceLevel3)
                    currentLevel = "3";
            }
            level.SetText(currentLevel);
        }
    }

    [Autoload(Side = ModSide.Client)]
    internal class TeleporterExitHUD : TF2HUD
    {
        protected override bool CanDisplay => Player.GetModPlayer<TF2Player>().currentClass == TF2Item.Engineer;

        protected override string Texture => "TF2/Content/Textures/UI/HUD/TeleporterExitHUD";

        private UIText level;

        protected override void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement
            {
                Left = StyleDimension.FromPixelsAndPercent(-750f, 1f),
                Top = StyleDimension.FromPixelsAndPercent(351f, 0f),
                Width = StyleDimension.FromPixels(180f),
                Height = StyleDimension.FromPixels(56f),
                IgnoresMouseInteraction = true
            };
            _texture = null;
            level = new UIText("", 0.5f)
            {
                HAlign = 0.95f,
                VAlign = 0.1f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(level);

        protected override void HUDDraw(SpriteBatch spriteBatch)
        {
            int teleporterID = Player.GetModPlayer<TF2Player>().teleporterExitWhoAmI;
            TeleporterStatistics carriedTeleporter = Player.GetModPlayer<TF2Player>().carriedTeleporter;
            if (teleporterID > -1 && Main.npc[teleporterID].ModNPC is TeleporterExit exit && Main.npc[teleporterID].active)
            {
                Texture2D newTexture = HUDTextures.TeleporterExitHUD;
                if (!exit.Initialized && exit is TeleporterExitLevel1)
                    newTexture = HUDTextures.TeleporterExitHUDInitial;
                else if (exit is TeleporterExitLevel1 || exit is TeleporterExitLevel2)
                    newTexture = HUDTextures.TeleporterExitLevel1HUD;
                else if (exit is TeleporterExitLevel3)
                    newTexture = HUDTextures.TeleporterExitLevel3HUD;
                spriteBatch.Draw(newTexture, new Vector2(Main.screenWidth + TF2Inventory.MapMargin, 351f), Color.White);
                Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
                if (!exit.Initialized && exit is TeleporterExitLevel1)
                {
                    hitbox.X += 80;
                    hitbox.Y += 22;
                    hitbox.Width = 94;
                    hitbox.Height = 10;
                    int buildDuration = !exit.hauled ? exit.BuildingCooldown : exit.BuildingCooldownHauled;
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * (float)(buildDuration - exit.UpgradeCooldown) / buildDuration), hitbox.Height), Color.White);
                    hitbox = area.GetInnerDimensions().ToRectangle();
                }
                hitbox.X += 22;
                hitbox.Y += 2;
                hitbox.Width = 14;
                hitbox.Height = 52;
                int height = TF2.Round(hitbox.Height * ((float)exit.NPC.life / exit.NPC.lifeMax));
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Bottom - height, hitbox.Width, height), ((float)exit.NPC.life / exit.NPC.lifeMax) >= 0.2f ? new Color(243, 243, 187) : Color.Red);
                if (!exit.Initialized || exit is TeleporterExitLevel3) return;
                hitbox = area.GetInnerDimensions().ToRectangle();
                hitbox.X += 106;
                hitbox.Y += 22;
                hitbox.Width = 68;
                hitbox.Height = 10;
                int metal = Utils.Clamp(exit.Metal, 0, 200);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * (float)metal / 200), hitbox.Height), Color.White);
            }
            else if (carriedTeleporter == null || carriedTeleporter.Type == ModContent.NPCType<TeleporterEntranceLevel1>() || carriedTeleporter.Type == ModContent.NPCType<TeleporterEntranceLevel2>() || carriedTeleporter.Type == ModContent.NPCType<TeleporterEntranceLevel3>())
                spriteBatch.Draw(HUDTextures.TeleporterExitHUD, new Vector2(Main.screenWidth + TF2Inventory.MapMargin, 351f), Color.White);
            else if (carriedTeleporter.Type == ModContent.NPCType<TeleporterExitLevel1>() || carriedTeleporter.Type == ModContent.NPCType<TeleporterExitLevel2>() || carriedTeleporter.Type == ModContent.NPCType<TeleporterExitLevel3>())
                spriteBatch.Draw(HUDTextures.TeleporterExitLevel1HUD, new Vector2(Main.screenWidth + TF2Inventory.MapMargin, 351f), Color.White);
        }

        protected override void HUDUpdate(GameTime gameTime)
        {
            area.Left = StyleDimension.FromPixelsAndPercent(TF2Inventory.MapMargin, 1f);
            string currentLevel = "";
            int teleporterID = Player.GetModPlayer<TF2Player>().teleporterExitWhoAmI;
            if (teleporterID > -1 && Main.npc[teleporterID].ModNPC is TeleporterExit exit && Main.npc[teleporterID].active)
            {
                if (exit is TeleporterExitLevel1)
                    currentLevel = "1";
                else if (exit is TeleporterExitLevel2)
                    currentLevel = "2";
                else if (exit is TeleporterExitLevel3)
                    currentLevel = "3";
            }
            level.SetText(currentLevel);
        }
    }
}