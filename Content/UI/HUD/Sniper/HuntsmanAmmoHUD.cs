using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.Items.Weapons;
using TF2.Content.Items.Weapons.Sniper;

namespace TF2.Content.UI.HUD.Sniper
{
    [Autoload(Side = ModSide.Client)]
    internal class HuntsmanAmmoHUD : AmmoHUD
    {
        protected override bool CanDisplay => Player.HeldItem.ModItem is TF2Weapon && Player.inventory[58].ModItem != Weapon && Weapon.equipped && Weapon.GetWeaponMechanic("Sniper Rifle") && (Weapon is Huntsman || Weapon.maxAmmoReserve > 0);

        protected override string Texture => "TF2/Content/Textures/UI/HUD/AmmoChargeMeterHUD";

        protected override void HUDDraw(SpriteBatch spriteBatch)
        {
            TF2Weapon weapon = Player.HeldItem.ModItem as TF2Weapon;
            Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
            hitbox.X += 18;
            hitbox.Y += 48;
            hitbox.Width = 84;
            hitbox.Height = 6;
            int left = hitbox.Left;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left, hitbox.Y, TF2.Round(hitbox.Width * weapon.chargeTime / weapon.maxChargeTime), hitbox.Height), Color.White);
        }

        protected override void HUDUpdate(GameTime gameTime)
        {
            currentAmmo.SetText(((Weapon.maxAmmoClip > 0) ? Weapon.currentAmmoClip : (Weapon.cooldownTimer >= TF2.Time(1.94) ? 1 : 0)).ToString());
            currentAmmo.Left = StyleDimension.FromPixels(13.5f - FontAssets.DeathText.Value.MeasureString(currentAmmo.Text).X / 2);
            currentAmmoReserve.SetText(Weapon.currentAmmoReserve.ToString());
            currentAmmoReserve.Left = StyleDimension.FromPixels(20f + FontAssets.DeathText.Value.MeasureString(currentAmmoReserve.Text).X / 4);
        }
    }
}