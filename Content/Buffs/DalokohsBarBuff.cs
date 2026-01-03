using Terraria;
using Terraria.ModLoader;

namespace TF2.Content.Buffs
{
    public class DalokohsBarBuff : ModBuff
    {
        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<DalokohsBarBuffPlayer>().dalokohsBarBuff = true;
    }

    public class DalokohsBarBuffPlayer : ModPlayer
    {
        public bool dalokohsBarBuff;

        public override void ResetEffects() => dalokohsBarBuff = false;
    }
}