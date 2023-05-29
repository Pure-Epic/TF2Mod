using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Gensokyo.Content.Items.Sniper;

namespace TF2.Gensokyo.Content.UI
{
    [ExtendsFromMod("Gensokyo")]
    [Autoload(Side = ModSide.Client)]
    internal class HarshPunisherChargeUI : UIState
    {
        // For this bar we'll be using a frame texture and then a gradient inside bar, as it's one of the more simpler approaches while still looking decent.
        // Once this is all set up make sure to go and do the required stuff for most UI's in the Mod class.
        private UIText text;
        private UIElement area;
        private UIImage barFrame;
        private Color gradientA;
        private Color gradientB;

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

            text = new UIText("", 0.8f); // Text to show current value
            text.Width.Set(138, 0f);
            text.Height.Set(34, 0f);
            text.Top.Set(55, 0f);
            text.Left.Set(0, 0f);

            gradientA = new Color(240, 129, 73); // Yellow Orange
            gradientB = new Color(239, 152, 73); // Lighter yellow orange

            area.Append(text);
            area.Append(barFrame);
            Append(area);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // This prevents drawing unless we are using an TF2Weapon
            if (Main.LocalPlayer.HeldItem.ModItem is not HarshPunisher)
                return;

            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            var modPlayer = Main.LocalPlayer.GetModPlayer<TF2Player>();
            // Calculate quotient
            float quotient = modPlayer.sniperCharge / modPlayer.sniperMaxCharge; // Creating a quotient that represents the difference of your currentResource vs your maximumResource, resulting in a float of 0-1f.
            quotient = Utils.Clamp(quotient, 0f, 1f); // Clamping it to 0-1f so it doesn't go over that.

            // Here we get the screen dimensions of the barFrame element, then tweak the resulting rectangle to arrive at a rectangle within the barFrame texture that we will draw the gradient. These values were measured in a drawing program.
            Rectangle hitbox = barFrame.GetInnerDimensions().ToRectangle();
            hitbox.X += 12;
            hitbox.Width -= 24;
            hitbox.Y += 8;
            hitbox.Height -= 16;

            // Now, using this hitbox, we draw a gradient by drawing vertical lines while slowly interpolating between the 2 colors.
            int left = hitbox.Left;
            int right = hitbox.Right;
            int steps = (int)((right - left) * quotient);
            for (int i = 0; i < steps; i += 1)
            {
                //float percent = (float)i / steps; // Alternate Gradient Approach
                float percent = (float)i / (right - left);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y, 1, hitbox.Height), Color.Lerp(gradientA, gradientB, percent));
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.HeldItem.ModItem is not HarshPunisher)
                return;

            var modPlayer = Main.LocalPlayer.GetModPlayer<TF2Player>();
            // Setting the text per tick to update and show our resource values.
            text.SetText($"Charge: {(int)(100 * (modPlayer.sniperCharge / modPlayer.sniperMaxCharge))}%");
            base.Update(gameTime);
        }
    }
}