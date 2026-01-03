using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.Items.Weapons;

namespace TF2.Content.UI.HUD.Spy
{
    [Autoload(Side = ModSide.Client)]
    internal class KnifeHUD : TF2HUD
    {
        protected override bool CanDisplay => Player.HeldItem.ModItem is TF2Weapon weapon && Player.inventory[58].ModItem != weapon && weapon.equipped && weapon.GetWeaponMechanic("Knife");

        protected override Asset<Texture2D> Texture => HUDTextures.KnifeHUDTexture;

        private UIImage hudTexture;
        private float alpha;

        protected override void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement
            {
                Width = StyleDimension.FromPixels(42f),
                Height = StyleDimension.FromPixels(16f),
                HAlign = 0.5f,
                VAlign = 0.575f,
                IgnoresMouseInteraction = true,
            };
            _texture = new UIImage(Texture)
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPercent(1f),
                IgnoresMouseInteraction = true
            };
            hudTexture = _texture;
        }

        protected override void HUDSilentUpdate(GameTime gameTime)
        {
            if (!CanDisplay)
                alpha = 0f;
        }

        protected override void HUDDraw(SpriteBatch spriteBatch)
        {
            TF2Weapon weapon = Player.HeldItem.ModItem as TF2Weapon;
            alpha += weapon.deployTimer >= weapon.deploySpeed && !Player.ItemAnimationActive && weapon.KnifeCanBackstab(Player) ? 0.05f : -0.05f;
            alpha = Utils.Clamp(alpha, 0f, 1f);
            hudTexture.Color = Color.White * alpha;
            Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
            hitbox.X += 6;
            hitbox.Y += 6;
            hitbox.Width = 30;
            hitbox.Height = 4;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * weapon.chargeTime / weapon.maxChargeTime), hitbox.Height), Color.White * alpha);
        }
    }
}