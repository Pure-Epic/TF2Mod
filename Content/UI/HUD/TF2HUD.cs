using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace TF2.Content.UI.HUD
{
    [Autoload(Side = ModSide.Client)]
    public abstract class TF2HUD : UIState
    {
        protected static Player Player => Main.LocalPlayer;

        protected virtual bool CanDisplay => false;

        protected virtual string Texture => "TF2/Content/Textures/Nothing";

        protected UIElement area;
        protected UIImage texture;
        private bool updating;

        protected virtual void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement();
            _texture = new UIImage(ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value);
        }

        protected virtual void HUDPostInitialize(UIElement area)
        { }

        protected virtual void HUDDraw(SpriteBatch spriteBatch)
        { }

        protected virtual void HUDUpdate(GameTime gameTime)
        { }

        public sealed override void OnInitialize()
        {
            Width = StyleDimension.Fill;
            Height = StyleDimension.Fill;
            HUDPreInitialize(out UIElement _area, out UIImage _texture);
            area = _area;
            texture = _texture;
            if (texture != null)
                area.Append(texture);
            Append(area);
            HUDPostInitialize(area);
        }

        public sealed override void Draw(SpriteBatch spriteBatch)
        {
            if (CanDisplay && updating)
                base.Draw(spriteBatch);
        }

        protected sealed override void DrawSelf(SpriteBatch spriteBatch)
        {
            HUDDraw(spriteBatch);
            base.DrawSelf(spriteBatch);
        }

        public sealed override void Update(GameTime gameTime)
        {
            if (CanDisplay)
            {
                updating = true;
                HUDUpdate(gameTime);
                base.Update(gameTime);
            }
        }
    }
}