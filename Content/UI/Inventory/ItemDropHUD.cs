using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.UI.HUD;

namespace TF2.Content.UI.Inventory
{
    [Autoload(Side = ModSide.Client)]
    internal class ItemDropHUD : TF2HUD
    {
        protected override bool CanDisplay => Player.GetModPlayer<TF2Player>().ItemDropReady && Main.playerInventory;

        protected override Asset<Texture2D> Texture => HUDTextures.ItemDropIcon;

        protected override void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement
            {
                Left = StyleDimension.FromPixelsAndPercent(-184f, 1f),
                Top = StyleDimension.FromPixelsAndPercent(132f, 0f),
                Width = StyleDimension.FromPixels(40f),
                Height = StyleDimension.FromPixels(34f),
            };
            _texture = new UIImage(Texture)
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPercent(1f),
                IgnoresMouseInteraction = true
            };
            _area.OnMouseOver += Hover_ItemDropIcon;
            _area.OnLeftMouseDown += Click_ItemDropIcon;
        }

        protected override void HUDSilentUpdate(GameTime gameTime) => area.Top = StyleDimension.FromPixelsAndPercent(TF2Inventory.MapOpen ? 386f : 132f, 0f);

        private void Hover_ItemDropIcon(UIMouseEvent evt, UIElement listeningElement)
        {
            if (CanDisplay && IsMouseHovering)
                Player.mouseInterface = true;
        }

        private void Click_ItemDropIcon(UIMouseEvent evt, UIElement listeningElement)
        {
            if (CanDisplay)
            {
                Item item = new Item();
                item.SetDefaults(TF2Item.GetRandomWeapon);
                (item.ModItem as TF2Item).availability = TF2Item.Unlock;
                Player.QuickSpawnItem(Player.GetSource_GiftOrReward(), item);
                Player.GetModPlayer<TF2Player>().itemDropTime = 0;
            }
        }
    }
}