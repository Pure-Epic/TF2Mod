using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TF2.Common
{
    public class TF2Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("NPCs")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool Shop;

        [Header("Enemies")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool Loot;

        [DefaultValue(false)]
        public bool FreeHealthPacks;

        [DefaultValue(false)]
        [ReloadRequired]
        public bool NoTF2Loot;

        [Header("CustomMusic")]
        [DefaultValue(false)]
        public bool BossMusic;
    }

    public class TF2ConfigClient : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Weapons")]
        [DefaultValue(false)]
        public bool SingleReload;

        [Header("MainMenu")]
        [DefaultValue(false)]
        public bool DisablePlayerIcons;

        [DefaultValue(false)]
        [ReloadRequired]
        public bool DefaultTips;

        [Header("MannCoStore")]
        [DefaultValue(true)]
        public bool WarningText;

        [Header("Cheats")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool InfiniteAmmo;
    }
}