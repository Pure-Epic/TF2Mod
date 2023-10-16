using Terraria;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Buffs
{
    public class Rage : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soldier's Rage");
            Description.SetDefault("Mini-crits everything");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<Items.Soldier.BuffBannerPlayer>().buffActive = true;
            player.GetModPlayer<TF2Player>().miniCrit = true;
        }
    }
}