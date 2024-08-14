using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.Buffs;

namespace TF2.Content.UI.HUD.Spy
{
    [Autoload(Side = ModSide.Client)]
    internal class CloakAndDaggerChargeMeterHUD : TF2HUD
    {
        protected override bool CanDisplay => Player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped;

        protected override string Texture => "TF2/Content/Textures/UI/HUD/RightChargeMeterHUD";

        private UIText cloak;

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
            _texture = new UIImage(ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value)
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPercent(1f),
                IgnoresMouseInteraction = true
            };
            cloak = new UIText(TF2HUDSystem.TF2HUDLocalization[14], 0.5f)
            {
                HAlign = 0.5f,
                VAlign = 0.75f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(cloak);
        
        protected override void HUDDraw(SpriteBatch spriteBatch)
        {
            CloakAndDaggerPlayer cloakAndDaggerPlayer = Player.GetModPlayer<CloakAndDaggerPlayer>();
            Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
            hitbox.X += 12;
            hitbox.Y += 24;
            hitbox.Width = 44;
            hitbox.Height = 6;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * (float)cloakAndDaggerPlayer.cloakMeter / cloakAndDaggerPlayer.cloakMeterMax), hitbox.Height), Color.White);
        }
    }
}