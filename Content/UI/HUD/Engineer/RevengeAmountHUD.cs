using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.Items.Weapons.Engineer;

namespace TF2.Content.UI.HUD.Engineer
{
    [Autoload(Side = ModSide.Client)]
    internal class RevengeAmountHUD : TF2HUD
    {
        protected override bool CanDisplay => TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<FrontierJustice>());

        protected override string Texture => "TF2/Content/Textures/UI/HUD/CounterHUD";

        private UIText revenge;
        private UIText amount;

        protected override void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement
            {
                Left = StyleDimension.FromPixelsAndPercent(-592f, 1f),
                Top = StyleDimension.FromPixelsAndPercent(145f, 0f),
                Width = StyleDimension.FromPixels(84f),
                Height = StyleDimension.FromPixels(56f),
                IgnoresMouseInteraction = true
            };
            _texture = new UIImage(ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value)
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPercent(1f),
                IgnoresMouseInteraction = true
            };
            UIText _amount = new UIText("", 0.8f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.375f,
                IgnoresMouseInteraction = true
            };
            amount = _amount;
            UIText _revenge = new UIText(TF2HUDSystem.TF2HUDLocalization[6], 0.5f)
            {
                HAlign = 0.5f,
                VAlign = 0.85f,
                IgnoresMouseInteraction = true
            };
            revenge = _revenge;
        }

        protected override void HUDPostInitialize(UIElement area)
        {
            area.Append(amount);
            area.Append(revenge);
        }

        protected override void HUDUpdate(GameTime gameTime) => amount.SetText(Player.GetModPlayer<FrontierJusticePlayer>().revenge.ToString());
    }
}