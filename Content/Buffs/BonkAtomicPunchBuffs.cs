using Terraria;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Buffs
{
    public class BonkAtomicPunchBuff : ModBuff
    {
        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<BonkAtomicPunchBuffPlayer>().bonkAtomicPunchBuff = true;
    }

    public class BonkAtomicPunchDebuff : ModBuff
    {
        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(Player player, ref int buffIndex) => TF2Player.SetPlayerSpeed(player, player.GetModPlayer<BonkAtomicPunchBuffPlayer>().damageTaken > 200f ? 50 : 75);
    }

    public class BonkAtomicPunchBuffPlayer : ModPlayer
    {
        public bool bonkAtomicPunchBuff;
        public bool penalty;
        public float damageTaken;

        public override void ResetEffects() => bonkAtomicPunchBuff = false;

        public override void PostUpdate()
        {
            if (penalty && !bonkAtomicPunchBuff)
            {
                Player.AddBuff(ModContent.BuffType<BonkAtomicPunchDebuff>(), TF2.Time(5));
                penalty = false;
            }
            if (bonkAtomicPunchBuff && Player.GetModPlayer<BonkAtomicPunchBuffPlayer>().bonkAtomicPunchBuff)
            {
                Player.controlUseItem = false;
                Player.controlUseTile = false;
            }
            if (!(bonkAtomicPunchBuff || Player.HasBuff<BonkAtomicPunchDebuff>()))
                damageTaken = 0;
        }

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (bonkAtomicPunchBuff)
            {
                damageTaken += info.Damage / Player.GetModPlayer<TF2Player>().healthMultiplier;
                Player.SetImmuneTimeForAllTypes(TF2.Time(0.5));
                penalty = true;
            }
            return bonkAtomicPunchBuff;
        }
    }

    public class BonkAtomicPunchItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool CanUseItem(Item item, Player player)
        {
            if (player.GetModPlayer<BonkAtomicPunchBuffPlayer>().bonkAtomicPunchBuff)
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