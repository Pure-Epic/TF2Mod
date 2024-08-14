using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
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
    internal class UberchargeHUD : AmmoHUD
    {
        protected override bool CanDisplay => !(Player.HeldItem.ModItem is TF2Weapon weapon && weapon.IsWeaponType(TF2Item.Primary)) && TF2.IsItemTypeInHotbar(Player, [ModContent.ItemType<MediGun>(), ModContent.ItemType<Kritzkrieg>(), ModContent.ItemType<QuickFix>()]);

        protected override string Texture => "TF2/Content/Textures/UI/HUD/UberchargeHUD";

        private UIText ubercharge;

        protected override void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement
            {
                Left = StyleDimension.FromPixelsAndPercent(-530f, 1f),
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
            ubercharge = new UIText(TF2HUDSystem.TF2HUDLocalization[9], 0.8f)
            {
                HAlign = 0.5f,
                VAlign = 0.35f,
                Left = StyleDimension.FromPixels(-32.5f),
                TextOriginX = 0f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(ubercharge);

        protected override void HUDDraw(SpriteBatch spriteBatch)
        {
            TF2Weapon weapon = TF2.GetItemInHotbar(Player, [ModContent.ItemType<MediGun>(), ModContent.ItemType<Kritzkrieg>(), ModContent.ItemType<QuickFix>()]).ModItem as TF2Weapon;
            TF2Player p = Player.GetModPlayer<TF2Player>();
            Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
            hitbox.X += 26;
            hitbox.Y += 36;
            hitbox.Width = 106;
            hitbox.Height = 10;
            int left = hitbox.Left;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left, hitbox.Y, TF2.Round(hitbox.Width * (!Player.GetModPlayer<TF2Player>().activateUberCharge ? (weapon.uberCharge / weapon.uberChargeCapacity) : ((float)(p.uberChargeDuration - p.uberChargeTime) / p.uberChargeDuration))), hitbox.Height), Color.White);
        }

        protected override void HUDUpdate(GameTime gameTime)
        {
            TF2Weapon weapon = TF2.GetItemInHotbar(Player, [ModContent.ItemType<MediGun>(), ModContent.ItemType<Kritzkrieg>(), ModContent.ItemType<QuickFix>()]).ModItem as TF2Weapon;
            TF2Player p = Player.GetModPlayer<TF2Player>();
            ubercharge.SetText(TF2HUDSystem.TF2HUDLocalization[9] + ": " + (TF2.Round(100 * (!Player.GetModPlayer<TF2Player>().activateUberCharge ? (weapon.uberCharge / weapon.uberChargeCapacity) : ((float)(p.uberChargeDuration - p.uberChargeTime) / p.uberChargeDuration))) + "%").ToString());
            ubercharge.Left = StyleDimension.FromPixels((FontAssets.MouseText.Value.MeasureString(ubercharge.Text).X / 2.5f) - 32.5f);
        }
    }
}