using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.Items.Weapons;
using TF2.Content.Items.Weapons.Scout;

namespace TF2.Content.UI.HUD.Scout
{
    [Autoload(Side = ModSide.Client)]
    internal class SandmanAmmoHUD : SingleAmmoHUD
    {
        protected override bool CanDisplay => Player.HeldItem.ModItem is Sandman weapon && Player.inventory[58].ModItem != weapon && weapon.equipped;

        protected override int RechargeTime => TF2.Time(10);

        protected override TF2Weapon Weapon => Player.HeldItem.ModItem as Sandman;
    }

    [Autoload(Side = ModSide.Client)]
    internal class SandmanChargeMeterHUD : TF2HUD
    {
        protected override bool CanDisplay => TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<Sandman>());

        protected override string Texture => "TF2/Content/Textures/UI/HUD/RightChargeMeterHUD";

        private UIText ball;

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
            ball = new UIText(TF2HUDSystem.TF2HUDLocalization[3], 0.5f)
            {
                HAlign = 0.5f,
                VAlign = 0.75f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(ball);
        
        protected override void HUDDraw(SpriteBatch spriteBatch)
        {
            TF2Weapon weapon = TF2.GetItemInHotbar(Player, ModContent.ItemType<Sandman>()).ModItem as Sandman;
            Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
            hitbox.X += 12;
            hitbox.Y += 24;
            hitbox.Width = 44;
            hitbox.Height = 6;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * weapon.timer[0] / (float)TF2.Time(10)), hitbox.Height), Color.White);
        }
    }
}