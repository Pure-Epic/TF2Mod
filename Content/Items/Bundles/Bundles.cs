using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items.Ammo;
using TF2.Content.Items.Armor;
using TF2.Content.Items.MultiClass;
using TF2.Content.Items.Scout;
using TF2.Content.Items.Soldier;
using TF2.Content.Items.Pyro;
using TF2.Content.Items.Demoman;
using TF2.Content.Items.Heavy;
using TF2.Content.Items.Engineer;
using TF2.Content.Items.Medic;
using TF2.Content.Items.Sniper;
using TF2.Content.Items.Spy;

namespace TF2.Content.Items.Bundles
{
    public class ScoutBundle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scout Starter Pack");
            Tooltip.SetDefault("Contains Scattergun, Pistol, Bat, and Scout Class Token");
        }

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
            var entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Scattergun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Pistol_Scout>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Bat>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<ScoutClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SecondaryAmmo>(), 1000);
        }
    }

    public class SoldierBundle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soldier Starter Pack");
            Tooltip.SetDefault("Contains Rocket Launcher, Shotgun, Shovel, and Soldier Class Token");
        }

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
            var entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<RocketLauncher>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun_Soldier>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shovel>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SoldierClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SecondaryAmmo>(), 1000);
        }
    }

    public class PyroBundle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pyro Starter Pack");
            Tooltip.SetDefault("Contains Flame Thrower, Shotgun, Fire Axe, and Pyro Class Token");
        }

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
            var entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<FlameThrower>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun_Pyro>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<FireAxe>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<PyroClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SecondaryAmmo>(), 1000);
        }
    }

    public class DemomanBundle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demoman Starter Pack");
            Tooltip.SetDefault("Contains Grenade Launcher, Stickybomb Launcher, Bottle, and Demoman Class Token");
        }

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
            var entitySource = player.GetSource_OpenItem(Type);

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
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavy Starter Pack");
            Tooltip.SetDefault("Contains Minigun, Shotgun, Fists, and Heavy Class Token");
        }

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
            var entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Minigun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun_Heavy>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Fists>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<HeavyClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SecondaryAmmo>(), 1000);
        }
    }

    public class EngineerBundle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Engineer Starter Pack");
            Tooltip.SetDefault("Contains Shotgun, Pistol, Wrench, Sentry Tool, Dispenser Tool, and Engineer Class Token");
        }

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
            var entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Shotgun_Engineer>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Pistol_Engineer>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Wrench>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<BuildToolSentry>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<BuildTool>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<EngineerClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<SecondaryAmmo>(), 1000);
        }
    }

    public class MedicBundle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Medic Starter Pack");
            Tooltip.SetDefault("Contains Syringe Gun, Medi Gun, Bonesaw, and Medic Class Token");
        }

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
            var entitySource = player.GetSource_OpenItem(Type);

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
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sniper Starter Pack");
            Tooltip.SetDefault("Contains Sniper Rifle, SMG, Kukri, and Sniper Class Token");
        }

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
            var entitySource = player.GetSource_OpenItem(Type);

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
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spy Starter Pack");
            Tooltip.SetDefault("Contains Revolver, Knife, Sapper, Invis Watch, and Spy Class Token");
        }

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
            var entitySource = player.GetSource_OpenItem(Type);

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
