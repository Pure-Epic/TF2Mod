using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Soldier;

namespace TF2.Content.Buffs
{
    public class Rage : ModBuff
    {
        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BuffBannerPlayer>().buffActive = true;
            player.GetModPlayer<TF2Player>().miniCrit = true;
        }
    }
}