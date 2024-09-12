using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace TF2.Content.UI.HUD
{
    [Autoload(Side = ModSide.Client)]
    public abstract class SingleAmmoHUD : AmmoHUD
    {
        protected virtual int RechargeTime => 0;

        protected override void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement
            {
                Left = StyleDimension.FromPixelsAndPercent(-500f, 1f),
                Top = StyleDimension.FromPixelsAndPercent(70f, 0f),
                Width = StyleDimension.FromPixels(120f),
                Height = StyleDimension.FromPixels(60f),
                IgnoresMouseInteraction = true
            };
            _texture = new UIImage(Texture)
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPercent(1f),
                IgnoresMouseInteraction = true
            };
            currentAmmo = new UIText("", 1f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Left = StyleDimension.FromPixels(42.5f),
                Top = StyleDimension.FromPixels(-1f),
                TextOriginX = 0f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(currentAmmo);

        protected override void HUDUpdate(GameTime gameTime)
        {
            currentAmmo.SetText(((Weapon.timer[0] >= RechargeTime) ? 1 : 0).ToString());
            currentAmmo.Left = StyleDimension.FromPixels(42.5f - FontAssets.DeathText.Value.MeasureString(currentAmmo.Text).X / 2);
        }
    }
}