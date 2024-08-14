using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using TF2.Content.Items.Weapons;

namespace TF2.Content.UI.HUD.Sniper
{
    [Autoload(Side = ModSide.Client)]
    internal class SniperRifleAmmoHUD : HeavyAmmoHUD
    {
        protected override bool CanDisplay => Player.HeldItem.ModItem is TF2Weapon && Player.inventory[58].ModItem != Weapon && Weapon.equipped && Weapon.GetWeaponMechanic("Sniper Rifle") && Weapon.maxAmmoReserve <= 0;

        protected override string Texture => "TF2/Content/Textures/UI/HUD/AmmoChargeMeterHUD";

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
}