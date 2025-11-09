using System.Collections.Generic;
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

namespace TF2.Content.Items.Consumables.Bundles
{
    public class ScoutBundle : TF2Item
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 45;
            Item.consumable = true;
            WeaponAddQuality(Unique);
            noThe = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = Scout;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Scattergun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Pistol>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Bat>());
        }
    }

    public class SoldierBundle : TF2Item
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 44;
            Item.consumable = true;
            WeaponAddQuality(Unique);
            noThe = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = Soldier;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<RocketLauncher>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shovel>());
        }
    }

    public class PyroBundle : TF2Item
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 41;
            Item.consumable = true;
            WeaponAddQuality(Unique);
            noThe = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = Pyro;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<FlameThrower>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<FireAxe>());
        }
    }

    public class DemomanBundle : TF2Item
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 42;
            Item.consumable = true;
            WeaponAddQuality(Unique);
            noThe = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = Demoman;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<GrenadeLauncher>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<StickybombLauncher>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Bottle>());
        }
    }

    public class HeavyBundle : TF2Item
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 46;
            Item.consumable = true;
            WeaponAddQuality(Unique);
            noThe = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = Heavy;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Minigun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Fists>());
        }
    }

    public class EngineerBundle : TF2Item
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 49;
            Item.consumable = true;
            WeaponAddQuality(Unique);
            noThe = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = Engineer;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Pistol>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Wrench>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<ConstructionPDA>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<DestructionPDA>());
        }
    }

    public class MedicBundle : TF2Item
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 45;
            Item.consumable = true;
            WeaponAddQuality(Unique);
            noThe = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = Medic;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SyringeGun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<MediGun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Bonesaw>());
        }
    }

    public class SniperBundle : TF2Item
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 49;
            Item.consumable = true;
            WeaponAddQuality(Unique);
            noThe = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = Sniper;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SniperRifle>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SMG>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Kukri>());
        }
    }

    public class SpyBundle : TF2Item
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 42;
            Item.consumable = true;
            WeaponAddQuality(Unique);
            noThe = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<TF2Player>().CanChangeClass;

        public override void RightClick(Player player)
        {
            IEntitySource entitySource = player.GetSource_OpenItem(Type);
            player.GetModPlayer<TF2Player>().currentClass = Spy;
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Revolver>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Knife>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Sapper>());
            ModContent.GetInstance<PDASlot>().FunctionalItem.SetDefaults(ModContent.ItemType<InvisWatch>());
        }
    }
}