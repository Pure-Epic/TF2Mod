﻿using Terraria;
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
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (ModContent.GetInstance<TF2Config>().BossMusic && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return true;
            }
            return false;
        }
    }
}