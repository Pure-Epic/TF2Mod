using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace TF2.Content.Items
{
    public class ItemTextures : ModSystem
    {
        internal static Asset<Texture2D>[] BuffBannerTextures { get; private set; }

        internal static Asset<Texture2D>[] BattalionsBackupTextures { get; private set; }

        internal static Asset<Texture2D>[] ConcherorTextures { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                BuffBannerTextures =
                [
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Items/Soldier/BuffBannerActive"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Items/Soldier/BuffBannerReverseActive")
                ];
                BattalionsBackupTextures =
                [
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Items/Soldier/BattalionsBackupActive"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Items/Soldier/BattalionsBackupReverseActive")
                ];
                ConcherorTextures =
                [
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Items/Soldier/ConcherorActive"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Items/Soldier/ConcherorReverseActive")
                ];
            }
        }
    }
}