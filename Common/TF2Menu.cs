using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace TF2.Common
{
    public class TF2Menu : ModMenu
    {
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("TF2/Content/UI/TF2");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/gamestartup1");

        /*public override ModSurfaceBackgroundStyle MenuBackgroundStyle => Mod.GetSurfaceBackgroundStyle(""); TODO: Reimplement backgrounds */

        public override string DisplayName => "Team Fortress 2";

        public override void OnSelected()
        {
            //SoundEngine.PlaySound(SoundID.Thunder); // Plays a thunder sound when this ModMenu is selected
        }
    }
}