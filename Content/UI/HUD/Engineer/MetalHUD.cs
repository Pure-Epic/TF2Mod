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

namespace TF2.Content.UI.HUD.Engineer
{
    [Autoload(Side = ModSide.Client)]
    internal class MetalHUD : TF2HUD
    {
        protected override bool CanDisplay => Player.GetModPlayer<TF2Player>().currentClass == TF2Item.Engineer;

        protected override Asset<Texture2D> Texture => HUDTextures.MetalHUDTexture;

        private UIText metal;

        protected override void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement
            {
                Left = StyleDimension.FromPixelsAndPercent(-592f, 1f),
                Top = StyleDimension.FromPixelsAndPercent(80f, 0f),
                Width = StyleDimension.FromPixels(84f),
                Height = StyleDimension.FromPixels(50f),
                IgnoresMouseInteraction = true
            };
            _texture = new UIImage(Texture)
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPercent(1f),
                IgnoresMouseInteraction = true
            };
            metal = new UIText("", 0.6f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Left = StyleDimension.FromPixels(-15f),
                TextOriginX = 0f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(metal);

        protected override void HUDUpdate(GameTime gameTime)
        {
            metal.SetText(Player.GetModPlayer<TF2Player>().metal.ToString());
            metal.Left = StyleDimension.FromPixels(FontAssets.DeathText.Value.MeasureString(metal.Text).X / 3.33333f - 15f);
        }
    }
}