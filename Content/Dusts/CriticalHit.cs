using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TF2.Content.Dusts
{
    public class CriticalHit : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 50, 28);
        }

        public override bool Update(Dust dust)
        {
            dust.scale += 0.01f;

            if (dust.scale > 1.5f)
                dust.active = false;

            return false;
        }
    }

    public class MiniCrit : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 37, 24);
        }

        public override bool Update(Dust dust)
        {
            dust.scale += 0.01f;

            if (dust.scale > 1.5f)
                dust.active = false;

            return false;
        }
    }
}