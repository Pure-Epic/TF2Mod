﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.Items.Weapons.Sniper;

namespace TF2.Content.UI.HUD.Sniper
{
    [Autoload(Side = ModSide.Client)]
    internal class RazorbackChargeMeterHUD : TF2HUD
    {
        protected override bool CanDisplay => Player.GetModPlayer<RazorbackPlayer>().razorbackEquipped;

        protected override Asset<Texture2D> Texture => HUDTextures.RightChargeHUDTexture;

        private UIText razorback;

        protected override void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement
            {
                Left = StyleDimension.FromPixelsAndPercent(-578f, 1f),
                Top = StyleDimension.FromPixelsAndPercent(76f, 0f),
                Width = StyleDimension.FromPixels(68f),
                Height = StyleDimension.FromPixels(54f),
                IgnoresMouseInteraction = true
            };
            _texture = new UIImage(Texture)
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPercent(1f),
                IgnoresMouseInteraction = true
            };
            razorback = new UIText(TF2HUDSystem.TF2HUDLocalization[12], 0.5f)
            {
                HAlign = 0.5f,
                VAlign = 0.75f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(razorback);

        protected override void HUDDraw(SpriteBatch spriteBatch)
        {
            RazorbackPlayer razorbackPlayer = Player.GetModPlayer<RazorbackPlayer>();
            Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
            hitbox.X += 12;
            hitbox.Y += 24;
            hitbox.Width = 44;
            hitbox.Height = 6;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * razorbackPlayer.timer / (float)TF2.Time(30)), hitbox.Height), Color.White);
        }
    }
}