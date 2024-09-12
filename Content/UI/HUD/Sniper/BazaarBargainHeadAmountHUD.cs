using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.Items.Weapons.Sniper;

namespace TF2.Content.UI.HUD.Sniper
{
    [Autoload(Side = ModSide.Client)]
    internal class BazaarBargainHeadAmountHUD : TF2HUD
    {
        protected override bool CanDisplay => TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<BazaarBargain>());

        protected override Asset<Texture2D> Texture => HUDTextures.WeaponCounterHUDTexture;

        private UIText heads;
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
            _texture = new UIImage(Texture)
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPercent(1f),
                IgnoresMouseInteraction = true
            };
            amount = new UIText("", 0.8f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.375f,
                IgnoresMouseInteraction = true
            };
            heads = new UIText(TF2HUDSystem.TF2HUDLocalization[6], 0.5f)
            {
                HAlign = 0.5f,
                VAlign = 0.85f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area)
        {
            area.Append(amount);
            area.Append(heads);
        }

        protected override void HUDUpdate(GameTime gameTime) => amount.SetText(Player.GetModPlayer<BazaarBargainPlayer>().heads.ToString());
    }
}