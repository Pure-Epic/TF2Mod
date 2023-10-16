using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Items.Consumables
{
    public class MannCoStorePackage : ModItem
    {
        public bool keyFound;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mann Co. Store Package");
            Tooltip.SetDefault("This bag contains one free weapon from the Mann Co. store!\n"
                + "{$CommonItemTooltip.RightClickToOpen}");
        }

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
                ItemDropRule.Common(ModContent.ItemType<Scout.ForceANature>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Scout.Sandman>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Scout.BonkAtomicPunch>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Soldier.Equalizer>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Soldier.DirectHit>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Soldier.BuffBanner>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Pyro.FlareGun>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Pyro.Backburner>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Pyro.Axtinguisher>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Demoman.CharginTarge>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Demoman.Eyelander>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Demoman.ScottishResistance>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Heavy.Sandvich>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Heavy.Natascha>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Heavy.KillingGlovesofBoxing>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Engineer.FrontierJustice>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Engineer.Gunslinger>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Engineer.Wrangler>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Medic.Blutsauger>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Medic.Kritzkrieg>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Medic.Ubersaw>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Sniper.Huntsman>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Sniper.Jarate>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Sniper.Razorback>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Spy.Ambassador>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Spy.CloakandDagger>(), 1),
                ItemDropRule.Common(ModContent.ItemType<Spy.DeadRinger>(), 1),
            };
            itemLoot.Add(new OneFromRulesRule(1, weapons));
        }
    }
}
