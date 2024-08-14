using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TF2.Content.Dusts
{
    public class Stomp : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 100, 74);
            dust.position -= new Vector2(50f, 37f);
        }

        public override bool Update(Dust dust)
        {
            dust.position.Y--;
            dust.alpha++;
            if (dust.alpha >= 255)
                dust.active = false;
            return false;
        }
    }
}