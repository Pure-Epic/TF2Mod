using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Content.NPCs.Buddies;

namespace TF2.Content.Items.Buddies
{
    public abstract class BuddyItem : TF2Item
    {
        protected override string CustomDescription => Language.GetTextValue("Mods.TF2.UI.Items.Buddy");

        public virtual int BuddyType => NPCID.None;

        public virtual int BuddyCooldown => TF2.Time(60);

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<UniqueRarity>();
            qualityHashSet.Add(Unique);
            availability = Purchase;
            noThe = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);
    }

    public class ScoutBuddy : BuddyItem
    {
        public override int BuddyType => ModContent.NPCType<ScoutNPC>();

        public override int BuddyCooldown => TF2.Time(30);

        public override string Texture => "TF2/Content/Textures/UI/MercenaryCreationMenu/Scout";
    }

    public class SoldierBuddy : BuddyItem
    {
        public override int BuddyType => ModContent.NPCType<SoldierNPC>();

        public override int BuddyCooldown => TF2.Time(60);

        public override string Texture => "TF2/Content/Textures/UI/MercenaryCreationMenu/Soldier";
    }

    public class PyroBuddy : BuddyItem
    {
        public override int BuddyType => ModContent.NPCType<PyroNPC>();

        public override int BuddyCooldown => TF2.Time(60);

        public override string Texture => "TF2/Content/Textures/UI/MercenaryCreationMenu/Pyro";
    }

    public class DemomanBuddy : BuddyItem
    {
        public override int BuddyType => ModContent.NPCType<DemomanNPC>();

        public override int BuddyCooldown => TF2.Time(45);

        public override string Texture => "TF2/Content/Textures/UI/MercenaryCreationMenu/Demoman";
    }

    public class HeavyBuddy : BuddyItem
    {
        public override int BuddyType => ModContent.NPCType<HeavyNPC>();

        public override int BuddyCooldown => TF2.Time(90);

        public override string Texture => "TF2/Content/Textures/UI/MercenaryCreationMenu/Heavy";
    }

    public class EngineerBuddy : BuddyItem
    {
        public override int BuddyType => ModContent.NPCType<EngineerNPC>();

        public override int BuddyCooldown => TF2.Time(60);

        public override string Texture => "TF2/Content/Textures/UI/MercenaryCreationMenu/Engineer";
    }

    public class MedicBuddy : BuddyItem
    {
        public override int BuddyType => ModContent.NPCType<MedicNPC>();

        public override int BuddyCooldown => TF2.Time(60);

        public override string Texture => "TF2/Content/Textures/UI/MercenaryCreationMenu/Medic";
    }

    public class SniperBuddy : BuddyItem
    {
        public override int BuddyType => ModContent.NPCType<SniperNPC>();

        public override int BuddyCooldown => TF2.Time(45);

        public override string Texture => "TF2/Content/Textures/UI/MercenaryCreationMenu/Sniper";
    }

    public class SpyBuddy : BuddyItem
    {
        public override int BuddyType => ModContent.NPCType<SpyNPC>();

        public override int BuddyCooldown => TF2.Time(45);

        public override string Texture => "TF2/Content/Textures/UI/MercenaryCreationMenu/Spy";
    }
}