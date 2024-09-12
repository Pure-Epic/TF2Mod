using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.NPCs.Buildings.Dispenser;
using TF2.Content.UI.Inventory;

namespace TF2.Content.UI.HUD.Engineer
{
    [Autoload(Side = ModSide.Client)]
    internal class DispenserHUD : TF2HUD
    {
        protected override bool CanDisplay => Player.GetModPlayer<TF2Player>().currentClass == TF2Item.Engineer;

        protected override Asset<Texture2D> Texture => HUDTextures.DispenserHUD;

        private UIText level;

        protected override void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement
            {
                Left = StyleDimension.FromPixelsAndPercent(-750f, 1f),
                Top = StyleDimension.FromPixelsAndPercent(232f, 0f),
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
            int dispenserID = Player.GetModPlayer<TF2Player>().dispenserWhoAmI;
            DispenserStatistics carriedDispenser = Player.GetModPlayer<TF2Player>().carriedDispenser;
            if (dispenserID > -1 && Main.npc[dispenserID].ModNPC is TF2Dispenser dispenser && Main.npc[dispenserID].active)
            {
                Texture2D newTexture = Texture.Value;
                if (!dispenser.Initialized && dispenser is DispenserLevel1)
                    newTexture = HUDTextures.DispenserLevel1HUDInitial.Value;
                else if (dispenser is DispenserLevel1)
                    newTexture = HUDTextures.DispenserLevel1HUD.Value;
                else if (dispenser is DispenserLevel2)
                    newTexture = HUDTextures.DispenserLevel2HUD.Value;
                else if (dispenser is DispenserLevel3)
                    newTexture = HUDTextures.DispenserLevel3HUD.Value;
                spriteBatch.Draw(newTexture, new Vector2(Main.screenWidth + TF2Inventory.MapMargin, 232f), Color.White);
                Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
                if (!dispenser.Initialized && dispenser is DispenserLevel1)
                {
                    hitbox.X += 78;
                    hitbox.Y += 22;
                    hitbox.Width = 96;
                    hitbox.Height = 10;
                    int buildDuration = !dispenser.hauled ? dispenser.BuildingCooldown : dispenser.BuildingCooldownHauled;
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * (float)(buildDuration - dispenser.UpgradeCooldown) / buildDuration), hitbox.Height), Color.White);
                    hitbox = area.GetInnerDimensions().ToRectangle();
                }
                hitbox.X += 22;
                hitbox.Y += 2;
                hitbox.Width = 14;
                hitbox.Height = 52;
                int height = TF2.Round(hitbox.Height * ((float)dispenser.NPC.life / dispenser.NPC.lifeMax));
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Bottom - height, hitbox.Width, height), ((float)dispenser.NPC.life / dispenser.NPC.lifeMax) >= 0.2f ? new Color(243, 243, 187) : Color.Red);
                if (!dispenser.Initialized && dispenser is DispenserLevel1) return;
                hitbox = area.GetInnerDimensions().ToRectangle();
                hitbox.X += 106;
                hitbox.Y += dispenser is not DispenserLevel3 ? 14 : 22;
                hitbox.Width = 68;
                hitbox.Height = 10;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * (float)dispenser.ReservedMetal / dispenser.MaxReservedMetal), hitbox.Height), Color.White);
                if (dispenser is not DispenserLevel3)
                {
                    hitbox = area.GetInnerDimensions().ToRectangle();
                    hitbox.X += 106;
                    hitbox.Y += 32;
                    hitbox.Width = 68;
                    hitbox.Height = 10;
                    int metal = Utils.Clamp(dispenser.Metal, 0, 200);
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * (float)metal / 200), hitbox.Height), Color.White);
                }
            }
            else if (carriedDispenser == null)
                spriteBatch.Draw(HUDTextures.DispenserHUD.Value, new Vector2(Main.screenWidth + TF2Inventory.MapMargin, 232f), Color.White);
            else
                spriteBatch.Draw(HUDTextures.DispenserLevel1HUD.Value, new Vector2(Main.screenWidth + TF2Inventory.MapMargin, 232f), Color.White);
        }

        protected override void HUDUpdate(GameTime gameTime)
        {
            area.Left = StyleDimension.FromPixelsAndPercent(TF2Inventory.MapMargin, 1f);
            string currentLevel = "";
            int dispenserID = Player.GetModPlayer<TF2Player>().dispenserWhoAmI;
            if (dispenserID > -1 && Main.npc[dispenserID].ModNPC is TF2Dispenser dispenser && Main.npc[dispenserID].active)
            {
                if (dispenser is DispenserLevel1)
                    currentLevel = "1";
                else if (dispenser is DispenserLevel2)
                    currentLevel = "2";
                else if (dispenser is DispenserLevel3)
                    currentLevel = "3";
            }
            level.SetText(currentLevel);
        }
    }
}