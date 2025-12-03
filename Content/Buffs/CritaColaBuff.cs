using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Scout;

namespace TF2.Content.Buffs
{
    public class CritaColaBuff : ModBuff
    {
        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<TF2Player>().miniCrit = true;
            player.GetModPlayer<CritaColaPlayer>().critaColaActive = true;
        }
    }
}