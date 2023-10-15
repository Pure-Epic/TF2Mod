using Terraria;
using Terraria.ModLoader;

namespace TF2.Content.Buffs
{
    public class DalokohsBarBuff : ModBuff
    {
        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<DalokohsBarPlayer>().dalokohsBarBuff = true;
    }

    public class DalokohsBarPlayer : ModPlayer
    {
        public bool dalokohsBarBuff;

        public override void ResetEffects() => dalokohsBarBuff = false;

        public override void PostUpdate()
        {
            if (dalokohsBarBuff)
                Player.statLifeMax2 = (int)(Player.statLifeMax2 * 1.1666666666f);
        }
    }
}