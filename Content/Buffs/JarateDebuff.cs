using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Buffs
{
    public class JarateDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<JaratePlayer>().jarateDebuff = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<JarateNPC>().jarateDebuff = true;
    }

    public class JaratePlayer : ModPlayer
    {
        public bool jarateDebuff;

        public override void ResetEffects() => jarateDebuff = false;
    }

    public class JarateNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool jarateDebuff;
        public Color initialColor;
        public bool colorInitialized;

        public override void ResetEffects(NPC npc)
        {
            jarateDebuff = false;
            if (colorInitialized)
                npc.color = initialColor;
        }

        public override void AI(NPC npc)
        {
            if (jarateDebuff)
                npc.color = Color.Yellow;
        }

        public override void PostAI(NPC npc)
        {
            if (!colorInitialized)
            {
                initialColor = npc.color;
                colorInitialized = true;
            }
        }
    }
}