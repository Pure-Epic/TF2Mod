using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using TF2.Common;

namespace TF2.Content.UI
{
    internal class ClassIcon : UIElement
    {
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

        protected override void DrawSelf(SpriteBatch spriteBatch) => spriteBatch.Draw(UITextures.ClassIconTextures[playerReference.GetModPlayer<TF2Player>().currentClass].Value, GetDimensions().Position(), Color.White);
    }
}