using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using TF2.Common;

namespace TF2.Content.Items
{
    public abstract class TF2AccessoryPrimary : TF2Accessory
    {
        protected new (int index, Item accessory) FindDifferentEquippedAccessory()
        {
            int maxAccessoryIndex = 5 + Main.LocalPlayer.extraAccessorySlots;
            for (int i = 3; i < 3 + maxAccessoryIndex; i++)
            {
                Item otherAccessory = Main.LocalPlayer.armor[i];
                if (!otherAccessory.IsAir &&
                    !(Item.type == otherAccessory.type) &&
                    otherAccessory.ModItem is TF2AccessoryPrimary)
                    return (i, otherAccessory);
            }
            return (-1, null);
        }
    }

    public abstract class TF2AccessorySecondary : TF2Accessory
    {
        protected new (int index, Item accessory) FindDifferentEquippedAccessory()
        {
            int maxAccessoryIndex = 5 + Main.LocalPlayer.extraAccessorySlots;
            for (int i = 3; i < 3 + maxAccessoryIndex; i++)
            {
                Item otherAccessory = Main.LocalPlayer.armor[i];
                if (!otherAccessory.IsAir &&
                    !(Item.type == otherAccessory.type) &&
                    otherAccessory.ModItem is TF2AccessorySecondary)
                    return (i, otherAccessory);
            }
            return (-1, null);
        }
    }

    public abstract class TF2Accessory : TF2Item
    {
        // We make our own method for compacting the code because we will need to check equipped accessories often
        // This method returns a named ValueTuple, indicated by the (Type name1, Type name2, ...) as the return type
        // This allows us to return more than one value from a method
        protected (int index, Item accessory) FindDifferentEquippedAccessory()
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

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = WeaponResearchCost();

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.accessory = true;
            WeaponStatistics();
            ClampVariables();
            if (availability == Unlock)
                metalValue += Item.buyPrice(platinum: 1);
            Item.value = metalValue;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            RemoveDefaultTooltips(tooltips);
            string category = ((Classes)classType).ToString() + "'s " + ((Availability)availability).ToString() + " " + ((WeaponCategory)weaponType).ToString();
            if (classType == MultiClass)
            {
                int currentClass = Main.LocalPlayer.GetModPlayer<TF2Player>().currentClass;
                if (HasClass(currentClass))
                    category = ((Classes)currentClass).ToString() + "'s " + ((Availability)availability).ToString() + " " + ((WeaponCategory)weaponType).ToString();
                else
                    category = "Multi-Class " + ((Availability)availability).ToString() + " " + ((WeaponCategory)weaponType).ToString();
            }
            TooltipLine line = new TooltipLine(Mod, "Weapon Category", category)
            {
                OverrideColor = new Color(117, 107, 94)
            };
            tooltips.Insert(1, line);
            WeaponDescription(tooltips);
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            // To prevent the accessory from being equipped, we need to return false if there is one already in another slot
            // Therefore we go through each accessory slot ignoring vanity slots using FindDifferentEquippedAccessory()
            // which we declared in this class below
            if (slot < 10) // This allows the accessory to equip in vanity slots with no reservations
            {
                // Here we use named ValueTuples and retrieve the index of the item, since this is what we need here
                int index = FindDifferentEquippedAccessory().index;
                if (index != -1)
                {
                    return slot == index;
                }
            }
            // Here we want to respect individual items having custom conditions for equipability
            return base.CanEquipAccessory(player, slot, modded);
        }

        public override bool? PrefixChance(int pre, UnifiedRandom rand) => false;
    }
}