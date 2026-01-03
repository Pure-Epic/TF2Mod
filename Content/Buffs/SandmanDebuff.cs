using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TF2.Common;
using TF2.Content.NPCs.Buddies;
using TF2.Content.NPCs.Enemies;

namespace TF2.Content.Buffs
{
    public class SandmanDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => TF2Player.SetPlayerSpeed(player, 30);

        public override void Update(NPC npc, ref int buffIndex)
        {
            SandmanDebuffNPC sandmanDebuffNPC = npc.GetGlobalNPC<SandmanDebuffNPC>();
            sandmanDebuffNPC.sandmanDebuff = true;
            if (sandmanDebuffNPC.sandmanDebuff)
            {
                if (npc.ModNPC is Buddy buddy)
                    buddy.speedMultiplier *= 0.3f;
                else if (npc.ModNPC is BLUMercenary enemy)
                    enemy.speedMultiplier *= 0.3f;
            }
        }
    }

    public class SandmanDebuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool sandmanDebuff;
        public int timer;

        public override void ResetEffects(NPC npc) => sandmanDebuff = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.ModNPC is Buddy || npc.ModNPC is BLUMercenary) return;
            if (sandmanDebuff)
            {
                timer++;
                npc.velocity = new Vector2(npc.velocity.X / 2, npc.velocity.Y / 2);
            }
            else
                timer = 0;
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(sandmanDebuff);
            binaryWriter.Write(timer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            sandmanDebuff = binaryReader.ReadBoolean();
            timer = binaryReader.ReadInt32();
        }
    }
}