using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.Items.Weapons;

namespace TF2.Content.UI.HUD
{
    [Autoload(Side = ModSide.Client)]
    public class AmmoHUD : TF2HUD
    {
        protected virtual TF2Weapon Weapon => Player.HeldItem.ModItem as TF2Weapon;

        protected override bool CanDisplay => Player.HeldItem.ModItem is TF2Weapon && Player.inventory[58].ModItem != Weapon && !Weapon.noAmmoClip && Weapon.maxAmmoReserve > 0 && Weapon.equipped
            && !Weapon.GetWeaponMechanic("Stickybomb Launcher")
            && !Weapon.GetWeaponMechanic("Medi Gun")
            && !Weapon.GetWeaponMechanic("Sniper Rifle");

        protected override string Texture => "TF2/Content/Textures/UI/HUD/AmmoHUD";

        protected UIText currentAmmo;
        protected UIText currentAmmoReserve;

        protected override void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement
            {
                Left = StyleDimension.FromPixelsAndPercent(-500f, 1f),
                Top = StyleDimension.FromPixelsAndPercent(70f, 0f),
                Width = StyleDimension.FromPixels(120f),
                Height = StyleDimension.FromPixels(60f),
                IgnoresMouseInteraction = true
            };
            _texture = new UIImage(ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value)
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPercent(1f),
                IgnoresMouseInteraction = true
            };
            UIText _currentAmmo = new UIText("", 1f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Left = StyleDimension.FromPixels(13.5f),
                Top = StyleDimension.FromPixels(-1f),
                TextOriginX = 0f,
                IgnoresMouseInteraction = true
            };
            currentAmmo = _currentAmmo;
            UIText _maxAmmo = new UIText("", 0.5f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Left = StyleDimension.FromPixels(20f),
                Top = StyleDimension.FromPixels(7.5f),
                TextOriginX = 0f,
                IgnoresMouseInteraction = true
            };
            currentAmmoReserve = _maxAmmo;
        }

        protected override void HUDPostInitialize(UIElement area)
        {
            area.Append(currentAmmo);
            area.Append(currentAmmoReserve);
        }

        protected override void HUDUpdate(GameTime gameTime)
        {
            currentAmmo.SetText(Weapon.currentAmmoClip.ToString());
            currentAmmo.Left = StyleDimension.FromPixels(13.5f - FontAssets.DeathText.Value.MeasureString(currentAmmo.Text).X / 2);
            currentAmmoReserve.SetText(Weapon.currentAmmoReserve.ToString());
            currentAmmoReserve.Left = StyleDimension.FromPixels(20f + FontAssets.DeathText.Value.MeasureString(currentAmmoReserve.Text).X / 4);
        }
    }
}