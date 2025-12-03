using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Buffs
{
    public class BonkAtomicPunchBuff : ModBuff
    {
        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<BonkAtomicPunchPlayer>().radiationBuff = true;
    }

    public class BonkAtomicPunchDebuff : ModBuff
    {
        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(Player player, ref int buffIndex) => TF2Player.SetPlayerSpeed(player, player.GetModPlayer<BonkAtomicPunchPlayer>().damageTaken > 200f ? 50 : 75);
    }

    public class BonkAtomicPunchPlayer : ModPlayer
    {
        public bool radiationBuff;
        public bool penalty;
        public float damageTaken;

        public override void ResetEffects() => radiationBuff = false;

        public override void PostUpdate()
        {
            if (penalty && !radiationBuff)
            {
                Player.AddBuff(ModContent.BuffType<BonkAtomicPunchDebuff>(), TF2.Time(5));
                penalty = false;
            }
            if (radiationBuff && Player.GetModPlayer<BonkAtomicPunchPlayer>().radiationBuff)
            {
                Player.controlUseItem = false;
                Player.controlUseTile = false;
            }
            if (!(radiationBuff || Player.HasBuff<BonkAtomicPunchDebuff>()))
                damageTaken = 0;
        }

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (radiationBuff)
            {
                damageTaken += info.Damage / Player.GetModPlayer<TF2Player>().healthMultiplier;
                Player.SetImmuneTimeForAllTypes(TF2.Time(0.5));
                penalty = true;
            }
            return radiationBuff;
        }
    }

    public class BonkAtomicPunchItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool CanUseItem(Item item, Player player)
        {
            if (player.GetModPlayer<BonkAtomicPunchPlayer>().radiationBuff)
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