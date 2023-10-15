using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TF2.Common
{
    public class TF2Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("Enemies")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool Loot;

        [DefaultValue(false)]
        public bool FreeHealthPacks;

        [DefaultValue(false)]
        [ReloadRequired]
        public bool NoTF2Loot;

        [Header("Soldier&Demoman")]
        [DefaultValue(false)]
        public bool Explosions;

        [Header("CustomMusic")]
        [DefaultValue(false)]
        public bool BossMusic;
    }

    public class TF2ConfigClient : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Weapons")]
        [DefaultValue(false)]
        public bool EnableWeaponSwitch;

        [DefaultValue(false)]
        public bool SingleReload;

        [DefaultValue(false)]
        public bool InventoryStats;

        [DefaultValue(false)]
        public bool Channel;

        [DefaultValue(false)]
        public bool SniperAutoCharge;

        [Header("MainMenu")]
        [DefaultValue(false)]
        public bool DisablePlayerIcons;

        [DefaultValue(false)]
        [ReloadRequired]
        public bool DefaultTips;

        [Header("Cheats")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool InfiniteAmmo;
    }
}