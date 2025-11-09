using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items.Modules;

namespace TF2.Content.Buffs
{
    public class TF2BuffBase : GlobalBuff
    {
        public static readonly SetFactory Factory = new SetFactory(BuffLoader.BuffCount, "TF2Buffs");

        public static readonly bool[] cooldownBuff = Factory.CreateBoolSet(
            ModContent.BuffType<BrokenCloak>());

        public static readonly bool[] fireBuff = Factory.CreateBoolSet(
            ModContent.BuffType<PyroFlames>(),
            ModContent.BuffType<PyroFlamesDegreaser>());

        public static readonly bool[] moduleBuff = Factory.CreateBoolSet(
            ModContent.BuffType<MannsAntiDanmakuSystemBuff>(),
            ModContent.BuffType<DarkAntiDanmakuSystemBuff>());
    }
}