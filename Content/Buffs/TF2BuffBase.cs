using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Buffs
{
    public class TF2BuffBase : GlobalBuff
    {
        public static readonly SetFactory Factory = new SetFactory(BuffLoader.BuffCount, "TF2Buffs");

        public static readonly bool[] cooldownBuff = Factory.CreateBoolSet(
            ModContent.BuffType<CloakAndDaggerDebuff>());

        public static readonly bool[] fireBuff = Factory.CreateBoolSet(
            ModContent.BuffType<FlameThrowerDebuff>(),
            ModContent.BuffType<DegreaserDebuff>());
    }
}