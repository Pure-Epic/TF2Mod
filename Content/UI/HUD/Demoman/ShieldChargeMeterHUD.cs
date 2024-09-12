using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Weapons.Demoman;

namespace TF2.Content.UI.HUD.Demoman
{
    [Autoload(Side = ModSide.Client)]
    internal class ShieldChargeMeterHUD : TF2HUD
    {
        protected override bool CanDisplay => Player.GetModPlayer<TF2Player>().HasShield;

        protected override Asset<Texture2D> Texture => HUDTextures.ShieldChargeHUDTexture;

        private UIText charge;

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
            charge = new UIText(TF2HUDSystem.TF2HUDLocalization[5], 0.5f)
            {
                HAlign = 0.5f,
                VAlign = 0.85f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(charge);

        protected override void HUDDraw(SpriteBatch spriteBatch)
        {
            ShieldPlayer p = ShieldPlayer.GetShield(Player);
            Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
            hitbox.X += 10;
            hitbox.Y += 22;
            hitbox.Width = 62;
            hitbox.Height = 8;
            float charge;
            if (!p.chargeActive)
            {
                charge = (float)p.timer / p.ShieldRechargeTime;
                charge = Utils.Clamp(charge, 0f, 1f);
            }
            else
            {
                charge = (float)p.chargeLeft / p.chargeDuration;
                charge = Utils.Clamp(charge, 0f, 1f);
            }
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * charge), hitbox.Height), !p.Player.HasBuff<MeleeCrit>() ? Color.White : Color.Red);
        }
    }
}