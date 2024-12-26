using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.Items.Weapons.Scout;

namespace TF2.Content.UI.HUD.Scout
{
    [Autoload(Side = ModSide.Client)]
    internal class SodaPopperHUD : TF2HUD
    {
        protected override bool CanDisplay => Player.GetModPlayer<SodaPopperPlayer>().sodaPopperEquipped;

        protected override Asset<Texture2D> Texture => HUDTextures.LeftChargeHUDTexture;

        protected static bool HasSecondary => TF2.IsItemTypeInHotbar(Player, [ModContent.ItemType<BonkAtomicPunch>(), ModContent.ItemType<MadMilk>()]);

        protected static bool HasMelee => TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<Sandman>());

        private UIText hype;

        protected override void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement
            {
                Left = StyleDimension.FromPixelsAndPercent(-578f, 1f),
                Top = StyleDimension.FromPixelsAndPercent(74f, 0f),
                Width = StyleDimension.FromPixels(68f),
                Height = StyleDimension.FromPixels(56f),
                IgnoresMouseInteraction = true
            };
            _texture = new UIImage(Texture)
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPercent(1f),
                IgnoresMouseInteraction = true
            };
            hype = new UIText(TF2HUDSystem.TF2HUDLocalization[0], 0.5f)
            {
                HAlign = 0.5f,
                VAlign = 0.75f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(hype);

        protected override void HUDDraw(SpriteBatch spriteBatch)
        {
            SodaPopperPlayer p = Player.GetModPlayer<SodaPopperPlayer>();
            Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
            hitbox.X += 12;
            hitbox.Y += 24;
            hitbox.Width = 44;
            hitbox.Height = 6;
            float charge;
            if (!p.buffActive)
            {
                charge = p.hype / 350f;
                charge = Utils.Clamp(charge, 0f, 1f);
            }
            else
            {
                charge = (float)p.buffDuration / TF2.Time(10);
                charge = Utils.Clamp(charge, 0f, 1f);
            }
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * charge), hitbox.Height), (p.hype < 350) ? Color.White : Color.Red);
        }

        protected override void HUDUpdate(GameTime gameTime) => area.Left = StyleDimension.FromPixelsAndPercent(-578f - (HasSecondary ? 65f : 0f) - (HasMelee ? 65f : 0f), 1f);
    }
}