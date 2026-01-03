using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TF2.Content.Buffs
{
    public class JarateDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<JarateDebuffPlayer>().jarateDebuff = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<JarateDebuffNPC>().jarateDebuff = true;
    }

    public class JarateDebuffPlayer : ModPlayer
    {
        public bool jarateDebuff;

        public override void ResetEffects() => jarateDebuff = false;
    }

    public class JarateDebuffNPC : GlobalNPC
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

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(jarateDebuff);
            binaryWriter.Write(colorInitialized);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            jarateDebuff = binaryReader.ReadBoolean();
            colorInitialized = binaryReader.ReadBoolean();
        }
    }
}