using Terraria;
using Terraria.ID;

namespace TF2.Content.Items.Consumables
{
    public class SmallAmmoPoint : SmallAmmoBox
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.White;
            Item.ResearchUnlockCount = 0;
        }

        public override void GrabRange(Player player, ref int grabRange) => grabRange = 2500;
    }

    public class MediumAmmoPoint : MediumAmmoBox
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.White;
            Item.ResearchUnlockCount = 0;
        }

        public override void GrabRange(Player player, ref int grabRange) => grabRange = 2500;
    }

    public class LargeAmmoPoint : LargeAmmoBox
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.rare = ItemRarityID.White;
            Item.ResearchUnlockCount = 0;
        }

        public override void GrabRange(Player player, ref int grabRange) => grabRange = 2500;
    }

    public class SmallHealthPoint : SmallHealth
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.White;
            Item.ResearchUnlockCount = 0;
        }

        public override void GrabRange(Player player, ref int grabRange) => grabRange = 2500;
    }

    public class MediumHealthPoint : MediumHealth
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.White;
            Item.ResearchUnlockCount = 0;
        }

        public override void GrabRange(Player player, ref int grabRange) => grabRange = 2500;
    }

    public class LargeHealthPoint : LargeHealth
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.rare = ItemRarityID.White;
            Item.ResearchUnlockCount = 0;
        }

        public override void GrabRange(Player player, ref int grabRange) => grabRange = 2500;
    }
}