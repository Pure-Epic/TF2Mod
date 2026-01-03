using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.Items.Weapons;
using TF2.Content.Items.Weapons.Scout;

namespace TF2.Content.UI.HUD.Scout
{
    [Autoload(Side = ModSide.Client)]
    internal class BonkAtomicPunchAmmoHUD : SingleAmmoHUD
    {
        protected override bool CanDisplay => Player.HeldItem.ModItem is TF2Weapon weapon && (Player.HeldItem.ModItem is BonkAtomicPunch || Player.HeldItem.ModItem is CritaCola) && Player.inventory[58].ModItem != weapon && weapon.equipped;

        protected override int RechargeTime => TF2.Time(22);
    }

    [Autoload(Side = ModSide.Client)]
    internal class BonkAtomicPunchChargeMeterHUD : TF2HUD
    {
        protected override bool CanDisplay => TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<BonkAtomicPunch>())
            || TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<CritaCola>());

        protected override Asset<Texture2D> Texture => HUDTextures.MiddleChargeHUDTexture;

        protected static bool HasMelee => TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<Sandman>());

        private UIText drink;

        protected override void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement
            {
                Left = StyleDimension.FromPixelsAndPercent(-578f, 1f),
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
            drink = new UIText(TF2HUDSystem.TF2HUDLocalization[2], 0.5f)
            {
                HAlign = 0.5f,
                VAlign = 0.75f,
                IgnoresMouseInteraction = true
            };
        }

        protected override void HUDPostInitialize(UIElement area) => area.Append(drink);

        protected override void HUDDraw(SpriteBatch spriteBatch)
        {
            Rectangle hitbox = area.GetInnerDimensions().ToRectangle();
            hitbox.X += 8;
            hitbox.Y += 26;
            hitbox.Width = 46;
            hitbox.Height = 6;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.Left, hitbox.Y, TF2.Round(hitbox.Width * (TF2.GetItemInHotbar(Player, [ModContent.ItemType<BonkAtomicPunch>(), ModContent.ItemType<CritaCola>()]).ModItem as TF2Weapon).timer[0] / (float)TF2.Time(22)), hitbox.Height), Color.White);
        }

        protected override void HUDUpdate(GameTime gameTime) => area.Left = StyleDimension.FromPixelsAndPercent(-578f - (HasMelee ? 65f : 0f), 1f);
    }
}