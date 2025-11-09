using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Buffs
{
    public class Radiation : ModBuff
    {
        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<RadioactivePlayer>().radiationBuff = true;
    }

    public class RadioactivePlayer : ModPlayer
    {
        public bool radiationBuff;
        public bool penalty;

        public override void ResetEffects() => radiationBuff = false;

        public override void PostUpdate()
        {
            if (penalty && !radiationBuff)
            {
                Player.AddBuff(BuffID.Slow, TF2.Time(5));
                penalty = false;
            }
            if (radiationBuff && Player.GetModPlayer<RadioactivePlayer>().radiationBuff)
            {
                Player.controlUseItem = false;
                Player.controlUseTile = false;
            }
        }

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (radiationBuff)
                penalty = true;
            return radiationBuff;
        }
    }

    public class BonkItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool CanUseItem(Item item, Player player)
        {
            if (player.GetModPlayer<RadioactivePlayer>().radiationBuff)
            {
                player.controlUseItem = false;
                player.controlUseTile = false;
                return false;
            }
            else
                return true;
        }
    }
}