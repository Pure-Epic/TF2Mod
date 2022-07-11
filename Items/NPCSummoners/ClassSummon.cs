using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Linq;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.Chat;

namespace TF2.Items.NPCSummoners
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
            Item.scale = 1f;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/spawn_item");
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.maxStack = 20;
            Item.consumable = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("NPCs such as buildings and mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            if (player.whoAmI == Main.myPlayer)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.ScoutNPC>()) <= activePlayers && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.ScoutNPC>(), player.whoAmI);
                    NPCs.ScoutNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.ScoutNPC;
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

        public override bool? UseItem(Player player)
        {
            return null;
        }

        public override void HoldItem(Player player)
        {
            Item.scale = 0f;
        }

        public override bool ConsumeItem(Player player) => true;

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
            Item.scale = 1f;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/spawn_item");
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.maxStack = 20;
            Item.consumable = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("NPCs such as buildings and mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.SoldierNPC>()) <= activePlayers && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.SoldierNPC>(), player.whoAmI);
                    NPCs.SoldierNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.SoldierNPC;
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

        public override bool? UseItem(Player player)
        {
            return null;
        }

        public override void HoldItem(Player player)
        {
            Item.scale = 0f;
        }

        public override bool ConsumeItem(Player player) => true;

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
            Item.scale = 1f;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/spawn_item");
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.maxStack = 20;
            Item.consumable = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("NPCs such as buildings and mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.PyroNPC>()) <= activePlayers && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.PyroNPC>(), player.whoAmI);
                    NPCs.PyroNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.PyroNPC;
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

        public override bool? UseItem(Player player)
        {
            return null;
        }

        public override void HoldItem(Player player)
        {
            Item.scale = 0f;
        }

        public override bool ConsumeItem(Player player) => true;

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
            Item.scale = 1f;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/spawn_item");
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.maxStack = 20;
            Item.consumable = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("NPCs such as buildings and mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.DemomanNPC>()) <= activePlayers && player.altFunctionUse != 2)
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

        public override bool? UseItem(Player player)
        {
            return null;
        }

        public override void HoldItem(Player player)
        {
            Item.scale = 0f;
        }

        public override bool ConsumeItem(Player player) => true;

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
            Item.scale = 1f;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/spawn_item");
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.maxStack = 20;
            Item.consumable = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("NPCs such as buildings and mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.HeavyNPC>()) <= activePlayers && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.HeavyNPC>(), player.whoAmI);
                    NPCs.HeavyNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.HeavyNPC;
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

        public override bool? UseItem(Player player)
        {
            return null;
        }

        public override void HoldItem(Player player)
        {
            Item.scale = 0f;
        }

        public override bool ConsumeItem(Player player) => true;

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
            Item.scale = 1f;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/spawn_item");
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.maxStack = 20;
            Item.consumable = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("NPCs such as buildings and mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.EngineerNPC>()) <= activePlayers && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.EngineerNPC>(), player.whoAmI);
                    NPCs.EngineerNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.EngineerNPC;
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

        public override bool? UseItem(Player player)
        {
            return null;
        }

        public override void HoldItem(Player player)
        {
            Item.scale = 0f;
        }

        public override bool ConsumeItem(Player player) => true;

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
            Item.scale = 1f;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/spawn_item");
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.maxStack = 20;
            Item.consumable = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("NPCs such as buildings and mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.MedicNPC>()) <= activePlayers && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.MedicNPC>(), player.whoAmI);
                    NPCs.MedicNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.MedicNPC;
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

        public override bool? UseItem(Player player)
        {
            return null;
        }

        public override void HoldItem(Player player)
        {
            Item.scale = 0f;
        }

        public override bool ConsumeItem(Player player) => true;

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
            Item.scale = 1f;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/spawn_item");
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.maxStack = 20;
            Item.consumable = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("NPCs such as buildings and mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.SniperNPC>()) <= activePlayers && player.altFunctionUse != 2)
                {
                    var source = player.GetSource_FromThis();
                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.SniperNPC>(), player.whoAmI);
                    NPCs.SniperNPC spawnedModNPC = Main.npc[npc].ModNPC as NPCs.SniperNPC;
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

        public override bool? UseItem(Player player)
        {
            return null;
        }

        public override void HoldItem(Player player)
        {
            Item.scale = 0f;
        }

        public override bool ConsumeItem(Player player) => true;

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
            Item.scale = 1f;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/spawn_item");
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.maxStack = 20;
            Item.consumable = true;
            Item.value = Item.buyPrice(gold: 50);
        }

        int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        int npc;

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("NPCs such as buildings and mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.SpyNPC>()) <= activePlayers && player.altFunctionUse != 2)
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

        public override bool? UseItem(Player player)
        {
            return null;
        }

        public override void HoldItem(Player player)
        {
            Item.scale = 0f;
        }

        public override bool ConsumeItem(Player player) => true;

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<NPCs.SpyNPC>()) > activePlayers;
    }
}
