using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.NPCs;

namespace TF2.Content.Items.Weapons.Engineer
{
    public class ConstructionPDA : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Engineer, PDA, Stock, Starter);
            SetWeaponSize(50, 50);
            SetPDAUseStyle();
            SetWeaponAttackSpeed(1, hide: true);
            SetWeaponAttackIntervals(altClick: true, noAmmo: true);
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        protected override bool? WeaponOnUse(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (player.GetModPlayer<JagPlayer>().jagEquipped)
            {
                p.sentryCostReduction *= 0.7f;
                p.dispenserCostReduction *= 1.2f;
            }
            int sentry = player.GetModPlayer<EngineerBuildings>().sentryWhoAmI;
            int dispenser = player.GetModPlayer<EngineerBuildings>().dispenserWhoAmI;
            IEntitySource source = player.GetSource_FromThis();
            if (player.altFunctionUse != 2)
            {
                if (!player.GetModPlayer<GunslingerPlayer>().gunslingerEquipped)
                {
                    if (NPC.CountNPCS(ModContent.NPCType<SentryLevel1>()) <= 0 && NPC.CountNPCS(ModContent.NPCType<SentryLevel2>()) <= 0 && NPC.CountNPCS(ModContent.NPCType<SentryLevel3>()) <= 0 && NPC.CountNPCS(ModContent.NPCType<MiniSentry>()) <= 0)
                    {
                        if (p.metal >= 130 * p.sentryCostReduction && p.metal < 330 * p.sentryCostReduction)
                        {
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                sentry = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<SentryLevel1>(), player.whoAmI);
                                SentryLevel1 spawnedModNPC = (SentryLevel1)Main.npc[sentry].ModNPC;
                                spawnedModNPC.npcOwner = player.whoAmI;
                                NetMessage.SendData(MessageID.SyncNPC, number: sentry);
                                player.GetModPlayer<EngineerBuildings>().sentryWhoAmI = sentry;
                            }
                            else
                                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<SentryLevel1>());
                            p.metal -= (int)(130 * p.sentryCostReduction);
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                        }
                        else if (p.metal >= 330 * p.sentryCostReduction && p.metal < 530 * p.sentryCostReduction)
                        {
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                sentry = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<SentryLevel2>(), player.whoAmI);
                                SentryLevel2 spawnedModNPC = (SentryLevel2)Main.npc[sentry].ModNPC;
                                spawnedModNPC.npcOwner = player.whoAmI;
                                NetMessage.SendData(MessageID.SyncNPC, number: sentry);
                                player.GetModPlayer<EngineerBuildings>().sentryWhoAmI = sentry;
                            }
                            else
                                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<SentryLevel2>());
                            p.metal -= (int)(330 * p.sentryCostReduction);
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                        }
                        else if (p.metal >= 530 * p.sentryCostReduction)
                        {
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                sentry = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<SentryLevel3>(), player.whoAmI);
                                SentryLevel3 spawnedModNPC = (SentryLevel3)Main.npc[sentry].ModNPC;
                                spawnedModNPC.npcOwner = player.whoAmI;
                                NetMessage.SendData(MessageID.SyncNPC, number: sentry);
                                player.GetModPlayer<EngineerBuildings>().sentryWhoAmI = sentry;
                            }
                            else
                                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<SentryLevel3>());
                            p.metal -= (int)(530 * p.sentryCostReduction);
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                        }
                    }
                    else
                    {
                        if (p.metal >= 130 * p.sentryCostReduction && p.metal < 330 * p.sentryCostReduction)
                        {
                            if (Main.netMode != NetmodeID.SinglePlayer) return false;
                            if (Main.npc[sentry].ModNPC is Sentry)
                                Main.npc[sentry].ModNPC.NPC.active = false;

                            sentry = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<SentryLevel1>(), player.whoAmI);
                            SentryLevel1 spawnedModNPC = (SentryLevel1)Main.npc[sentry].ModNPC;
                            spawnedModNPC.npcOwner = player.whoAmI;
                            NetMessage.SendData(MessageID.SyncNPC, number: sentry);
                            player.GetModPlayer<EngineerBuildings>().sentryWhoAmI = sentry;
                            p.metal -= (int)(65 * p.sentryCostReduction);
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                        }
                        else if (p.metal >= 330 * p.sentryCostReduction && p.metal < 530 * p.sentryCostReduction)
                        {
                            if (Main.netMode != NetmodeID.SinglePlayer) return false;
                            if (Main.npc[sentry].ModNPC is Sentry)
                                Main.npc[sentry].ModNPC.NPC.active = false;

                            sentry = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<SentryLevel2>(), player.whoAmI);
                            SentryLevel2 spawnedModNPC = (SentryLevel2)Main.npc[sentry].ModNPC;
                            spawnedModNPC.npcOwner = player.whoAmI;
                            NetMessage.SendData(MessageID.SyncNPC, number: sentry);
                            player.GetModPlayer<EngineerBuildings>().sentryWhoAmI = sentry;
                            p.metal -= (int)(165 * p.sentryCostReduction);
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                        }
                        else if (p.metal >= 530 * p.sentryCostReduction)
                        {
                            if (Main.netMode != NetmodeID.SinglePlayer) return false;
                            if (Main.npc[sentry].ModNPC is Sentry)
                                Main.npc[sentry].ModNPC.NPC.active = false;

                            sentry = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<SentryLevel3>(), player.whoAmI);
                            SentryLevel3 spawnedModNPC = (SentryLevel3)Main.npc[sentry].ModNPC;
                            spawnedModNPC.npcOwner = player.whoAmI;
                            NetMessage.SendData(MessageID.SyncNPC, number: sentry);
                            player.GetModPlayer<EngineerBuildings>().sentryWhoAmI = sentry;
                            p.metal -= (int)(265 * p.sentryCostReduction);
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                        }
                    }
                }
                else
                {
                    if (NPC.CountNPCS(ModContent.NPCType<SentryLevel1>()) <= 0 && NPC.CountNPCS(ModContent.NPCType<SentryLevel2>()) <= 0 && NPC.CountNPCS(ModContent.NPCType<SentryLevel3>()) <= 0 && NPC.CountNPCS(ModContent.NPCType<MiniSentry>()) <= 0)
                    {
                        if (p.metal >= 100 * p.sentryCostReduction)
                        {
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                sentry = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<MiniSentry>(), player.whoAmI);
                                MiniSentry spawnedModNPC = (MiniSentry)Main.npc[sentry].ModNPC;
                                spawnedModNPC.npcOwner = player.whoAmI;
                                NetMessage.SendData(MessageID.SyncNPC, number: sentry);
                                player.GetModPlayer<EngineerBuildings>().sentryWhoAmI = sentry;
                            }
                            else
                                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<MiniSentry>());
                            p.metal -= (int)(100 * p.sentryCostReduction);
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                        }
                    }
                    else
                    {
                        if (p.metal >= 100 * p.sentryCostReduction)
                        {
                            if (Main.netMode != NetmodeID.SinglePlayer) return false;
                            if (Main.npc[sentry].ModNPC is Sentry)
                                Main.npc[sentry].ModNPC.NPC.active = false;

                            sentry = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<MiniSentry>(), player.whoAmI);
                            MiniSentry spawnedModNPC = (MiniSentry)Main.npc[sentry].ModNPC;
                            spawnedModNPC.npcOwner = player.whoAmI;
                            NetMessage.SendData(MessageID.SyncNPC, number: sentry);
                            player.GetModPlayer<EngineerBuildings>().sentryWhoAmI = sentry;
                            p.metal -= (int)(50 * p.sentryCostReduction);
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                        }
                    }
                }
            }
            else
            {
                if (NPC.CountNPCS(ModContent.NPCType<DispenserLevel1>()) <= 0 && NPC.CountNPCS(ModContent.NPCType<DispenserLevel2>()) <= 0 && NPC.CountNPCS(ModContent.NPCType<DispenserLevel3>()) <= 0)
                {
                    if (p.metal >= 100 * p.dispenserCostReduction && p.metal < 300 * p.dispenserCostReduction)
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            dispenser = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<DispenserLevel1>(), player.whoAmI);
                            DispenserLevel1 spawnedModNPC = (DispenserLevel1)Main.npc[dispenser].ModNPC;
                            spawnedModNPC.npcOwner = player.whoAmI;
                            NetMessage.SendData(MessageID.SyncNPC, number: dispenser);
                            player.GetModPlayer<EngineerBuildings>().dispenserWhoAmI = dispenser;
                        }
                        else
                            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<DispenserLevel1>());
                        p.metal -= (int)(100 * p.dispenserCostReduction);
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                        return true;
                    }
                    else if (p.metal >= 300 * p.dispenserCostReduction && p.metal < 500 * p.dispenserCostReduction)
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            dispenser = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<DispenserLevel2>(), player.whoAmI);
                            DispenserLevel2 spawnedModNPC = (DispenserLevel2)Main.npc[dispenser].ModNPC;
                            spawnedModNPC.npcOwner = player.whoAmI;
                            NetMessage.SendData(MessageID.SyncNPC, number: dispenser);
                            player.GetModPlayer<EngineerBuildings>().dispenserWhoAmI = dispenser;
                        }
                        else
                            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<DispenserLevel2>());
                        p.metal -= (int)(300 * p.dispenserCostReduction);
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                        return true;
                    }
                    else if (p.metal >= 500 * p.dispenserCostReduction)
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            dispenser = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<DispenserLevel3>(), player.whoAmI);
                            DispenserLevel3 spawnedModNPC = (DispenserLevel3)Main.npc[dispenser].ModNPC;
                            spawnedModNPC.npcOwner = player.whoAmI;
                            NetMessage.SendData(MessageID.SyncNPC, number: dispenser);
                            player.GetModPlayer<EngineerBuildings>().dispenserWhoAmI = dispenser;
                        }
                        else
                            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<DispenserLevel3>());
                        p.metal -= (int)(500 * p.dispenserCostReduction);
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                        return true;
                    }
                }
                else
                {
                    if (p.metal >= 100 * p.dispenserCostReduction && p.metal < 300 * p.dispenserCostReduction)
                    {
                        if (Main.netMode != NetmodeID.SinglePlayer) return false;
                        if (Main.npc[dispenser].ModNPC is Dispenser)
                            Main.npc[dispenser].ModNPC.NPC.active = false;

                        dispenser = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<DispenserLevel1>(), player.whoAmI);
                        DispenserLevel1 spawnedModNPC = (DispenserLevel1)Main.npc[dispenser].ModNPC;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: dispenser);
                        player.GetModPlayer<EngineerBuildings>().dispenserWhoAmI = dispenser;
                        p.metal -= (int)(50 * p.dispenserCostReduction);
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                        return true;
                    }
                    else if (p.metal >= 300 * p.dispenserCostReduction && p.metal < 500 * p.dispenserCostReduction)
                    {
                        if (Main.netMode != NetmodeID.SinglePlayer) return false;
                        if (Main.npc[dispenser].ModNPC is Dispenser)
                            Main.npc[dispenser].ModNPC.NPC.active = false;

                        dispenser = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<DispenserLevel2>(), player.whoAmI);
                        DispenserLevel2 spawnedModNPC = (DispenserLevel2)Main.npc[dispenser].ModNPC;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: dispenser);
                        player.GetModPlayer<EngineerBuildings>().dispenserWhoAmI = dispenser;
                        p.metal -= (int)(150 * p.dispenserCostReduction);
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                        return true;
                    }
                    else if (p.metal >= 500 * p.dispenserCostReduction)
                    {
                        if (Main.netMode != NetmodeID.SinglePlayer) return false;
                        if (Main.npc[dispenser].ModNPC is Dispenser)
                            Main.npc[dispenser].ModNPC.NPC.active = false;

                        dispenser = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<DispenserLevel3>(), player.whoAmI);
                        DispenserLevel3 spawnedModNPC = (DispenserLevel3)Main.npc[dispenser].ModNPC;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: dispenser);
                        player.GetModPlayer<EngineerBuildings>().dispenserWhoAmI = dispenser;
                        p.metal -= (int)(250 * p.dispenserCostReduction);
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                        return true;
                    }
                }
            }
            return true;
        }
    }

    public class EngineerBuildings : ModPlayer
    {
        public int sentryWhoAmI = -1;
        public int dispenserWhoAmI = -1;
    }
}