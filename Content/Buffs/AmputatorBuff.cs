using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TF2.Content.NPCs.Buddies;

namespace TF2.Content.Buffs
{
    public class AmputatorBuff : ModBuff
    {
        public override string Texture => "TF2/Content/Buffs/MediGunBuff";

        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<AmputatorBuffNPC>().amputatorBuff = true;
    }

    public class AmputatorBuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool amputatorBuff;
        private int timer;

        public override void ResetEffects(NPC npc)
        {
            amputatorBuff = false;
            if (!npc.HasBuff<AmputatorBuff>())
                timer = 0;
        }

        public override void AI(NPC npc)
        {
            if (amputatorBuff)
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
            binaryWriter.Write(amputatorBuff);
            binaryWriter.Write(timer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            amputatorBuff = binaryReader.ReadBoolean();
            timer = binaryReader.ReadInt32();
        }
    }
}