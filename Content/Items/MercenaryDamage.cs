using Terraria.ModLoader;

namespace TF2.Content.Items
{
    public class MercenaryDamage : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) => new StatInheritanceData(
                damageInheritance: 0f,
                critChanceInheritance: 0f,
                attackSpeedInheritance: 0f,
                armorPenInheritance: 0f,
                knockbackInheritance: 0f
            );

        public override bool GetEffectInheritance(DamageClass damageClass) => false;

        public override bool UseStandardCritCalcs => false;
    }
}