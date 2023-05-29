using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TF2.Content.Items.NPCSummoners
{
    public class ScoutSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scout");
            Tooltip.SetDefault("Summons Scout");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;         
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/spawn_item");
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        public int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.ScoutNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.ScoutNPC>(), player.whoAmI);
                    NPCs.ScoutNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.ScoutNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                {
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                }
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<NPCs.ScoutNPC>()) > activePlayers;
    }

    public class SoldierSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soldier");
            Tooltip.SetDefault("Summons Soldier");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;            
            Item.useTime = 60;
            Item.useAnimation = 60;           
            Item.useStyle = ItemUseStyleID.HoldUp;           
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/spawn_item");          
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        public int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.SoldierNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.SoldierNPC>(), player.whoAmI);
                    NPCs.SoldierNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.SoldierNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                {
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                }
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<NPCs.SoldierNPC>()) > activePlayers;
    }

    public class PyroSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pyro");
            Tooltip.SetDefault("Summons Pyro");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;           
            Item.useTime = 60;
            Item.useAnimation = 60;            
            Item.useStyle = ItemUseStyleID.HoldUp;       
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/spawn_item");           
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        public int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.PyroNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.PyroNPC>(), player.whoAmI);
                    NPCs.PyroNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.PyroNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                {
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                }
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<NPCs.PyroNPC>()) > activePlayers;
    }

    public class DemomanSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demoman");
            Tooltip.SetDefault("Summons Demoman");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;           
            Item.useTime = 60;
            Item.useAnimation = 60;            
            Item.useStyle = ItemUseStyleID.HoldUp;            
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/spawn_item");           
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        public int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.DemomanNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.DemomanNPC>(), player.whoAmI);
                    NPCs.DemomanNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.DemomanNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                    Item.stack -= 1;
                }
                else if (player.altFunctionUse == 2)
                {
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                }
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<NPCs.DemomanNPC>()) > activePlayers;
    }

    public class HeavySummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavy");
            Tooltip.SetDefault("Summons Heavy");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;           
            Item.useTime = 60;
            Item.useAnimation = 60;           
            Item.useStyle = ItemUseStyleID.HoldUp;           
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/spawn_item");           
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        public int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.HeavyNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.HeavyNPC>(), player.whoAmI);
                    NPCs.HeavyNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.HeavyNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                {
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                }
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<NPCs.HeavyNPC>()) > activePlayers;
    }

    public class EngineerSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Engineer");
            Tooltip.SetDefault("Summons Engineer");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;          
            Item.useTime = 60;
            Item.useAnimation = 60;         
            Item.useStyle = ItemUseStyleID.HoldUp;         
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/spawn_item");         
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        public int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.EngineerNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.EngineerNPC>(), player.whoAmI);
                    NPCs.EngineerNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.EngineerNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                {
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                }
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<NPCs.EngineerNPC>()) > activePlayers;
    }

    public class MedicSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Medic");
            Tooltip.SetDefault("Summons Medic");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;           
            Item.useTime = 60;
            Item.useAnimation = 60;        
            Item.useStyle = ItemUseStyleID.HoldUp;          
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/spawn_item");          
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        public int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.MedicNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.MedicNPC>(), player.whoAmI);
                    NPCs.MedicNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.MedicNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                {
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                }
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<NPCs.MedicNPC>()) > activePlayers;
    }

    public class SniperSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sniper");
            Tooltip.SetDefault("Summons Sniper");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;            
            Item.useTime = 60;
            Item.useAnimation = 60;         
            Item.useStyle = ItemUseStyleID.HoldUp;          
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/spawn_item");          
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        public int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.SniperNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.SniperNPC>(), player.whoAmI);
                    NPCs.SniperNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.SniperNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                {
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                }
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<NPCs.SniperNPC>()) > activePlayers;
    }

    public class SpySummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spy");
            Tooltip.SetDefault("Summons Spy");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;            
            Item.useTime = 60;
            Item.useAnimation = 60;        
            Item.useStyle = ItemUseStyleID.HoldUp;     
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/spawn_item");      
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        public int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.SpyNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.SpyNPC>(), player.whoAmI);
                    NPCs.SpyNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.SpyNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                    Item.stack -= 1;
                }
                else if (player.altFunctionUse == 2)
                {
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                }
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<NPCs.SpyNPC>()) > activePlayers;
    }
}