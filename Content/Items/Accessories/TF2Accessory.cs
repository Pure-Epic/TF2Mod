using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace TF2.Content.Items
{
    public abstract class TF2AccessoryPrimary : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.accessory = true;
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
                    otherAccessory.ModItem is TF2AccessoryPrimary)
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

    public abstract class TF2AccessorySecondary : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.accessory = true;
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
                    otherAccessory.ModItem is TF2AccessorySecondary)
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

    public abstract class TF2Accessory : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.accessory = true;
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
                    otherAccessory.ModItem is TF2Accessory)
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
}