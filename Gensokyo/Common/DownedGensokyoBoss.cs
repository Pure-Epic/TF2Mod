using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TF2.Gensokyo.Common
{
    [ExtendsFromMod("Gensokyo")]
    public class DownedGensokyoBoss : ModSystem
    {
        public static bool downedBossRush;
        public static bool downedByakurenHijiri;
        // public static bool downedOtherBoss = false;

        public override void OnWorldUnload()
        {
            downedBossRush = false;
            downedByakurenHijiri = false;
            // downedOtherBoss = false;
        }

        // We save our data sets using TagCompounds.
        // NOTE: The tag instance provided here is always empty by default.
        public override void SaveWorldData(TagCompound tag)
        {
            if (downedBossRush)
                tag["downedBossRush"] = true;
            if (downedByakurenHijiri)
                tag["downedByakurenHijiri"] = true;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedBossRush = tag.ContainsKey("downedBossRush");
            downedByakurenHijiri = tag.ContainsKey("downedByakurenHijiri");
        }

        public override void NetSend(BinaryWriter writer)
        {
            // Order of operations is important and has to match that of NetReceive
            BitsByte bitsByte = new BitsByte();
            bitsByte[0] = downedBossRush;
            BitsByte modEvents = bitsByte;
            writer.Write(modEvents);

            bitsByte = new BitsByte();
            bitsByte[0] = downedByakurenHijiri;
            // bitsByte[1] = downedOtherBoss;
            BitsByte tier8 = bitsByte;
            writer.Write(tier8);
        }

        public override void NetReceive(BinaryReader reader)
        {
            // Order of operations is important and has to match that of NetSend
            BitsByte bitsByte = reader.ReadByte();
            BitsByte modEvents = bitsByte;
            downedBossRush = modEvents[0];

            BitsByte tier8 = reader.ReadByte();
            downedByakurenHijiri = tier8[0];
            // downedOtherBoss = tier8[1];
        }
    }
}