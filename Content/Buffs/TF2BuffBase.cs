using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Buffs
{
    public class TF2BuffBase : GlobalBuff
    {
        public static readonly SetFactory Factory = new(BuffLoader.BuffCount);

        public static readonly bool[] cooldownBuff = Factory.CreateBoolSet(ModContent.BuffType<BaseballCooldown>(), ModContent.BuffType<BonkCooldown>(), ModContent.BuffType<BrokenCloak>(), ModContent.BuffType<FoodCooldown>(), ModContent.BuffType<JarateCooldown>());
    }
}