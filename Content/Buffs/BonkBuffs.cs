using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Buffs
{
    public class Radiation : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Radiation");
            Description.SetDefault("Invinciblity with some side effects");
            Main.buffNoSave[Type] = true;
        }

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

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (radiationBuff) return false;
            return true;
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (radiationBuff)
            {
                penalty = true;
                damage = 0;
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (radiationBuff)
            {
                penalty = true;
                damage = 0;
            }
        }
    }

    public class BonkCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bonk! Atomic Punch Cooldown");
            Description.SetDefault("You cannot drink Bonk! Atomic Punch");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            TF2BuffBase.cooldownBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<BonkPlayer>().bonkCooldown = true;
    }

    public class BonkPlayer : ModPlayer
    {
        public bool bonkCooldown;

        public override void ResetEffects() => bonkCooldown = false;
    }

    public class BonkItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool CanUseItem(Item item, Player player) => !player.GetModPlayer<RadioactivePlayer>().radiationBuff;
    }
}