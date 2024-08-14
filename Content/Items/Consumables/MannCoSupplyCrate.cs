using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Weapons.Demoman;
using TF2.Content.Items.Weapons.Engineer;
using TF2.Content.Items.Weapons.Heavy;
using TF2.Content.Items.Weapons.Medic;
using TF2.Content.Items.Weapons.Pyro;
using TF2.Content.Items.Weapons.Scout;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.Items.Weapons.Soldier;
using TF2.Content.Items.Weapons.Spy;

namespace TF2.Content.Items.Consumables
{
    public class MannCoSupplyCrate : TF2Item
    {
        public bool keyFound;

        public static int[] PossibleDrops =
        [
            ModContent.ItemType<ForceANature>(),
            ModContent.ItemType<Sandman>(),
            ModContent.ItemType<BonkAtomicPunch>(),
            ModContent.ItemType<Equalizer>(),
            ModContent.ItemType<DirectHit>(),
            ModContent.ItemType<BuffBanner>(),
            ModContent.ItemType<FlareGun>(),
            ModContent.ItemType<Backburner>(),
            ModContent.ItemType<Axtinguisher>(),
            ModContent.ItemType<CharginTarge>(),
            ModContent.ItemType<Eyelander>(),
            ModContent.ItemType<ScottishResistance>(),
            ModContent.ItemType<Sandvich>(),
            ModContent.ItemType<Natascha>(),
            ModContent.ItemType<KillingGlovesOfBoxing>(),
            ModContent.ItemType<FrontierJustice>(),
            ModContent.ItemType<Gunslinger>(),
            ModContent.ItemType<Wrangler>(),
            ModContent.ItemType<Blutsauger>(),
            ModContent.ItemType<Kritzkrieg>(),
            ModContent.ItemType<Ubersaw>(),
            ModContent.ItemType<Huntsman>(),
            ModContent.ItemType<Jarate>(),
            ModContent.ItemType<Razorback>(),
            ModContent.ItemType<Ambassador>(),
            ModContent.ItemType<CloakAndDagger>(),
            ModContent.ItemType<DeadRinger>(),
        ];

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 50;

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.width = 50;
            Item.height = 50;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<UniqueRarity>();
            noThe = true;
            qualityHashSet.Add(Unique);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override bool CanRightClick()
        {
            keyFound = false;
            foreach (Item item in Main.LocalPlayer.inventory)
            {
                if (item.type == ModContent.ItemType<MannCoSupplyCrateKey>())
                {
                    item.stack--;
                    keyFound = true;
                    break;
                }
            }
            return keyFound;
        }

        public override void RightClick(Player player)
        {
            for (int i = 0; i < 3; i++)
            {
                Item item = new Item();
                item.SetDefaults(PossibleDrops[Main.rand.Next(PossibleDrops.Length + 1)]);
                (item.ModItem as TF2Item).availability = Uncrate;
                player.QuickSpawnItem(player.GetSource_GiftOrReward(), item);
            }
        }
    }
}