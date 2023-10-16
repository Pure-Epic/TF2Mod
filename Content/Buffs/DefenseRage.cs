using Terraria;
using Terraria.ModLoader;

namespace TF2.Content.Buffs
{
    public class DefenseRage : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soldier's Blessing");
            Description.SetDefault("Damage reduction");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<Items.Soldier.BattalionsBackupPlayer>().buffActive = true;
    }
}