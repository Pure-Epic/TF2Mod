using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.Items;
using TF2.Content.Items.Weapons;
using TF2.Content.Items.Weapons.Engineer;
using TF2.Content.Items.Weapons.Spy;

namespace TF2.Content.UI.HUD
{
    [Autoload(Side = ModSide.Client)]
    public class HeavyAmmoHUD : SingleAmmoHUD
    {
        protected override bool CanDisplay => Player.HeldItem.ModItem is TF2Weapon && Player.inventory[58].ModItem != Weapon && Weapon.equipped && !Weapon.IsWeaponType(TF2Item.Melee) && !Weapon.GetWeaponMechanic("Medi Gun") && !Weapon.GetWeaponMechanic("Sniper Rifle") && Weapon is not Wrangler && Weapon is not ConstructionPDA && Weapon is not DestructionPDA && Weapon is not DeadRinger && Weapon is not Sapper && Weapon.maxAmmoReserve <= 0;

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
            currentAmmo = new UIText("", 1f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Left = StyleDimension.FromPixels(42.5f),
                Top = StyleDimension.FromPixels(-1f),
                TextOriginX = 0f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(currentAmmo);

        protected override void HUDUpdate(GameTime gameTime)
        {
            currentAmmo.SetText(Weapon.currentAmmoClip.ToString());
            currentAmmo.Left = StyleDimension.FromPixels(42.5f - FontAssets.DeathText.Value.MeasureString(currentAmmo.Text).X / 2);
        }
    }
}