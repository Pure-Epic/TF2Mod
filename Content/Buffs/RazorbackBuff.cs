using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Sniper;

namespace TF2.Content.Buffs
{
    public class RazorbackBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<RazorbackPlayer>().evadeHit = true;

        public override bool RightClick(int buffIndex)
        {
            Main.LocalPlayer.GetModPlayer<RazorbackPlayer>().timer = 0;
            return true;
        }
    }
}