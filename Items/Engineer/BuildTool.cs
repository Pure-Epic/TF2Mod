using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Linq;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.Chat;

namespace TF2.Items.Engineer
{
    public class BuildTool : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dispenser Tool");
            Tooltip.SetDefault("Builds dispensers\n"
                             + "Dispensers produces hearts, ammo, and metal\n"
                             + "Dispenser level depends on the amount of metal the player currently has");
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.scale = 1f;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/spawn_item");
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
        }

        int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        int npc;

        public override bool? UseItem(Player player)
        {
            
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("NPCs such as buildings and mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            
            if (player.whoAmI == Main.myPlayer)
            {
                TFClass p = player.GetModPlayer<TFClass>();
                var source = player.GetSource_FromThis();
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.DispenserLevel1>()) <= activePlayers && NPC.CountNPCS(ModContent.NPCType<NPCs.DispenserLevel2>()) <= activePlayers && NPC.CountNPCS(ModContent.NPCType<NPCs.DispenserLevel3>()) <= activePlayers) //&& Main.netMode != NetmodeID.MultiplayerClient
                {
                    if (p.metal >= 100 && p.metal < 300)
                    {
                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.DispenserLevel1>(), player.whoAmI);
                        NPCs.DispenserLevel1 spawnedModNPC = Main.npc[npc].ModNPC as NPCs.DispenserLevel1;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 100;
                        return true;
                    }
                    else if (p.metal >= 300 && p.metal < 500)
                    {
                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.DispenserLevel2>(), player.whoAmI);
                        NPCs.DispenserLevel2 spawnedModNPC = Main.npc[npc].ModNPC as NPCs.DispenserLevel2;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 300;
                        return true;
                    }
                    else if (p.metal >= 500)
                    {
                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.DispenserLevel3>(), player.whoAmI);
                        NPCs.DispenserLevel3 spawnedModNPC = Main.npc[npc].ModNPC as NPCs.DispenserLevel3;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 500;
                        return true;
                    }
                }
                else
                {
                    if (p.metal >= 100 && p.metal < 300)
                    {
                        if (NPC.CountNPCS(ModContent.NPCType<NPCs.DispenserLevel1>()) == 0) { return false; }
                        NPCs.DispenserLevel1 currentModNPC = Main.npc[npc].ModNPC as NPCs.DispenserLevel1;
                        currentModNPC.Kill();

                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.DispenserLevel1>(), player.whoAmI);
                        NPCs.DispenserLevel1 spawnedModNPC = Main.npc[npc].ModNPC as NPCs.DispenserLevel1;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 50;
                        return true;
                    }
                    else if (p.metal >= 300 && p.metal < 500)
                    {
                        if (NPC.CountNPCS(ModContent.NPCType<NPCs.DispenserLevel2>()) == 0) { return false; }
                        NPCs.DispenserLevel2 currentModNPC = Main.npc[npc].ModNPC as NPCs.DispenserLevel2;
                        currentModNPC.Kill();

                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.DispenserLevel2>(), player.whoAmI);
                        NPCs.DispenserLevel2 spawnedModNPC = Main.npc[npc].ModNPC as NPCs.DispenserLevel2;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 150;
                        return true;
                    }
                    else if (p.metal >= 500)
                    {
                        if (NPC.CountNPCS(ModContent.NPCType<NPCs.DispenserLevel3>()) == 0) { return false; }
                        NPCs.DispenserLevel3 currentModNPC = Main.npc[npc].ModNPC as NPCs.DispenserLevel3;
                        currentModNPC.Kill();

                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.DispenserLevel3>(), player.whoAmI);
                        NPCs.DispenserLevel3 spawnedModNPC = Main.npc[npc].ModNPC as NPCs.DispenserLevel3;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 250;
                        return true;
                    }
                }
            }
            return false;
        }

        public override void HoldItem(Player player)
        {
            Item.scale = 0f;
        }
    }
}
