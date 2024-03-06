using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Buffs
{
    public class TF2BuffBase : GlobalBuff
    {
        public static readonly SetFactory Factory = new(BuffLoader.BuffCount);

        public static readonly bool[] cooldownBuff = Factory.CreateBoolSet(
            ModContent.BuffType<BrokenCloak>());

        public static readonly bool[] fireBuff = Factory.CreateBoolSet(
            ModContent.BuffType<PyroFlames>(),
            ModContent.BuffType<PyroFlamesDegreaser>());
    }
}