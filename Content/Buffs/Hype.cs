using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Weapons.Scout;

namespace TF2.Content.Buffs
{
    public class Hype : ModBuff
    {
        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<SodaPopperPlayer>().buffActive = true;
    }
}