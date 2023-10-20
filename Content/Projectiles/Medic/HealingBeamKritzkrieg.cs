using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Medic
{
    public class HealingBeamKritzkrieg : HealingBeam
    {
        public override string Texture => "TF2/Content/Items/Medic/Kritzkrieg";

        protected override float Heal => 1.25f;

        protected override int UberCharge => ModContent.BuffType<KritzkriegUberCharge>();
    }
}