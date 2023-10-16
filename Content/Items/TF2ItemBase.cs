using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

// Purposely part of Content, not Common because it only affects TF2 weapons
namespace TF2.Content.Items
{
    public class TF2ItemBase : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public bool noRandomCrits;
        public bool allowBackstab; // Exclusive to Your Eternal Reward

        // TF2 weapons cannot be held by mouse; it has to be in the inventory to properly work.
        public override bool CanUseItem(Item item, Player player) => !(player.HeldItem == player.inventory[58] && player.HeldItem.ModItem is TF2Weapon);
    }

    public class NormalRarity : ModRarity
    {
        public override Color RarityColor => new(178, 178, 178);

        public override int GetPrefixedRarity(int offset, float valueMult) => Type;
    }

    public class UniqueRarity : ModRarity
    {
        public override Color RarityColor => new(255, 215, 0);

        public override int GetPrefixedRarity(int offset, float valueMult) => Type;
    }

    public class UnusualRarity : ModRarity
    {
        public override Color RarityColor => new(134, 80, 172);

        public override int GetPrefixedRarity(int offset, float valueMult) => Type;
    }
}