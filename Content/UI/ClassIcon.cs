using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;

namespace TF2.Content.UI
{
    internal class ClassIcon : UIElement
    {
        private Asset<Texture2D> texture;
        private readonly Player playerReference;

        public ClassIcon(Player player)
        {
            playerReference = player;
            player = player.SerializedClone();
            player.dead = false;
            player.PlayerFrame();
            Width.Set(60f, 0f);
            Height.Set(60f, 0f);
            IgnoresMouseInteraction = true;
        }

        public override void Update(GameTime gameTime)
        {
            playerReference.ResetEffects();
            playerReference.ResetVisibleAccessories();
            playerReference.UpdateMiscCounter();
            playerReference.UpdateDyes();
            playerReference.PlayerFrame();
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            int icon = playerReference.GetModPlayer<TF2Player>().currentClass;
            texture = ModContent.Request<Texture2D>(icon != 0 ? $"TF2/Content/Textures/{(TF2Player.ClassName)icon}Icon" : "Images/UI/PlayerBackground", AssetRequestMode.ImmediateLoad);
            CalculatedStyle dimensions = GetDimensions();
            spriteBatch.Draw(texture.Value, dimensions.Position(), Color.White);
        }
    }
}