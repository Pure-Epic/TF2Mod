using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Items;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace TF2.Items.Bundles
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
            Item.damage = 0;
            Item.width = 58;
            Item.height = 45;
            Item.consumable = true;
            Item.knockBack = 0f;
            Item.rare = ItemRarityID.White;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            var entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Scout.Scattergun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<MultiClass.Pistol_Scout>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Scout.Bat>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Armor.ScoutClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.SecondaryAmmo>(), 1000);
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
            Item.damage = 0;
            Item.width = 58;
            Item.height = 44;
            Item.consumable = true;
            Item.knockBack = 0f;
            Item.rare = ItemRarityID.White;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            var entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Soldier.RocketLauncher>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<MultiClass.Shotgun_Soldier>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Soldier.Shovel>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Armor.SoldierClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.SecondaryAmmo>(), 1000);
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
            Item.damage = 0;
            Item.width = 58;
            Item.height = 41;
            Item.consumable = true;
            Item.knockBack = 0f;
            Item.rare = ItemRarityID.White;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            var entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Pyro.FlameThrower>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<MultiClass.Shotgun_Pyro>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Pyro.FireAxe>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Armor.PyroClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.SecondaryAmmo>(), 1000);
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
            Item.damage = 0;
            Item.width = 58;
            Item.height = 42;
            Item.consumable = true;
            Item.knockBack = 0f;
            Item.rare = ItemRarityID.White;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            var entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Demoman.GrenadeLauncher>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Demoman.StickybombLauncher>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Demoman.Bottle>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Armor.DemomanClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.SecondaryAmmo>(), 1000);
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
            Item.damage = 0;
            Item.width = 58;
            Item.height = 46;
            Item.consumable = true;
            Item.knockBack = 0f;
            Item.rare = ItemRarityID.White;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            var entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Heavy.Minigun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<MultiClass.Shotgun_Heavy>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Heavy.Fists>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Armor.HeavyClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.SecondaryAmmo>(), 1000);
        }
    }

    public class EngineerBundle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Engineer Starter Pack");
            Tooltip.SetDefault("Contains Shotgun, Pistol, Wrench, Dispenser Tool, and Engineer Class Token");
        }

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.width = 58;
            Item.height = 49;
            Item.consumable = true;
            Item.knockBack = 0f;
            Item.rare = ItemRarityID.White;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            var entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<MultiClass.Shotgun_Engineer>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<MultiClass.Pistol_Engineer>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Engineer.Wrench>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Engineer.BuildTool>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Armor.EngineerClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.SecondaryAmmo>(), 1000);
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
            Item.damage = 0;
            Item.width = 58;
            Item.height = 45;
            Item.consumable = true;
            Item.knockBack = 0f;
            Item.rare = ItemRarityID.White;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            var entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Medic.SyringeGun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Medic.MediGun>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Medic.Bonesaw>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Armor.MedicClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.SecondaryAmmo>(), 1000);
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
            Item.damage = 0;
            Item.width = 58;
            Item.height = 49;
            Item.consumable = true;
            Item.knockBack = 0f;
            Item.rare = ItemRarityID.White;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            var entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Sniper.SniperRifle>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Sniper.SMG>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Sniper.Kukri>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Armor.SniperClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.SecondaryAmmo>(), 1000);
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
            Item.damage = 0;
            Item.width = 58;
            Item.height = 42;
            Item.consumable = true;
            Item.knockBack = 0f;
            Item.rare = ItemRarityID.White;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            var entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Spy.Revolver>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Spy.Knife>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Spy.Sapper>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Spy.InvisWatch>());
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Armor.SpyClass>());

            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.PrimaryAmmo>(), 1000);
            player.QuickSpawnItem(entitySource, ModContent.ItemType<Ammo.SecondaryAmmo>(), 1000);
        }
    }
}
