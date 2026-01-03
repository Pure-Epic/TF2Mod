using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Scout;

namespace TF2.Content.Buffs
{
    public class SodaPopperBuff : ModBuff
    {
        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<SodaPopperPlayer>().sodaPopperBuff = true;
            player.GetModPlayer<TF2Player>().extraJumps += 4;
            player.GetJumpState<TF2DoubleJump>().Enable();
        }
    }
}