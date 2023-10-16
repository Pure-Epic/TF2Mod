using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.NPCs;
using TF2.Common;

namespace TF2.Content.Items.Engineer
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
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        //public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        public int npc;

        public override bool? UseItem(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            var source = player.GetSource_FromThis();
            if (NPC.CountNPCS(ModContent.NPCType<DispenserLevel1>()) <= 0 && NPC.CountNPCS(ModContent.NPCType<DispenserLevel2>()) <= 0 && NPC.CountNPCS(ModContent.NPCType<DispenserLevel3>()) <= 0)
            {
                if (p.metal >= 100 && p.metal < 300)
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<DispenserLevel1>(), player.whoAmI);
                        DispenserLevel1 spawnedModNPC = Main.npc[npc].ModNPC as DispenserLevel1;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: ModContent.NPCType<DispenserLevel1>());
                    }
                    p.metal -= 100;
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                    return true;
                }
                else if (p.metal >= 300 && p.metal < 500)
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<DispenserLevel2>(), player.whoAmI);
                        DispenserLevel2 spawnedModNPC = Main.npc[npc].ModNPC as DispenserLevel2;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: ModContent.NPCType<DispenserLevel2>());
                    }
                    p.metal -= 300;
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                    return true;
                }
                else if (p.metal >= 500)
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<DispenserLevel3>(), player.whoAmI);
                        DispenserLevel3 spawnedModNPC = Main.npc[npc].ModNPC as DispenserLevel3;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: ModContent.NPCType<DispenserLevel3>());
                    }
                    p.metal -= 500;
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                    return true;
                }
            }
            else
            {
                if (p.metal >= 100 && p.metal < 300)
                {
                    if (Main.netMode != NetmodeID.SinglePlayer) return false;
                    /*
                    Dispenser currentModNPC = Main.npc[npc].ModNPC as Dispenser;
                    currentModNPC.Kill();                    
                    */
                    Dispenser currentModNPC = Main.npc[npc].ModNPC as Dispenser;
                    currentModNPC.Kill();

                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<DispenserLevel1>(), player.whoAmI);
                    DispenserLevel1 spawnedModNPC = Main.npc[npc].ModNPC as DispenserLevel1;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                    p.metal -= 50;
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                    return true;
                }
                else if (p.metal >= 300 && p.metal < 500)
                {
                    if (Main.netMode != NetmodeID.SinglePlayer) return false;
                    Dispenser currentModNPC = Main.npc[npc].ModNPC as Dispenser;
                    currentModNPC.Kill();

                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<DispenserLevel2>(), player.whoAmI);
                    DispenserLevel2 spawnedModNPC = Main.npc[npc].ModNPC as DispenserLevel2;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                    p.metal -= 150;
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                    return true;
                }
                else if (p.metal >= 500)
                {
                    if (Main.netMode != NetmodeID.SinglePlayer) return false;
                    Dispenser currentModNPC = Main.npc[npc].ModNPC as Dispenser;
                    currentModNPC.Kill();

                    npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<DispenserLevel3>(), player.whoAmI);
                    DispenserLevel3 spawnedModNPC = Main.npc[npc].ModNPC as DispenserLevel3;
                    spawnedModNPC.npcOwner = player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                    p.metal -= 250;
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                    return true;
                }
            }
            return true;
        }
    }

    public class BuildToolSentry : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sentry Tool");
            Tooltip.SetDefault("Builds a sentry gun\n"
                             + "Sentry guns attack the closest enemy\n"
                             + "Sentry gun level depends on the amount of metal the player currently has");
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
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        public int npc;

        public override bool? UseItem(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            var source = player.GetSource_FromThis();
            if (!player.GetModPlayer<GunslingerPlayer>().gunslingerEquipped)
            {
                if (NPC.CountNPCS(ModContent.NPCType<SentryLevel1>()) <= 0 && NPC.CountNPCS(ModContent.NPCType<SentryLevel2>()) <= 0 && NPC.CountNPCS(ModContent.NPCType<SentryLevel3>()) <= 0)
                {
                    if (p.metal >= 130 && p.metal < 330)
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<SentryLevel1>(), player.whoAmI);
                            SentryLevel1 spawnedModNPC = Main.npc[npc].ModNPC as SentryLevel1;
                            spawnedModNPC.npcOwner = player.whoAmI;
                            NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        }
                        else
                        {
                            NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: ModContent.NPCType<SentryLevel1>());
                        }
                        p.metal -= 130;
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                    }
                    else if (p.metal >= 330 && p.metal < 530)
                    {

                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<SentryLevel2>(), player.whoAmI);
                            SentryLevel2 spawnedModNPC = Main.npc[npc].ModNPC as SentryLevel2;
                            spawnedModNPC.npcOwner = player.whoAmI;
                            NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        }
                        else
                        {
                            NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: ModContent.NPCType<SentryLevel2>());
                        }
                        p.metal -= 330;
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                    }
                    else if (p.metal >= 530)
                    {

                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<SentryLevel3>(), player.whoAmI);
                            SentryLevel3 spawnedModNPC = Main.npc[npc].ModNPC as SentryLevel3;
                            spawnedModNPC.npcOwner = player.whoAmI;
                            NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        }
                        else
                        {
                            NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: ModContent.NPCType<SentryLevel3>());
                        }
                        p.metal -= 530;
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                    }
                }
                else
                {
                    if (p.metal >= 130 && p.metal < 330)
                    {
                        if (Main.netMode != NetmodeID.SinglePlayer) return false;
                        Sentry currentModNPC = Main.npc[npc].ModNPC as Sentry;
                        currentModNPC.Kill();

                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<SentryLevel1>(), player.whoAmI);
                        SentryLevel1 spawnedModNPC = Main.npc[npc].ModNPC as SentryLevel1;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 65;
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                    }
                    else if (p.metal >= 330 && p.metal < 530)
                    {
                        if (Main.netMode != NetmodeID.SinglePlayer) return false;
                        Sentry currentModNPC = Main.npc[npc].ModNPC as Sentry;
                        currentModNPC.Kill();

                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<SentryLevel2>(), player.whoAmI);
                        SentryLevel2 spawnedModNPC = Main.npc[npc].ModNPC as SentryLevel2;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 165;
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                    }
                    else if (p.metal >= 530)
                    {
                        if (Main.netMode != NetmodeID.SinglePlayer) return false;
                        Sentry currentModNPC = Main.npc[npc].ModNPC as Sentry;
                        currentModNPC.Kill();

                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<SentryLevel3>(), player.whoAmI);
                        SentryLevel3 spawnedModNPC = Main.npc[npc].ModNPC as SentryLevel3;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 265;
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                    }
                }
            }
            else
            {
                if (NPC.CountNPCS(ModContent.NPCType<MiniSentry>()) <= 0)
                {
                    if (p.metal >= 100)
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<MiniSentry>(), player.whoAmI);
                            MiniSentry spawnedModNPC = Main.npc[npc].ModNPC as MiniSentry;
                            spawnedModNPC.npcOwner = player.whoAmI;
                            NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        }
                        else
                        {
                            NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: ModContent.NPCType<MiniSentry>());
                        }
                        p.metal -= 100;
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                    }
                }
                else
                {
                    if (p.metal >= 100)
                    {
                        if (Main.netMode != NetmodeID.SinglePlayer) return false;
                        if (NPC.CountNPCS(ModContent.NPCType<MiniSentry>()) == 0) return false;
                        MiniSentry currentModNPC = Main.npc[npc].ModNPC as MiniSentry;
                        currentModNPC.Kill();

                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<MiniSentry>(), player.whoAmI);
                        MiniSentry spawnedModNPC = Main.npc[npc].ModNPC as MiniSentry;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 50;
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                    }
                }
            }
            return true;
        }
    }
}