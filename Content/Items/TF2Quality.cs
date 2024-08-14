using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace TF2.Content.Items
{
    public class NormalRarity : ModRarity
    {
        public override Color RarityColor => new Color(178, 178, 178);

        public override int GetPrefixedRarity(int offset, float valueMult) => Type;
    }

    public class UniqueRarity : ModRarity
    {
        public override Color RarityColor => new Color(255, 215, 0);

        public override int GetPrefixedRarity(int offset, float valueMult) => Type;
    }

    public class UnusualRarity : ModRarity
    {
        public override Color RarityColor => new Color(134, 80, 172);

        public override int GetPrefixedRarity(int offset, float valueMult) => Type;
    }
}