using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items;
using TF2.Content.Items.Soldier;

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
            TF2Weapon.SetPlayerSpeed(player, 135);
        }
    }
}