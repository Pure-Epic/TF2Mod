using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Buffs
{
    public class FoodCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavylicious");
            Description.SetDefault("You cannot eat");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            TF2BuffBase.cooldownBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<FoodPlayer>().foodCooldown = true;
    }

    public class FoodPlayer : ModPlayer
    {
        public bool foodCooldown;

        public override void ResetEffects() => foodCooldown = false;
    }
}