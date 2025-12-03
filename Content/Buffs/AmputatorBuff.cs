using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TF2.Content.NPCs.Buddies;

namespace TF2.Content.Buffs
{
    public class AmputatorBuff : ModBuff
    {
        public override string Texture => "TF2/Content/Buffs/UberCharge";

        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<AmputatorBuffNPC>().buffActive = true;
    }

    public class AmputatorBuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private int timer;
        public bool buffActive;

        public override void ResetEffects(NPC npc)
        {
            buffActive = false;
            if (!npc.HasBuff<AmputatorBuff>())
                timer = 0;
        }

        public override void AI(NPC npc)
        {
            if (buffActive)
            {
                timer++;
                if (timer >= TF2.Time(0.25) && npc.ModNPC is Buddy buddy)
                {
                    buddy.Heal(TF2.Round(buddy.healthMultiplier * 6.25f));
                    timer = 0;
                }
            }
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(timer);
            binaryWriter.Write(buffActive);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            timer = binaryReader.ReadInt32();
            buffActive = binaryReader.ReadBoolean();
        }
    }
}