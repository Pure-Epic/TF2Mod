using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.Items.Weapons.Soldier;

namespace TF2.Content.UI.HUD.Soldier
{
    [Autoload(Side = ModSide.Client)]
    internal class BuffBannerChargeMeterHUD : TF2HUD
    {
        protected override bool CanDisplay => Player.GetModPlayer<TF2Player>().HasBanner;

        protected override Asset<Texture2D> Texture => HUDTextures.RightChargeHUDTexture;

        private UIText rage;

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
            rage = new UIText(TF2HUDSystem.TF2HUDLocalization[4], 0.5f)
            {
                HAlign = 0.5f,
                VAlign = 0.75f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(rage);
        
        protected override void HUDDraw(SpriteBatch spriteBatch)
        {
            BannerPlayer p = Player.GetModPlayer<TF2Player>().bannerType switch
            {
                1 => Player.GetModPlayer<BuffBannerPlayer>(),
                2 => Player.GetModPlayer<BattalionsBackupPlayer>(),
                3 => Player.GetModPlayer<ConcherorPlayer>(),
                _ => Player.GetModPlayer<BuffBannerPlayer>()
            };
            Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
            hitbox.X += 12;
            hitbox.Y += 24;
            hitbox.Width = 44;
            hitbox.Height = 6;
            float charge;
            if (!p.buffActive)
            {
                charge = (float)p.rage / p.MaxRage;
                charge = Utils.Clamp(charge, 0f, 1f);
            }
            else
            {
                charge = (float)p.buffDuration / TF2.Time(10);
                charge = Utils.Clamp(charge, 0f, 1f);
            }
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * charge), hitbox.Height), !p.buffActive && p.rage < p.MaxRage ? Color.White : Color.Red);
        }
    }
}