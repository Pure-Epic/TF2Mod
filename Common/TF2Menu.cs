using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace TF2.Common
{
    public class TF2Menu : ModMenu
    {
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("TF2/Content/Textures/TF2");

        public override int Music
        {
            get
            {
                if (!ScreamFortress)
                    return MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/gamestartup1");               
                else
                    return MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/gamestartup_halloween");
            }
        }

        public override string DisplayName => "Team Fortress 2";

        public static bool ScreamFortress => DateTime.Today.Month == 10 || (DateTime.Today.Month == 11 && DateTime.Today.Day < 7);

        public static int classSelected;

        public override void Load() => classSelected = Main.rand.Next(1, 10);

        public override void Unload() => classSelected = 0;

        public override void OnSelected() => classSelected = Main.rand.Next(1, 10);

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            // Made with help from the Stars Above Mod
            // Take notes, Example Mod!
            Main.time = 27000;
            Main.dayTime = true;
            Main.raining = false;
            logoRotation = 0f;
            logoScale = 1f;
            Texture2D background = (Texture2D)ModContent.Request<Texture2D>("TF2/Content/Textures/" + (!ScreamFortress ? "Background" : "BackgroundScreamFortress"), AssetRequestMode.ImmediateLoad);
            float width = (float)Main.screenWidth / background.Width;
            float height = (float)Main.screenHeight / background.Height;
            Vector2 center = Vector2.Zero;
            if (height > width)
            {
                width = height;
                center.X -= (background.Width * width - Main.screenWidth) * 0.5f;
            }
            else
                center.Y -= (background.Height * width - Main.screenHeight) * 0.5f;
            spriteBatch.Draw(background, center, null, Color.White, 0f, Vector2.Zero, width, SpriteEffects.None, 0f);
            DrawClass(classSelected, spriteBatch, center, width);
            return true;
        }

        private static void DrawClass(int mercenary, SpriteBatch spriteBatch, Vector2 position, float scale)
        {
            if (mercenary != 0)
            {
                Texture2D mercenaryImage = (Texture2D)ModContent.Request<Texture2D>($"TF2/Content/Textures/Main_Menu_{(TF2Player.ClassName)mercenary}", AssetRequestMode.ImmediateLoad);
                spriteBatch.Draw(mercenaryImage, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }
    }
}