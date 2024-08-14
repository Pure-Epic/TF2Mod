using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.NPCs.Buildings.SentryGun;
using TF2.Content.UI.Inventory;

namespace TF2.Content.UI.HUD.Engineer
{
    [Autoload(Side = ModSide.Client)]
    internal class SentryHUD : TF2HUD
    {
        protected override bool CanDisplay => Player.GetModPlayer<TF2Player>().currentClass == TF2Item.Engineer;

        protected override string Texture => "TF2/Content/Textures/UI/HUD/SentryHUD";

        private UIText level;

        protected override void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement
            {
                Left = StyleDimension.FromPixelsAndPercent(-750f, 1f),
                Top = StyleDimension.FromPixelsAndPercent(133f, 0f),
                Width = StyleDimension.FromPixels(180f),
                Height = StyleDimension.FromPixels(96f),
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
            int sentryID = Player.GetModPlayer<TF2Player>().sentryWhoAmI;
            SentryStatistics carriedSentry = Player.GetModPlayer<TF2Player>().carriedSentry;
            if (sentryID > -1 && Main.npc[sentryID].ModNPC is TF2Sentry sentry && Main.npc[sentryID].active)
            {
                Texture2D newTexture = HUDTextures.SentryHUD;
                if (!sentry.Initialized)
                {
                    if (sentry is SentryLevel1)
                        newTexture = HUDTextures.SentryLevel1HUDInitial;
                    else if (sentry is SentryLevel2)
                        newTexture = HUDTextures.SentryLevel2HUD;
                    else if (sentry is SentryLevel3)
                        newTexture = HUDTextures.SentryLevel3HUD;
                    else if (sentry is MiniSentry)
                        newTexture = HUDTextures.MiniSentryHUDInitial;
                }
                else if (sentry is SentryLevel1)
                    newTexture = HUDTextures.SentryLevel1HUD;
                else if (sentry is SentryLevel2)
                    newTexture = HUDTextures.SentryLevel2HUD;
                else if (sentry is SentryLevel3)
                    newTexture = HUDTextures.SentryLevel3HUD;
                else if (sentry is MiniSentry)
                    newTexture = HUDTextures.MiniSentryHUD;
                spriteBatch.Draw(newTexture, new Vector2(Main.screenWidth + TF2Inventory.MapMargin, 133f), Color.White);
                Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
                if (!sentry.Initialized && (sentry is SentryLevel1 || sentry is MiniSentry))
                {
                    hitbox.X += 78;
                    hitbox.Y += 42;
                    hitbox.Width = 96;
                    hitbox.Height = 10;
                    int buildDuration = !sentry.hauled ? sentry.BuildingCooldown : sentry.BuildingCooldownHauled;
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * (float)(buildDuration - sentry.UpgradeCooldown) / buildDuration), hitbox.Height), Color.White);
                    hitbox = area.GetInnerDimensions().ToRectangle();
                }
                hitbox.X += 22;
                hitbox.Y += 2;
                hitbox.Width = 14;
                hitbox.Height = 92;
                int height = TF2.Round(hitbox.Height * ((float)sentry.NPC.life / sentry.NPC.lifeMax));
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Bottom - height, hitbox.Width, height), ((float)sentry.NPC.life / sentry.NPC.lifeMax) >= 0.2f ? new Color(243, 243, 187) : Color.Red);
                if (!sentry.Initialized && (sentry is SentryLevel1 || sentry is MiniSentry)) return;
                hitbox = area.GetInnerDimensions().ToRectangle();
                hitbox.X += 106;
                hitbox.Y += sentry is not MiniSentry ? 34 : 42;
                hitbox.Width = 68;
                hitbox.Height = 10;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * ((float)sentry.Ammo / sentry.Rounds)), hitbox.Height), Color.White);
                if (sentry is not MiniSentry)
                {
                    hitbox = area.GetInnerDimensions().ToRectangle();
                    hitbox.X += 106;
                    hitbox.Y += 52;
                    hitbox.Width = 68;
                    hitbox.Height = 10;
                    int metal = Utils.Clamp(sentry.Metal, 0, 200);
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * (float)(sentry is not SentryLevel3 ? ((float)metal / 200) : ((float)sentry.RocketAmmo / 20))), hitbox.Height), Color.White);
                }
            }
            else if (carriedSentry == null)
                spriteBatch.Draw(HUDTextures.SentryHUD, new Vector2(Main.screenWidth + TF2Inventory.MapMargin, 133f), Color.White);
            else if (carriedSentry.Type == ModContent.NPCType<SentryLevel1>() || carriedSentry.Type == ModContent.NPCType<SentryLevel2>() || carriedSentry.Type == ModContent.NPCType<SentryLevel3>())
                spriteBatch.Draw(HUDTextures.SentryLevel1HUD, new Vector2(Main.screenWidth + TF2Inventory.MapMargin, 133f), Color.White);
            else if (carriedSentry.Type == ModContent.NPCType<MiniSentry>())
                spriteBatch.Draw(HUDTextures.MiniSentryHUD, new Vector2(Main.screenWidth + TF2Inventory.MapMargin, 133f), Color.White);
        }

        protected override void HUDUpdate(GameTime gameTime)
        {
            area.Left = StyleDimension.FromPixelsAndPercent(TF2Inventory.MapMargin, 1f);
            string currentLevel = "";
            int sentryID = Player.GetModPlayer<TF2Player>().sentryWhoAmI;
            if (sentryID > -1 && Main.npc[sentryID].ModNPC is TF2Sentry sentry && Main.npc[sentryID].active)
            {
                if (sentry is SentryLevel1)
                    currentLevel = "1";
                else if (sentry is SentryLevel2)
                    currentLevel = "2";
                else if (sentry is SentryLevel3)
                    currentLevel = "3";
            }
            level.SetText(currentLevel);
        }
    }
}