using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using TF2.Content.Items.Demoman;
using TF2.Content.Items.Engineer;
using TF2.Content.Items.Heavy;
using TF2.Content.Items.Medic;
using TF2.Content.Items.Pyro;
using TF2.Content.Items.Scout;
using TF2.Content.Items.Sniper;
using TF2.Content.Items.Soldier;
using TF2.Content.Items.Spy;

namespace TF2.Content.Items.Consumables
{
    public class MannCoStorePackage : ModItem
    {
        public bool keyFound;

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.width = 35;
            Item.height = 35;
            Item.consumable = true;
            Item.knockBack = 0f;
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            IItemDropRule[] weapons = new IItemDropRule[] {
                ItemDropRule.Common(ModContent.ItemType<ForceANature>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Sandman>(), 1),
                ItemDropRule.Common(ModContent.ItemType<BonkAtomicPunch>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Equalizer>(), 1),
                ItemDropRule.Common(ModContent.ItemType<DirectHit>(), 1),
                ItemDropRule.Common(ModContent.ItemType<BuffBanner>(), 1),
                ItemDropRule.Common(ModContent.ItemType<FlareGun>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Backburner>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Axtinguisher>(), 1),
                ItemDropRule.Common(ModContent.ItemType<CharginTarge>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Eyelander>(), 1),
                ItemDropRule.Common(ModContent.ItemType<ScottishResistance>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Sandvich>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Natascha>(), 1),
                ItemDropRule.Common(ModContent.ItemType<KillingGlovesOfBoxing>(), 1),
                ItemDropRule.Common(ModContent.ItemType<FrontierJustice>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Gunslinger>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Wrangler>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Blutsauger>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Kritzkrieg>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Ubersaw>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Huntsman>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Jarate>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Razorback>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Ambassador>(), 1),
                ItemDropRule.Common(ModContent.ItemType<CloakAndDagger>(), 1),
                ItemDropRule.Common(ModContent.ItemType<DeadRinger>(), 1),
            };
            itemLoot.Add(new OneFromRulesRule(1, weapons));
        }
    }
}