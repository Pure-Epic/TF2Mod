using Terraria.ModLoader;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Medic
{
    public class HealingBeamKritzkrieg : HealingBeam
    {
        public override string Texture => "TF2/Content/Items/Weapons/Medic/Kritzkrieg";

        protected override float UberchargeMultiplier => 1.25f;

        protected override int UberCharge => ModContent.BuffType<KritzkriegBuff>();
    }
}