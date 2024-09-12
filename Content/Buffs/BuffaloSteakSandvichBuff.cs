using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.Items.Weapons;
using TF2.Content.Mounts;

namespace TF2.Content.Buffs
{
    public class BuffaloSteakSandvichBuff : ModBuff
    {
        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BuffaloSteakSandvichPlayer>().buffaloSteakSandvichBuff = true;
            TF2Player.SetPlayerSpeed(player, 130);
            player.GetModPlayer<TF2Player>().miniCrit = true;
        }
    }

    public class BuffaloSteakSandvichPlayer : ModPlayer
    {
        public bool buffaloSteakSandvichBuff;

        public override void ResetEffects() => buffaloSteakSandvichBuff = false;

        public override void PostUpdate()
        {
            if (buffaloSteakSandvichBuff)
                Player.noKnockback = true;
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (buffaloSteakSandvichBuff)
                modifiers.FinalDamage *= 0.2f;
        }
    }

    public class BuffaloSteakSandvichItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ModContent.ItemType<TF2MountItem>())
                return true;
            if (player.GetModPlayer<BuffaloSteakSandvichPlayer>().buffaloSteakSandvichBuff && item.ModItem is TF2Weapon weapon && !weapon.IsWeaponType(TF2Item.Melee))
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