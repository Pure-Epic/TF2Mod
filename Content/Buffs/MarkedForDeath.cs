using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TF2.Content.Buffs
{
    public class MarkedForDeath : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<MarkedForDeathPlayer>().markedForDeath = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<MarkedForDeathNPC>().markedForDeath = true;
    }

    public class MarkedForDeathPlayer : ModPlayer
    {
        public bool markedForDeath;

        public override void ResetEffects() => markedForDeath = false;
    }

    public class MarkedForDeathNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool markedForDeath;

        public override void ResetEffects(NPC npc) => markedForDeath = false;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) => binaryWriter.Write(markedForDeath);

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) => markedForDeath = binaryReader.ReadBoolean();
    }
}