using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Items
{
    public abstract class TF2Item : ModItem
    {
        public int classType;
        public int weaponType;
        public int availability;
        public const int MultiClass = 0;
        public const int Scout = 1;
        public const int Soldier = 2;
        public const int Pyro = 3;
        public const int Demoman = 4;
        public const int Heavy = 5;
        public const int Engineer = 6;
        public const int Medic = 7;
        public const int Sniper = 8;
        public const int Spy = 9;
        public const int Primary = 1;
        public const int Secondary = 2;
        public const int Melee = 3;
        public const int PDA = 4;
        public const int PDA2 = 5;
        protected const int Starter = 1;
        protected const int Unlock = 2;
        protected const int Craft = 3;
        protected const int Contract = 4;
        protected const int Exclusive = 5;
        protected const int Stock = 1;
        protected const int Unique = 2;
        protected const int Vintage = 3;
        protected const int Genuine = 4;
        protected const int Strange = 5;
        protected const int Unusual = 6;

        protected int metalValue;
        protected int[] timer = new int[5];
        protected readonly HashSet<int> classHashSet = new HashSet<int>();
        protected readonly HashSet<int> qualityHashSet = new HashSet<int>();
        protected readonly int[] qualityTypes = new int[]
        {
            ItemRarityID.White,
            ModContent.RarityType<NormalRarity>(),
            ModContent.RarityType<UniqueRarity>(),
            0,
            0,
            0,
            ModContent.RarityType<UnusualRarity>()
        };

        public enum Classes
        {
            Classless,
            Scout,
            Soldier,
            Pyro,
            Demoman,
            Heavy,
            Engineer,
            Medic,
            Sniper,
            Spy
        }

        public enum Availability
        {
            Exotic,
            Starter,
            Unlocked,
            Crafted,
            Contract,
            Exclusive
        }

        public enum WeaponCategory
        {
            Weapon,
            Primary,
            Secondary,
            Melee,
            PDA,
            PDA2
        }

        protected virtual int WeaponResearchCost() => availability switch
        {
            Starter => 1,
            Unlock or Craft or Contract => 5,
            Exclusive => 10,
            _ => 0,
        };

        protected virtual void WeaponStatistics()
        { }

        protected virtual void WeaponDescription(List<TooltipLine> description)
        { }

        protected virtual void WeaponAddQuality(int quality)
        {
            if (quality <= 0 || quality > Unusual)
                quality = 0;
            if (qualityHashSet.Add(quality) && qualityHashSet.Max() <= quality)
                Item.rare = qualityTypes[quality];
        }

        protected virtual void WeaponMultiClassUpdate(Player player)
        { }

        protected void SetWeaponClass(int[] classes)
        {
            foreach (int i in classes)
            {
                if (i > 0 && i <= Spy)
                    classHashSet.Add(i);
            }
        }

        protected virtual void SetWeaponSlot(int weaponCategory)
        { }

        protected static void RemoveDefaultTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine materialTooltip = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(materialTooltip);
            TooltipLine damageTooltip = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            tooltips.Remove(damageTooltip);
            TooltipLine speedTooltip = tooltips.FirstOrDefault(x => x.Name == "Speed" && x.Mod == "Terraria");
            tooltips.Remove(speedTooltip);
            TooltipLine knockbackTooltip = tooltips.FirstOrDefault(x => x.Name == "Knockback" && x.Mod == "Terraria");
            tooltips.Remove(knockbackTooltip);
            TooltipLine equipableTooltip = tooltips.FirstOrDefault(x => x.Name == "Equipable" && x.Mod == "Terraria");
            tooltips.Remove(equipableTooltip);
        }

        protected void AddHeader(List<TooltipLine> description)
        {
            string text = (string)this.GetLocalization("Header");
            TooltipLine line = new TooltipLine(Mod, "Header", text)
            {
                OverrideColor = new Color(235, 226, 202)
            };
            description.Add(line);
        }

        protected void AddPositiveAttribute(List<TooltipLine> description)
        {
            string text = (string)this.GetLocalization("Upsides");
            TooltipLine line = new TooltipLine(Mod, "Positive Attributes", text)
            {
                OverrideColor = new Color(153, 204, 255)
            };
            description.Add(line);
        }

        protected void AddNegativeAttribute(List<TooltipLine> description)
        {
            string text = (string)this.GetLocalization("Downsides");
            TooltipLine line = new TooltipLine(Mod, "Negative Attributes", text)
            {
                OverrideColor = new Color(255, 64, 64)
            };
            description.Add(line);
        }

        protected void AddNeutralAttribute(List<TooltipLine> description)
        {
            string text = (string)this.GetLocalization("Notes");
            TooltipLine line = new TooltipLine(Mod, "Neutral Attributes", text)
            {
                OverrideColor = new Color(235, 226, 202)
            };
            description.Add(line);
        }

        protected void AddOtherAttribute(List<TooltipLine> description, string customText = "")
        {
            string text = customText;
            TooltipLine line = new TooltipLine(Mod, "Neutral Attributes", text)
            {
                OverrideColor = new Color(235, 226, 202)
            };
            description.Add(line);
        }

        protected void SetWeaponCategory(int classCategory, int weaponCategory, int quality, int weaponAvailability)
        {
            classType = classCategory;
            weaponType = weaponCategory;
            availability = weaponAvailability;
            WeaponAddQuality(quality);
        }

        protected void SetWeaponPrice(int weapon = 0, int refined = 0, int reclaimed = 0, int scrap = 0)
        {
            int metalCost = refined * 18 + reclaimed * 6 + scrap * 2;
            metalValue = Item.buyPrice(platinum: weapon, gold: metalCost);
        }

        protected void SetTimers(int timer1 = 0, int timer2 = 0, int timer3 = 0, int timer4 = 0, int timer5 = 0)
        {
            timer[0] = timer1;
            timer[1] = timer2;
            timer[2] = timer3;
            timer[3] = timer4;
            timer[4] = timer5;
        }

        protected void ClampVariables()
        {
            if (classType <= 0 || classType > Spy)
                classType = 0;
            if (weaponType <= 0 || weaponType > PDA2)
                weaponType = 0;
            if (availability <= 0 || availability > Exclusive)
                availability = 0;
        }

        protected static int Time(double time) => (int)Math.Round(time * 60);

        public bool IsWeaponType(int type) => weaponType == type;

        public bool HasClass(int classes) => classHashSet.Contains(classes);

        public bool HasQuality(int quality) => qualityHashSet.Contains(quality);
    }
}
