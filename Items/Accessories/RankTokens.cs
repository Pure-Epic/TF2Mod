using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Items.Accessories
{
    // This file is showcasing inheritance to implement an accessory "type" that you can only have one of equipped
    // It also shows how you can interact with inherited methods
    // Additionally, it takes advantage of ValueTuple to make code more compact

    // First, we create an abstract class that all our exclusive accessories will be based on
    // This class won't be autoloaded by tModLoader, meaning it won't "exist" in the game, and we don't need to provide it a texture
    // Further down below will be the actual items (Green/Yellow Exclusive Accessory)
    public abstract class RankToken : ModItem
    {
        public virtual void SafeSetDefaults()
        {

        }
        public sealed override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.accessory = true;
            SafeSetDefaults();
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
                    otherAccessory.ModItem is RankToken)
                {
                    // If we find an item that matches these criteria, return both the index and the item itself
                    // The second argument is just for convenience, technically we don't need it since we can get the item from just i
                    return (i, otherAccessory);
                }
            }
            // If no item is found, we return default values for index and item, always check one of them with this default when you call this method!
            return (-1, null);
        }
    }

    // Here we add our accessories, note that they inherit from ClassToken, and not ModItem

    public class Rank1 : RankToken
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mercenary I");
            Tooltip.SetDefault("75% weapon damage\n"
                             + "75% npc efficiency");
        }
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.White;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            p.classMultiplier = 0.75f;
        }
    } //EoC

    public class Rank2 : RankToken
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mercenary II");
            Tooltip.SetDefault("125% health\n"
                             + "100% weapon damage\n"
                             + "100% npc efficiency");
        }
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.White;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            p.classMultiplier = 1f;
            player.statLifeMax2 += (int)(player.statLifeMax2 * 0.25f);
        }
    } //EoW & BoC

    public class Rank3 : RankToken
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mercenary III");
            Tooltip.SetDefault("125% health\n" 
                             + "125% weapon damage\n"
                             + "125% npc efficiency\n"
                             + "+1 bullet pierce");
        }
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.White;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            p.classMultiplier = 1.25f;
            player.statLifeMax2 += (int)(player.statLifeMax2 * 0.25f);
            p.pierce += 1;
        }
    } //Skeletron

    public class Rank4 : RankToken
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Contract Killer I");
            Tooltip.SetDefault("125% health\n"
                             + "300% weapon damage\n"
                             + "300% npc efficiency\n"
                             + "+200 metal capacity\n"
                             + "+1 bullet pierce");
        }
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.White;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            p.classMultiplier = 3f;
            player.statLifeMax2 += (int)(player.statLifeMax2 * 0.25f);
            p.maxMetal = 400;
            p.pierce += 1;
        }
    } //WoF

    public class Rank5 : RankToken
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Contract Killer II");
            Tooltip.SetDefault("150% health\n"
                             + "400% weapon damage\n"
                             + "400% npc efficiency\n"
                             + "+200 metal capacity\n"
                             + "+1 bullet pierce");
        }
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.White;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            p.classMultiplier = 4f;
            player.statLifeMax2 += (int)(player.statLifeMax2 * 0.5f);
            p.maxMetal = 400;
            p.pierce += 1;
        }
    } //Mech Boss 1

    public class Rank6 : RankToken
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Contract Killer III");
            Tooltip.SetDefault("150% health\n"
                             + "500% weapon damage\n"
                             + "500% npc efficiency\n"
                             + "+200 metal capacity\n"
                             + "+2 bullet pierce");
        }
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.White;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            p.classMultiplier = 5f;
            player.statLifeMax2 += (int)(player.statLifeMax2 * 0.5f);
            p.maxMetal = 400;
            p.pierce += 2;
        }
    } //Mech Boss 3

    public class Rank7 : RankToken
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Executioner I");
            Tooltip.SetDefault("150% health\n"
                             + "750% weapon damage\n"
                             + "750% npc efficiency\n"
                             + "+200 metal capacity\n"
                             + "+2 bullet pierce");
        }
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.White;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            p.classMultiplier = 7.5f;
            player.statLifeMax2 += (int)(player.statLifeMax2 * 0.5f);
            p.maxMetal = 400;
            p.pierce += 2;
        }
    } //Plantera

    public class Rank8 : RankToken
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Executioner II");
            Tooltip.SetDefault("200% health\n"
                             + "1500% weapon damage\n"
                             + "1500% npc efficiency\n"
                             + "+400 metal capacity\n"
                             + "+2 bullet pierce");
        }
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.White;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            p.classMultiplier = 15f;
            player.statLifeMax2 += (int)(player.statLifeMax2 * 1f);
            p.maxMetal = 600;
            p.pierce += 2;
        }
    } //Lunatic Cultist

    public class Rank9 : RankToken
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Executioner III");
            Tooltip.SetDefault("200% health\n"
                             + "2000% weapon damage\n"
                             + "2000% npc efficiency\n"
                             + "+400 metal capacity\n"
                             + "+3 bullet pierce");
        }
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.White;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            p.classMultiplier = 20f;
            player.statLifeMax2 += (int)(player.statLifeMax2 * 1f);
            p.maxMetal = 600;
            p.pierce += 3;
        }
    } //Moon Lord

    public class Rank10 : RankToken
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Expert Assassin I");
            Tooltip.SetDefault("200% health\n"
                             + "5000% weapon damage\n"
                             + "5000% npc efficiency\n"
                             + "+400 metal capacity\n"
                             + "+3 bullet pierce");
        }
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.White;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            p.classMultiplier = 50f;
            player.statLifeMax2 += (int)(player.statLifeMax2 * 1f);
            p.maxMetal = 600;
            p.pierce += 3;
        }
    }

    public class Rank11 : RankToken
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Expert Assassin II");
            Tooltip.SetDefault("500% health\n"
                             + "10000% weapon damage\n"
                             + "10000% npc efficiency\n"
                             + "+400 metal capacity\n"
                             + "+3 bullet pierce");
        }
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.White;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            p.classMultiplier = 100f;
            player.statLifeMax2 += (int)(player.statLifeMax2 * 4f);
            p.maxMetal = 600;
            p.pierce += 3;
        }
    }

    public class Rank12 : RankToken
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Expert Assassin III");
            Tooltip.SetDefault("500% health\n"
                             + "20000% weapon damage\n"
                             + "20000% npc efficiency\n"
                             + "+400 metal capacity\n"
                             + "+4 bullet pierce");
        }
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.White;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            p.classMultiplier = 200f;
            player.statLifeMax2 += (int)(player.statLifeMax2 * 4f);
            p.maxMetal = 600;
            p.pierce += 4;
        }
    }

    public class Rank13 : RankToken
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Death Merchant");
            Tooltip.SetDefault("1000% health\n"
                             + "50000% weapon damage\n"
                             + "50000% npc efficiency\n"
                             + "+800 metal capacity\n"
                             + "+5 bullet pierce");
        }
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.White;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            p.classMultiplier = 500f;
            player.statLifeMax2 += (int)(player.statLifeMax2 * 9f);
            p.maxMetal = 1000;
            p.pierce += 5;
        }
    }
}