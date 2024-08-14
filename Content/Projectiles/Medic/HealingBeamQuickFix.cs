using Terraria.ModLoader;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Medic
{
    public class HealingBeamQuickFix : HealingBeam
    {
        public override string Texture => "TF2/Content/Items/Weapons/Medic/QuickFix";

        protected override float HealMultiplier => !Player.HasBuff<QuickFixUberCharge>() ? 1.4f : 4.2f;

        protected override float UberchargeMultiplier => 1.1f;

        protected override float OverhealLimit => 0.25f;

        protected override int UberCharge => ModContent.BuffType<QuickFixUberCharge>();
    }
}