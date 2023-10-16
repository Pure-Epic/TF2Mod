using Terraria;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Buffs
{
    public class MeleeCrit : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shield Charge");
            Description.SetDefault("Grants melee crits");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<TF2Player>().critMelee = true;
    }
}