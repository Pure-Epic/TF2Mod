using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Demoman;
using TF2.Content.Items.Weapons.Engineer;
using TF2.Content.Items.Weapons.Heavy;
using TF2.Content.Items.Weapons.Medic;
using TF2.Content.Items.Weapons.MultiClass;
using TF2.Content.Items.Weapons.Pyro;
using TF2.Content.Items.Weapons.Scout;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.Items.Weapons.Soldier;
using TF2.Content.Items.Weapons.Spy;
using TF2.Content.UI.Inventory;

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

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = TF2Item.Scout;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Scattergun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Pistol>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Bat>());
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

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = TF2Item.Soldier;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<RocketLauncher>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shovel>());
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

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = TF2Item.Pyro;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<FlameThrower>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<FireAxe>());
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

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = TF2Item.Demoman;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<GrenadeLauncher>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<StickybombLauncher>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Bottle>());
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

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = TF2Item.Heavy;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Minigun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Fists>());
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

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = TF2Item.Engineer;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Pistol>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Wrench>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<ConstructionPDA>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<DestructionPDA>());
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

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = TF2Item.Medic;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SyringeGun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<MediGun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Bonesaw>());
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

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = TF2Item.Sniper;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SniperRifle>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SMG>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Kukri>());
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

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = TF2Item.Spy;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Revolver>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Knife>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Sapper>());
            ModContent.GetInstance<PDASlot>().FunctionalItem.SetDefaults(ModContent.ItemType<InvisWatch>());
        }
    }
}