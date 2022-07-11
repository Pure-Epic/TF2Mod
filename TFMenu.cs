using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;

namespace TF2
{
	public class TFMenu : ModMenu
	{
		public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("TF2/UI/TF2");

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/gamestartup1");

		/*public override ModSurfaceBackgroundStyle MenuBackgroundStyle => Mod.GetSurfaceBackgroundStyle(""); TODO: Reimplement backgrounds */

		public override string DisplayName => "Team Fortress 2";

		public override void OnSelected()
		{
			//SoundEngine.PlaySound(SoundID.Thunder); // Plays a thunder sound when this ModMenu is selected
		}
	}
}