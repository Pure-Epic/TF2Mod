using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.Items.Weapons;
using TF2.Content.Items.Weapons.Heavy;

namespace TF2.Content.UI.HUD.Heavy
{
    [Autoload(Side = ModSide.Client)]
    internal class SandvichAmmoHUD : SingleAmmoHUD
    {
        protected override bool CanDisplay => (Player.HeldItem.ModItem is Sandvich || Player.HeldItem.ModItem is DalokohsBar || Player.HeldItem.ModItem is BuffaloSteakSandvich) && Player.inventory[58].ModItem != Weapon && Weapon.equipped;

        protected override int RechargeTime => TF2.Time(!(Weapon.Type == ModContent.ItemType<DalokohsBar>()) ? 30 : 10);
    }

    [Autoload(Side = ModSide.Client)]
    internal class SandvichChargeMeterHUD : TF2HUD
    {
        protected override bool CanDisplay => TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<Sandvich>())
            || TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<DalokohsBar>())
            || TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<BuffaloSteakSandvich>());

        protected override string Texture => "TF2/Content/Textures/UI/HUD/RightChargeMeterHUD";

        private UIText food;

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
            UIText _food = new UIText(TF2HUDSystem.TF2HUDLocalization[5], 0.5f)
            {
                HAlign = 0.5f,
                VAlign = 0.75f,
                IgnoresMouseInteraction = true
            };
            food = _food;
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(food);
        
        protected override void HUDDraw(SpriteBatch spriteBatch)
        {
            TF2Weapon weapon = TF2.GetItemInHotbar(Player, new int[] { ModContent.ItemType<Sandvich>(), ModContent.ItemType<DalokohsBar>(), ModContent.ItemType<BuffaloSteakSandvich>() }).ModItem as TF2Weapon;
            Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
            hitbox.X += 12;
            hitbox.Y += 24;
            hitbox.Width = 44;
            hitbox.Height = 6;
            int left = hitbox.Left;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left, hitbox.Y, TF2.Round(hitbox.Width * weapon.timer[0] / (float)TF2.Time(!(weapon.Type == ModContent.ItemType<DalokohsBar>()) ? 30 : 10)), hitbox.Height), Color.White);
        }
    }
}