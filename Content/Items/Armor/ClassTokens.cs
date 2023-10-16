using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using TF2.Common;

namespace TF2.Content.Items.Armor
{
    // This file is showcasing inheritance to implement an accessory "type" that you can only have one of equipped
    // It also shows how you can interact with inherited methods
    // Additionally, it takes advantage of ValueTuple to make code more compact

    // First, we create an abstract class that all our exclusive accessories will be based on
    // This class won't be autoloaded by tModLoader, meaning it won't "exist" in the game, and we don't need to provide it a texture
    // Further down below will be the actual items (Green/Yellow Exclusive Accessory)
    public abstract class ClassToken : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.accessory = true;
            Item.rare = ItemRarityID.White;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            // To prevent the accessory from being equipped, we need to return false if there is one already in another slot
            // Therefore we go through each accessory slot ignoring vanity slots using FindDifferentEquippedClassToken()
            // which we declared in this class below
            if (slot < 10) // This allows the accessory to equip in vanity slots with no reservations
            {
                // Here we use named ValueTuples and retrieve the index of the item, since this is what we need here
                int index = FindDifferentEquippedClassToken().index;
                if (index != -1)
                {
                    return slot == index;
                }
            }
            // Here we want to respect individual items having custom conditions for equipability
            return base.CanEquipAccessory(player, slot, modded);
        }

        // We make our own method for compacting the code because we will need to check equipped accessories often
        // This method returns a named ValueTuple, indicated by the (Type name1, Type name2, ...) as the return type
        // This allows us to return more than one value from a method
        protected (int index, Item accessory) FindDifferentEquippedClassToken()
        {
            int maxAccessoryIndex = 5 + Main.LocalPlayer.extraAccessorySlots;
            for (int i = 3; i < 3 + maxAccessoryIndex; i++)
            {
                Item otherAccessory = Main.LocalPlayer.armor[i];
                // IsAir makes sure we don't check for "empty" slots
                // IsTheSameAs() compares two items and returns true if their types match
                // "is ClassToken" is a way of performing pattern matching
                // Here, inheritance helps us determine if the given item is indeed one of our ClassToken ones
                if (!otherAccessory.IsAir &&
                    !(Item.type == otherAccessory.type) &&
                    otherAccessory.ModItem is ClassToken)
                {
                    // If we find an item that matches these criteria, return both the index and the item itself
                    // The second argument is just for convenience, technically we don't need it since we can get the item from just i
                    return (i, otherAccessory);
                }
            }
            // If no item is found, we return default values for index and item, always check one of them with this default when you call this method!
            return (-1, null);
        }

        public override bool? PrefixChance(int pre, UnifiedRandom rand) => false;
    }

    // Here we add our accessories, note that they inherit from ClassToken, and not ModItem

    public class ScoutClass : ClassToken
    {
        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
                return;

            // Add equip textures
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Wings}", EquipType.Wings, this);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scout Class Token");
            Tooltip.SetDefault("Change class to Scout");
            SetupDrawing();
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.White;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.currentClass == 0)
                p.currentClass = 1;
            p.classAccessory = true;
            p.classHideVanity = hideVisual;
            p.hasJumpOption_Scout = true;
            p.extraJumps += 1;
            player.statLifeMax2 += 25;
            player.moveSpeed += 0.33f;
        }

        private void SetupDrawing()
        {
            // Since the equipment textures weren't loaded on the server, we can't have this code running server-side
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
    }

    public class SoldierClass : ClassToken
    {
        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
                return;

            // Add equip textures
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Wings}", EquipType.Wings, this);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soldier Class Token");
            Tooltip.SetDefault("Change class to Soldier");
            SetupDrawing();
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.White;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.currentClass == 0)
                p.currentClass = 2;
            p.classAccessory = true;
            p.classHideVanity = hideVisual;
            player.statLifeMax2 += 100;
            player.moveSpeed -= 0.20f;
        }
        private void SetupDrawing()
        {
            // Since the equipment textures weren't loaded on the server, we can't have this code running server-side
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
    }

    public class PyroClass : ClassToken
    {
        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
                return;

            // Add equip textures
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Wings}", EquipType.Wings, this);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pyro Class Token");
            Tooltip.SetDefault("Change class to Pyro");
            SetupDrawing();
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.White;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.currentClass == 0)
                p.currentClass = 3;
            p.classAccessory = true;
            p.classHideVanity = hideVisual;
            player.statLifeMax2 += 75;
        }
        private void SetupDrawing()
        {
            // Since the equipment textures weren't loaded on the server, we can't have this code running server-side
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
    }

    public class DemomanClass : ClassToken
    {
        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
                return;

            // Add equip textures
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Wings}", EquipType.Wings, this);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demoman Class Token");
            Tooltip.SetDefault("Change class to Demoman");
            SetupDrawing();
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.White;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.currentClass == 0)
                p.currentClass = 4;
            p.classAccessory = true;
            p.classHideVanity = hideVisual;
            player.moveSpeed -= 0.07f;
            player.statLifeMax2 += 75;
        }
        private void SetupDrawing()
        {
            // Since the equipment textures weren't loaded on the server, we can't have this code running server-side
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
    }

    public class HeavyClass : ClassToken
    {
        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
                return;

            // Add equip textures
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Wings}", EquipType.Wings, this);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavy Class Token");
            Tooltip.SetDefault("Change class to Heavy");
            SetupDrawing();
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.White;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.currentClass == 0)
                p.currentClass = 5;
            p.classAccessory = true;
            p.classHideVanity = hideVisual;
            player.moveSpeed -= 0.23f;
            player.statLifeMax2 += 200;
        }
        private void SetupDrawing()
        {
            // Since the equipment textures weren't loaded on the server, we can't have this code running server-side
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
    }

    public class EngineerClass : ClassToken
    {
        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
                return;

            // Add equip textures
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Wings}", EquipType.Wings, this);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Engineer Class Token");
            Tooltip.SetDefault("Change class to Engineer");
            SetupDrawing();
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.White;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.currentClass == 0)
                p.currentClass = 6;
            p.classAccessory = true;
            p.classHideVanity = hideVisual;
            player.statLifeMax2 += 25;
        }
        private void SetupDrawing()
        {
            // Since the equipment textures weren't loaded on the server, we can't have this code running server-side
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
    }

    public class MedicClass : ClassToken
    {
        public int timer;

        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
                return;

            // Add equip textures
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Wings}", EquipType.Wings, this);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Medic Class Token");
            Tooltip.SetDefault("Change class to Medic");
            SetupDrawing();
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.White;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            timer++;
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.currentClass == 0)
                p.currentClass = 7;
            p.classAccessory = true;
            p.classHideVanity = hideVisual;
            player.moveSpeed += 0.07f;
            player.statLifeMax2 += 50;
            if (timer >= 60 && !(player.statLife >= player.statLifeMax2))
            {
                if (!player.GetModPlayer<Medic.BlutsaugerPlayer>().blutsaugerEquipped)
                {
                    player.statLife += (int)(0.02f * player.statLifeMax2);
                }
                else
                {
                    player.statLife += (int)(0.006667f * player.statLifeMax2);
                }
                timer = 0;
            }
        }
        private void SetupDrawing()
        {
            // Since the equipment textures weren't loaded on the server, we can't have this code running server-side
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
    }

    public class SniperClass : ClassToken
    {
        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
                return;

            // Add equip textures
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Wings}", EquipType.Wings, this);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sniper Class Token");
            Tooltip.SetDefault("Change class to Sniper");
            SetupDrawing();
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.White;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.currentClass == 0)
                p.currentClass = 8;
            p.classAccessory = true;
            p.classHideVanity = hideVisual;
            player.statLifeMax2 += 25;
        }
        private void SetupDrawing()
        {
            // Since the equipment textures weren't loaded on the server, we can't have this code running server-side
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
    }

    public class SpyClass : ClassToken
    {
        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
                return;

            // Add equip textures
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Wings}", EquipType.Wings, this);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spy Class Token");
            Tooltip.SetDefault("Change class to Spy");
            SetupDrawing();
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.White;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.currentClass == 0)
                p.currentClass = 9;
            p.classAccessory = true;
            p.classHideVanity = hideVisual;
            player.moveSpeed += 0.07f;
            player.statLifeMax2 += 25;
        }
        private void SetupDrawing()
        {
            // Since the equipment textures weren't loaded on the server, we can't have this code running server-side
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
    }
}
