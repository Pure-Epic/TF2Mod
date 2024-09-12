using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.Items.Weapons;
using TF2.Content.Items.Weapons.Medic;

namespace TF2.Content.UI.HUD.Medic
{
    [Autoload(Side = ModSide.Client)]
    internal class OrganAmountHUD : TF2HUD
    {
        protected override bool CanDisplay => TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<VitaSaw>());

        protected override Asset<Texture2D> Texture => HUDTextures.OrganCounterHUDTexture;

        private UIText organ;
        private UIText amount;

        protected override void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement
            {
                Left = StyleDimension.FromPixelsAndPercent(-608f, 1f),
                Top = StyleDimension.FromPixelsAndPercent(74f, 0f),
                Width = StyleDimension.FromPixels(68f),
                Height = StyleDimension.FromPixels(56f),
                IgnoresMouseInteraction = true
            };
            _texture = new UIImage(Texture)
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPercent(1f),
                IgnoresMouseInteraction = true
            };
            amount = new UIText("", 0.8f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.375f,
                IgnoresMouseInteraction = true
            };
            organ = new UIText(TF2HUDSystem.TF2HUDLocalization[10], 0.5f)
            {
                HAlign = 0.5f,
                VAlign = 0.85f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area)
        {
            area.Append(amount);
            area.Append(organ);
        }

        protected override void HUDUpdate(GameTime gameTime)
        {
            amount.SetText(Player.GetModPlayer<TF2Player>().organs.ToString());
            area.Left = StyleDimension.FromPixelsAndPercent(-608f + ((Player.HeldItem.ModItem is TF2Weapon weapon && weapon.IsWeaponType(TF2Item.Primary)) ? 30f : 0f), 1f);
        }
    }
}