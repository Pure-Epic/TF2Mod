using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Sniper;

namespace TF2.Content.Buffs
{
    public class RazorbackBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sniper's Protection");
            Description.SetDefault("You will dodge the next attack");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<Items.Sniper.RazorbackPlayer>().evadeHit = true;

        public override bool RightClick(int buffIndex)
        {
            Main.LocalPlayer.GetModPlayer<RazorbackPlayer>().timer = 1800;
            return true;
        }
    }
}