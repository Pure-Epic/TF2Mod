using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Common
{
    public class TF2SceneEffect : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/bossmusic");

        public override SceneEffectPriority Priority => (SceneEffectPriority)9; // This is higher than SceneEffectPriority.BossHigh

        public override bool IsSceneEffectActive(Player player)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (ModContent.GetInstance<TF2Config>().BossMusic && npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return true;
            }
            return false;
        }
    }
}