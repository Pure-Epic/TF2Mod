using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.Items.Soldier;

namespace TF2.Content.UI
{
    [Autoload(Side = ModSide.Client)]
    internal class BuffBannerUI : UIState
    {
        // For this bar we'll be using a frame texture and then a gradient inside bar, as it's one of the more simpler approaches while still looking decent.
        // Once this is all set up make sure to go and do the required stuff for most UI's in the Mod class.
        private UIText text;

        private UIElement area;
        private UIImage barFrame;

        public override void OnInitialize()
        {
            // Create a UIElement for all the elements to sit on top of, this simplifies the numbers as nested elements can be positioned relative to the top left corner of this element.
            // UIElement is invisible and has no padding. You can use a UIPanel if you wish for a background.
            area = new UIElement();
            area.Left.Set(-area.Width.Pixels - 600, 1f); // Place the resource bar to the left of the hearts.
            area.Top.Set(75, 0f); // Placing it just a bit below the top of the screen.
            area.Width.Set(182, 0f); // We will be placing the following 2 UIElements within this 182x60 area.
            area.Height.Set(60, 0f);

            barFrame = new UIImage(ModContent.Request<Texture2D>($"TF2/Content/UI/AmmoFrame", AssetRequestMode.ImmediateLoad).Value);
            barFrame.Left.Set(22, 0f);
            barFrame.Top.Set(15, 0f);
            barFrame.Width.Set(138, 0f);
            barFrame.Height.Set(34, 0f);

            text = new UIText("Rage", 0.8f); // Text to show current value
            text.Width.Set(138, 0f);
            text.Height.Set(34, 0f);
            text.Top.Set(55, 0f);
            text.Left.Set(-20, 0f);

            area.Append(text);
            area.Append(barFrame);
            Append(area);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            TF2Player modPlayer = Main.LocalPlayer.GetModPlayer<TF2Player>();
            if (!modPlayer.hasBanner) return;
            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            BannerPlayer modPlayer = Main.LocalPlayer.GetModPlayer<TF2Player>().bannerType switch
            {
                1 => Main.LocalPlayer.GetModPlayer<BuffBannerPlayer>(),
                2 => Main.LocalPlayer.GetModPlayer<BattalionsBackupPlayer>(),
                3 => Main.LocalPlayer.GetModPlayer<ConcherorPlayer>(),
                _ => Main.LocalPlayer.GetModPlayer<BuffBannerPlayer>()
            };

            float quotient;
            if (!modPlayer.buffActive)
            {
                quotient = (float)modPlayer.rage / modPlayer.maxRage;
                quotient = Utils.Clamp(quotient, 0f, 1f);
            }
            else
            {
                quotient = (float)modPlayer.buffDuration / 600;
                quotient = Utils.Clamp(quotient, 0f, 1f);
            }

            Rectangle hitbox = barFrame.GetInnerDimensions().ToRectangle();
            hitbox.X += 12;
            hitbox.Width -= 24;
            hitbox.Y += 8;
            hitbox.Height -= 16;

            // Now, using this hitbox, we draw a gradient by drawing vertical lines while slowly interpolating between the 2 colors.
            int left = hitbox.Left;
            int right = hitbox.Right;
            int steps = (int)((right - left) * quotient);
            for (int i = 0; i < steps; i++)
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y, 1, hitbox.Height), Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            TF2Player modPlayer = Main.LocalPlayer.GetModPlayer<TF2Player>();
            if (!modPlayer.hasBanner)
                return;

            // Setting the text per tick to update and show our resource values.
            text.SetText($"Rage");
            base.Update(gameTime);
        }
    }
}