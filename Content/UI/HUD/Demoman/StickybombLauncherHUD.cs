using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.Items.Weapons;
using TF2.Content.Items.Weapons.Demoman;

namespace TF2.Content.UI.HUD.Demoman
{
    [Autoload(Side = ModSide.Client)]
    internal class StickybombLauncherAmmoHUD : AmmoHUD
    {
        protected override bool CanDisplay => Player.HeldItem.ModItem is TF2Weapon weapon && Player.inventory[58].ModItem != weapon && !weapon.noAmmoClip && weapon.equipped && weapon.GetWeaponMechanic("Stickybomb Launcher");

        protected override Asset<Texture2D> Texture => HUDTextures.AmmoChargeHUDTexture;

        protected override void HUDDraw(SpriteBatch spriteBatch)
        {
            TF2Weapon weapon = Player.HeldItem.ModItem as TF2Weapon;
            Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
            hitbox.X += 18;
            hitbox.Y += 48;
            hitbox.Width = 84;
            hitbox.Height = 6;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * weapon.chargeTime / weapon.maxChargeTime), hitbox.Height), Color.White);
        }
    }

    [Autoload(Side = ModSide.Client)]
    internal class StickybombAmountHUD : SingleAmmoHUD
    {
        protected override bool CanDisplay => TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<StickybombLauncher>())
            || TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<ScottishResistance>())
            || TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<StickyJumper>());

        protected override Asset<Texture2D> Texture => HUDTextures.StickybombAmountHUDTexture;

        private UIText amount;

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
            amount = new UIText("", 0.8f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Left = StyleDimension.FromPixels(5f),
                TextOriginX = 0f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(amount);

        protected override void HUDUpdate(GameTime gameTime)
        {
            amount.SetText((TF2.GetItemInHotbar(Player, [ModContent.ItemType<StickybombLauncher>(), ModContent.ItemType<ScottishResistance>(), ModContent.ItemType<StickyJumper>()]).ModItem as TF2Weapon).stickybombsAmount.ToString());
            amount.Left = StyleDimension.FromPixels(5f + FontAssets.DeathText.Value.MeasureString(amount.Text).X / 2.5f);
        }
    }
}