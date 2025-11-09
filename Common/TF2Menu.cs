using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.UI;

namespace TF2.Common
{
    public class TF2Menu : ModMenu
    {
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("TF2/Content/Textures/TF2Mod");

        public override int Music
        {
            get
            {
                if (!(TF2.ScreamFortress || TF2.Smissmas))
                    return MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/gamestartup");
                else if (TF2.ScreamFortress)
                    return MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/gamestartup_halloween");
                else
                    return MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/gamestartup_smissmas");
            }
        }

        internal static Texture2D Background
        {
            get
            {
                if (!(TF2.ScreamFortress || TF2.Smissmas))
                    return UITextures.MainMenuBackgroundTextures[0].Value;
                else if (TF2.ScreamFortress)
                    return UITextures.MainMenuBackgroundTextures[1].Value;
                else
                    return UITextures.MainMenuBackgroundTextures[2].Value;
            }
        }

        public override string DisplayName => "TF2Mod";

        public static int classSelected;

        public override void Load() => classSelected = Main.rand.Next(1, 10);

        public override void Unload() => classSelected = 0;

        public override void OnSelected() => classSelected = Main.rand.Next(1, 10);

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            // Made with help from the Stars Above Mod
            // Take notes, Example Mod!
            Main.time = TF2.Minute(7.5);
            Main.dayTime = true;
            Main.raining = false;
            logoRotation = 0f;
            logoScale = 0.75f;
            drawColor = Color.White;
            float width = (float)Main.screenWidth / Background.Width;
            float height = (float)Main.screenHeight / Background.Height;
            Vector2 center = Vector2.Zero;
            if (height > width)
            {
                width = height;
                center.X -= (Background.Width * width - Main.screenWidth) * 0.5f;
            }
            else
                center.Y -= (Background.Height * width - Main.screenHeight) * 0.5f;
            spriteBatch.Draw(Background, center, null, Color.White, 0f, Vector2.Zero, width, SpriteEffects.None, 0f);
            DrawClass(classSelected, spriteBatch, center, width);
            return true;
        }

        private static void DrawClass(int mercenary, SpriteBatch spriteBatch, Vector2 position, float scale)
        {
            if (mercenary != 0)
                spriteBatch.Draw(UITextures.MainMenuMercenaryTextures[mercenary - 1].Value, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}