using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Buffs
{
    public class NataschaDebuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff_" + BuffID.Slow;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<NataschaNPC>().slowDebuff = true;
    }

    public class NataschaNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool slowDebuff;
        public int timer;

        public override void ResetEffects(NPC npc) => slowDebuff = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (slowDebuff)
            {
                timer++;
                npc.velocity = new Vector2(npc.velocity.X / 2, npc.velocity.Y / 2);
            }
            else
                timer = 0;
        }
    }
}