using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Mounts;

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
                Player.AddBuff(BuffID.Slow, 300);
                penalty = false;
            }
        }

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (radiationBuff)
                penalty = true;
            return radiationBuff;
        }
    }

    public class DrinkCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            TF2BuffBase.cooldownBuff[Type] = true;
        }
    }

    public class BonkItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ModContent.ItemType<TF2MountItem>())
                return true;
            if (player.GetModPlayer<RadioactivePlayer>().radiationBuff)
            {
                player.controlUseItem = false;
                Main.mouseLeft = false;
                Main.mouseRight = false;
                return false;
            }
            else
                return true;
        }
    }
}