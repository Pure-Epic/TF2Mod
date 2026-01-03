using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using TF2.Content.Items.Weapons;

namespace TF2.Content.UI.HUD.Sniper
{
    [Autoload(Side = ModSide.Client)]
    internal class SniperRifleAmmoHUD : HeavyAmmoHUD
    {
        protected override bool CanDisplay => Player.HeldItem.ModItem is TF2Weapon weapon && Player.inventory[58].ModItem != weapon && weapon.equipped && weapon.GetWeaponMechanic("Sniper Rifle") && weapon.maxAmmoReserve <= 0;

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
}