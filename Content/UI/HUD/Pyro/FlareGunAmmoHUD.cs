using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Weapons.Pyro;

namespace TF2.Content.UI.HUD.Pyro
{
    [Autoload(Side = ModSide.Client)]
    internal class FlareGunAmmoHUD : HeavyAmmoHUD
    {
        protected override bool CanDisplay => Player.HeldItem.ModItem == Weapon && Player.inventory[58].ModItem != Weapon && Weapon.equipped && (Weapon is FlareGun);
    }
}