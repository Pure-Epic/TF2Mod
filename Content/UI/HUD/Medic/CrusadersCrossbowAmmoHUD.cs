using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.Items.Weapons;
using TF2.Content.Items.Weapons.Medic;

namespace TF2.Content.UI.HUD.Medic
{
    [Autoload(Side = ModSide.Client)]
    internal class CrusadersCrossbowAmmoHUD : AmmoHUD
    {
        protected override bool CanDisplay => Player.HeldItem.ModItem == Weapon && Player.inventory[58].ModItem != Weapon && Weapon.equipped;

        protected override TF2Weapon Weapon => Player.HeldItem.ModItem as CrusadersCrossbow;

        protected override void HUDUpdate(GameTime gameTime)
        {
            currentAmmo.SetText(((Weapon.cooldownTimer >= TF2.Time(1.51)) ? 1 : 0).ToString());
            currentAmmo.Left = StyleDimension.FromPixels(13.5f - FontAssets.DeathText.Value.MeasureString(currentAmmo.Text).X / 2);
            currentAmmoReserve.SetText(Weapon.currentAmmoReserve.ToString());
            currentAmmoReserve.Left = StyleDimension.FromPixels(20f + FontAssets.DeathText.Value.MeasureString(currentAmmoReserve.Text).X / 4);
        }
    }
}