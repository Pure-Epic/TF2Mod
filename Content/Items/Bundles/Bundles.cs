using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Content.Items.Ammo;
using TF2.Content.Items.Armor;
using TF2.Content.Items.Demoman;
using TF2.Content.Items.Engineer;
using TF2.Content.Items.Heavy;
using TF2.Content.Items.Medic;
using TF2.Content.Items.MultiClass;
using TF2.Content.Items.Pyro;
using TF2.Content.Items.Scout;
using TF2.Content.Items.Sniper;
using TF2.Content.Items.Soldier;
using TF2.Content.Items.Spy;

namespace TF2.Content.Items.Bundles
{
    public class ScoutBundle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 45;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Scattergun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Pistol>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Bat>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<ScoutClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SecondaryAmmo>(), 1000);
        }
    }

    public class SoldierBundle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 44;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<RocketLauncher>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shovel>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SoldierClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SecondaryAmmo>(), 1000);
        }
    }

    public class PyroBundle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 41;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<FlameThrower>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<FireAxe>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<PyroClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SecondaryAmmo>(), 1000);
        }
    }

    public class DemomanBundle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 42;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<GrenadeLauncher>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<StickybombLauncher>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Bottle>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<DemomanClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SecondaryAmmo>(), 1000);
        }
    }

    public class HeavyBundle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 46;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Minigun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Fists>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<HeavyClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SecondaryAmmo>(), 1000);
        }
    }

    public class EngineerBundle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 49;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Pistol>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Wrench>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<ConstructionPDA>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<DestructionPDA>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<EngineerClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SecondaryAmmo>(), 1000);
        }
    }

    public class MedicBundle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 45;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<SyringeGun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<MediGun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Bonesaw>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<MedicClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SecondaryAmmo>(), 1000);
        }
    }

    public class SniperBundle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 49;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<SniperRifle>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SMG>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Kukri>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SniperClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SecondaryAmmo>(), 1000);
        }
    }

    public class SpyBundle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 42;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Revolver>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Knife>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Sapper>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<InvisWatch>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SpyClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SecondaryAmmo>(), 1000);
        }
    }
}