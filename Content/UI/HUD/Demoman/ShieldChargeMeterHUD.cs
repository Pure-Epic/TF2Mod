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
        protected override bool CanDisplay => Player.GetModPlayer<TF2Player>().hasShield;

        protected override string Texture => "TF2/Content/Textures/UI/HUD/ShieldChargeMeterHUD";

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
            _texture = new UIImage(ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value)
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPercent(1f),
                IgnoresMouseInteraction = true
            };
            UIText _charge = new UIText(TF2HUDSystem.TF2HUDLocalization[3], 0.5f)
            {
                HAlign = 0.5f,
                VAlign = 0.85f,
                IgnoresMouseInteraction = true
            };
            charge = _charge;
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(charge);

        protected override void HUDDraw(SpriteBatch spriteBatch)
        {
            ShieldPlayer p = Player.GetModPlayer<TF2Player>().shieldType switch
            {
                1 => Player.GetModPlayer<CharginTargePlayer>(),
                _ => Player.GetModPlayer<CharginTargePlayer>(),
            };
            Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
            hitbox.X += 10;
            hitbox.Y += 22;
            hitbox.Width = 62;
            hitbox.Height = 8;
            int left = hitbox.Left;
            float charge;
            if (!p.chargeActive)
            {
                charge = (float)p.timer / p.shieldRechargeTime;
                charge = Utils.Clamp(charge, 0f, 1f);
            }
            else
            {
                charge = (float)p.chargeLeft / p.chargeDuration;
                charge = Utils.Clamp(charge, 0f, 1f);
            }
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left, hitbox.Y, TF2.Round(hitbox.Width * charge), hitbox.Height), !p.Player.HasBuff<MeleeCrit>() ? Color.White : Color.Red);
        }

        // protected override void HUDUpdate(GameTime gameTime) => area.Top = StyleDimension.FromPixels(!TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<Eyelander>()) ? 80f : 151f);
    }
}