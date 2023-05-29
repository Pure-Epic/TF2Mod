using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TF2.Common
{
    public class TF2Config : ModConfig
    {
        // ConfigScope.ClientSide should be used for client side, usually visual or audio tweaks.
        // ConfigScope.ServerSide should be used for basically everything else, including disabling items or changing NPC behaviours
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // The things in brackets are known as "Attributes".
        [Header("Enemies")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category.
        [Label("Extra Loot")] // A label is the text displayed next to the option. This should usually be a short description of what it does.
        [Tooltip("Disable enemies dropping ammo and health packs? Bosses will always drop these, even if this option is enabled.")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option.
        [DefaultValue(false)] // This sets the configs default value.
        [ReloadRequired]
        public bool Loot;
        [Label("Risky Health Packs")] // A label is the text displayed next to the option. This should usually be a short description of what it does.
        [Tooltip("Disable enemies dropping health packs when hit?")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option.
        [DefaultValue(false)] // This sets the configs default value.
        public bool FreeHealthPacks;
        [Label("Disable Loot")] // A label is the text displayed next to the option. This should usually be a short description of what it does.
        [Tooltip("Disable drops from the Team Fortess 2 mod? Overrides Extra Loot and Risky Health Packs.")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option.
        [DefaultValue(false)] // This sets the configs default value.
        [ReloadRequired]
        public bool NoTF2Loot;

        [Header("Soldier & Demoman")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category.
        [Label("Destructive Explosions")] // A label is the text displayed next to the option. This should usually be a short description of what it does.
        [Tooltip("Can explosives from weapons destroy tiles?")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option.
        [DefaultValue(false)] // This sets the configs default value.
        public bool Explosions;

        [Header("Custom Music")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category.
        [Label("Goofy Ahh Boss Music")] // A label is the text displayed next to the option. This should usually be a short description of what it does.
        [Tooltip("Replaces boss music with Fruity Robo 2 Theme.")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option.
        [DefaultValue(true)] // This sets the configs default value.
        public bool BossMusic;
    }

    public class TF2ConfigClient : ModConfig
    {

        // ConfigScope.ClientSide should be used for client side, usually visual or audio tweaks.
        // ConfigScope.ServerSide should be used for basically everything else, including disabling items or changing NPC behaviours
        public override ConfigScope Mode => ConfigScope.ClientSide;

        // The things in brackets are known as "Attributes".
        [Header("Weapons")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category.
        [Label("Unlock Single Reload Weapons")] // A label is the text displayed next to the option. This should usually be a short description of what it does.
        [Tooltip("Are these weapons allowed to fire while reloading? Enabling may desync reloading sounds.")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option.
        [DefaultValue(false)] // This sets the configs default value.
        public bool SingleReload;
        [Label("Weapon Attribute Requirements")] // A label is the text displayed next to the option. This should usually be a short description of what it does.
        [Tooltip("Apply certain weapon attributes in the inventory, instead of solely in the hotbar?")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option.
        [DefaultValue(false)] // This sets the configs default value.
        public bool InventoryStats;
        [Label("Hold and Release")] // A label is the text displayed next to the option. This should usually be a short description of what it does.
        [Tooltip("Is charge increasing automatic? (Legacy)")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option.
        [DefaultValue(false)] // This sets the configs default value.
        public bool Channel;
        [Label("Sniper Rifle Auto Charge")] // A label is the text displayed next to the option. This should usually be a short description of what it does.
        [Tooltip("Is charge increased by scoping or is it automatic? (Legacy)")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option.
        [DefaultValue(false)] // This sets the configs default value.
        public bool SniperAutoCharge;
    }
}