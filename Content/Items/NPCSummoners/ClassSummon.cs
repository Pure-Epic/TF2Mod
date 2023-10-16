using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Content.NPCs;

namespace TF2.Content.Items.NPCSummoners
{
    public class ScoutSummon : ModItem
    {
        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);
        public int npc;

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

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<ScoutNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    IEntitySource source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<ScoutNPC>(), player.whoAmI);
                    ScoutNPC spawnedModNPC = (ScoutNPC)Main.npc[npc].ModNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<ScoutNPC>()) > activePlayers;
    }

    public class SoldierSummon : ModItem
    {
        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);
        public int npc;

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

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<SoldierNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    IEntitySource source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<SoldierNPC>(), player.whoAmI);
                    SoldierNPC spawnedModNPC = (SoldierNPC)Main.npc[npc].ModNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<SoldierNPC>()) > activePlayers;
    }

    public class PyroSummon : ModItem
    {
        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);
        public int npc;

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

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<PyroNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    IEntitySource source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<PyroNPC>(), player.whoAmI);
                    PyroNPC spawnedModNPC = (PyroNPC)Main.npc[npc].ModNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<PyroNPC>()) > activePlayers;
    }

    public class DemomanSummon : ModItem
    {
        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);
        public int npc;

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

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<DemomanNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    IEntitySource source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<DemomanNPC>(), player.whoAmI);
                    DemomanNPC spawnedModNPC = (DemomanNPC)Main.npc[npc].ModNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<DemomanNPC>()) > activePlayers;
    }

    public class HeavySummon : ModItem
    {
        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);
        public int npc;

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

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<HeavyNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    IEntitySource source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<HeavyNPC>(), player.whoAmI);
                    HeavyNPC spawnedModNPC = (HeavyNPC)Main.npc[npc].ModNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<HeavyNPC>()) > activePlayers;
    }

    public class EngineerSummon : ModItem
    {
        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);
        public int npc;

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

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<EngineerNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    IEntitySource source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<EngineerNPC>(), player.whoAmI);
                    EngineerNPC spawnedModNPC = (EngineerNPC)Main.npc[npc].ModNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<EngineerNPC>()) > activePlayers;
    }

    public class MedicSummon : ModItem
    {
        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);
        public int npc;

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

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<MedicNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    IEntitySource source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<MedicNPC>(), player.whoAmI);
                    MedicNPC spawnedModNPC = (MedicNPC)Main.npc[npc].ModNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<MedicNPC>()) > activePlayers;
    }

    public class SniperSummon : ModItem
    {
        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);
        public int npc;

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

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail))
                    return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<SniperNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    IEntitySource source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<SniperNPC>(), player.whoAmI);
                    SniperNPC spawnedModNPC = (SniperNPC)Main.npc[npc].ModNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<SniperNPC>()) > activePlayers;
    }

    public class SpySummon : ModItem
    {
        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);
        public int npc;

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

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<SpyNPC>()) < activePlayers + 1 && player.altFunctionUse != 2)
                {
                    IEntitySource source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<SpyNPC>(), player.whoAmI);
                    SpyNPC spawnedModNPC = (SpyNPC)Main.npc[npc].ModNPC;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else if (player.altFunctionUse == 2)
                    Main.npc[npc].Center = new Vector2(player.Center.X, player.Center.Y - 100f);
                return true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<SpyNPC>()) > activePlayers;
    }
}