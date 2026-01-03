using Terraria.ModLoader;

namespace TF2.Content.Items
{
    public class MercenaryDamage : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) => StatInheritanceData.None;

        public override bool GetEffectInheritance(DamageClass damageClass) => false;

        public override bool UseStandardCritCalcs => false;
    }
}