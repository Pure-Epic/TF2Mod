using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Soldier;

namespace TF2.Content.Buffs
{
    public class HealthRage : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<ConcherorPlayer>().buffActive = true;
            TF2Player.SetPlayerSpeed(player, 135);
        }
    }
}