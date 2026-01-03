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
    public class NataschaDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            NataschaDebuffPlayer p = player.GetModPlayer<NataschaDebuffPlayer>();
            p.nataschaDebuff = true;
            if (p.nataschaDebuff)
                TF2Player.SetPlayerSpeed(player, 100 * p.slowMultiplier);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            NataschaDebuffNPC nataschaDebuffNPC = npc.GetGlobalNPC<NataschaDebuffNPC>();
            nataschaDebuffNPC.nataschaDebuff = true;
            if (nataschaDebuffNPC.nataschaDebuff)
            {
                if (npc.ModNPC is Buddy buddy)
                    buddy.speedMultiplier = nataschaDebuffNPC.slowMultiplier;
                else if (npc.ModNPC is BLUMercenary enemy)
                    enemy.speedMultiplier = nataschaDebuffNPC.slowMultiplier;
            }
        }
    }

    public class NataschaDebuffPlayer : ModPlayer
    {
        public bool nataschaDebuff;
        public float slowMultiplier = 1f;

        public override void ResetEffects()
        {
            nataschaDebuff = false;
            if (!Player.HasBuff<NataschaDebuff>())
                slowMultiplier = 1f;
        }
    }

    public class NataschaDebuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool nataschaDebuff;
        private int timer;
        public float slowMultiplier = 1f;

        public override void ResetEffects(NPC npc)
        {
            nataschaDebuff = false;
            if (!npc.HasBuff<NataschaDebuff>())
                slowMultiplier = 1f;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.ModNPC is Buddy || npc.ModNPC is BLUMercenary) return;
            if (nataschaDebuff)
            {
                timer++;
                npc.velocity = new Vector2(npc.velocity.X / 2, npc.velocity.Y / 2);
            }
            else
                timer = 0;
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(nataschaDebuff);
            binaryWriter.Write(timer);
            binaryWriter.Write(slowMultiplier);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            nataschaDebuff = binaryReader.ReadBoolean();
            timer = binaryReader.ReadInt32();
            slowMultiplier = binaryReader.ReadSingle();
        }
    }
}