using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Projectiles;
using TF2.Items.Ammo;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;

namespace TF2.NPCs
{
    public abstract class Dispenser : ModNPC
    {
        public int npcOwner = 0;

        public bool lifeInitialized = false;

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.Name == "Wrench_TF2")
            {
                damage = 0;
                if (NPC.life >= NPC.lifeMax)
                {
                    NPC.life = NPC.lifeMax;
                    return;
                }
                TFClass p = Main.player[projectile.owner].GetModPlayer<TFClass>();
                int cost = 102;
                if (!(p.metal >= cost / 3)) { return; }
                NPC.life += cost * (int)p.classMultiplier;
                p.metal -= cost / 3;

                /*
                if (NPC.lifeMax - NPC.life >= cost)
                {
                    p.metal -= (NPC.lifeMax - NPC.life) / 3;
                    NPC.life = NPC.lifeMax;
                }
                else
                {
                    NPC.life += cost;
                    p.metal -= cost / 3;
                }
                */
            }
        }

        public override bool CheckActive() => false;

        public override bool PreAI()
        {
            if (!lifeInitialized)
            {
                TFClass p = Main.player[npcOwner].GetModPlayer<TFClass>();
                NPC.life = (int)(NPC.lifeMax * p.classMultiplier);
                lifeInitialized = true;
            }
            return true;
        }

        public override void PostAI()
        {
            if (NPC.life >= NPC.lifeMax)
            {
                NPC.life = NPC.lifeMax;
                return;
            }
        }

        public void Kill()
        {
            NPC.life = 0;
        }
    }

    public class DispenserLevel1 : Dispenser
    {
        private int ai;
        private int ai2;
        private int ai3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dispenser Level 1");
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 63;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 10;
            NPC.lifeMax = 150;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Sounds/SFX/wrench_hit_build_success1");
            NPC.DeathSound = new SoundStyle("TF2/Sounds/SFX/dispenser_explode");
            NPC.friendly = true;
            NPC.GetGlobalNPC<TF2GlobalNPC>().building = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Need a Dispenser here."),
            });
        }

        public override void AI()
        {
            TFClass p = Main.player[npcOwner].GetModPlayer<TFClass>();
            NPC.lifeMax = (int)(150 * p.classMultiplier);
            NPC.defense = (int)(20 * p.classMultiplier);
            ai += 1;
            ai2 += 1;
            ai3 += 1;
            if (ai >= 120) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                var heartSource = NPC.GetSource_FromAI();
                int type = ModContent.ItemType<SmallHealth>();
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Item.NewItem(heartSource, NPC.Center, type);
                }
                else
                {
                    NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(heartSource, NPC.Center, type));
                }
                ai = 0;
            }
            if (ai2 >= 750) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                var metalSource = NPC.GetSource_FromAI();
                int type = ModContent.ItemType<Metal>();
                SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/dispenser_generate_metal"), NPC.Center);
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Item.NewItem(metalSource, NPC.Center, type);
                }
                else
                {
                    NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(metalSource, NPC.Center, type));
                }
                ai2 = 0;
            }
            if (ai3 >= 60) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                for (int i = 0; i < 10; i++)
                {
                    var ammoSource = NPC.GetSource_FromAI();
                    int type = ModContent.ItemType<PrimaryAmmo>();
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Item.NewItem(ammoSource, NPC.Center, type);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(ammoSource, NPC.Center, type));
                    }
                    int type2 = ModContent.ItemType<SecondaryAmmo>();
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Item.NewItem(ammoSource, NPC.Center, type2);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(ammoSource, NPC.Center, type2));
                    }
                }
                ai3 = 0;
            }
        }
    }

    public class DispenserLevel2 : Dispenser
    {
        private int ai;
        private int ai2;
        private int ai3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dispenser Level 2");
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 63;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 15;
            NPC.lifeMax = 180;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Sounds/SFX/wrench_hit_build_success1");
            NPC.DeathSound = new SoundStyle("TF2/Sounds/SFX/dispenser_explode");
            NPC.friendly = true;
            NPC.GetGlobalNPC<TF2GlobalNPC>().building = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Deploy a Dispenser here!"),
            });
        }

        public override void AI()
        {
            TFClass p = Main.player[npcOwner].GetModPlayer<TFClass>();
            NPC.lifeMax = (int)(180 * p.classMultiplier);
            NPC.defense = (int)(30 * p.classMultiplier);
            ai += 1;
            ai2 += 1;
            ai3 += 1;
            if (ai >= 90) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                var heartSource = NPC.GetSource_FromAI();
                int type = ModContent.ItemType<SmallHealth>();
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Item.NewItem(heartSource, NPC.Center, type);
                }
                else
                {
                    NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(heartSource, NPC.Center, type));
                }
                ai = 0;
            }
            if (ai2 >= 600) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                var metalSource = NPC.GetSource_FromAI();
                int type = ModContent.ItemType<Metal>();
                SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/dispenser_generate_metal"), NPC.Center);
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Item.NewItem(metalSource, NPC.Center, type);
                }
                else
                {
                    NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(metalSource, NPC.Center, type));
                }
                ai2 = 0;
            }
            if (ai3 >= 45) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                for (int i = 0; i < 10; i++)
                {
                    var ammoSource = NPC.GetSource_FromAI();
                    int type = ModContent.ItemType<PrimaryAmmo>();
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Item.NewItem(ammoSource, NPC.Center, type);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(ammoSource, NPC.Center, type));
                    }
                    int type2 = ModContent.ItemType<SecondaryAmmo>();
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Item.NewItem(ammoSource, NPC.Center, type2);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(ammoSource, NPC.Center, type2));
                    }
                }
                ai3 = 0;
            }
        }
    }

    public class DispenserLevel3 : Dispenser
    {
        private int ai;
        private int ai2;
        private int ai3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dispenser Level 3");
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 71;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.lifeMax = 216;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Sounds/SFX/wrench_hit_build_success1");
            NPC.DeathSound = new SoundStyle("TF2/Sounds/SFX/dispenser_explode");
            NPC.friendly = true;
            NPC.GetGlobalNPC<TF2GlobalNPC>().building = true;            
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Put Dispenser here."),
            });
        }

        public override void AI()
        {
            TFClass p = Main.player[npcOwner].GetModPlayer<TFClass>();
            NPC.lifeMax = (int)(216 * p.classMultiplier);
            NPC.defense = (int)(40 * p.classMultiplier);
            ai += 1;
            ai2 += 1;
            ai3 += 1;
            if (ai >= 60) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                var heartSource = NPC.GetSource_FromAI();
                int type = ModContent.ItemType<SmallHealth>();
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Item.NewItem(heartSource, NPC.Center, type);
                }
                else
                {
                    NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(heartSource, NPC.Center, type));
                };
                ai = 0;
            }
            if (ai2 >= 375) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                var metalSource = NPC.GetSource_FromAI();
                int type = ModContent.ItemType<Metal>();
                SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/dispenser_generate_metal"), NPC.Center);
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Item.NewItem(metalSource, NPC.Center, type);
                }
                else
                {
                    NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(metalSource, NPC.Center, type));
                }
                ai2 = 0;
            }
            if (ai3 >= 30) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                for (int i = 0; i < 10; i++)
                {
                    var ammoSource = NPC.GetSource_FromAI();
                    int type = ModContent.ItemType<PrimaryAmmo>();
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Item.NewItem(ammoSource, NPC.Center, type);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(ammoSource, NPC.Center, type));
                    }
                    int type2 = ModContent.ItemType<SecondaryAmmo>();
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Item.NewItem(ammoSource, NPC.Center, type2);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(ammoSource, NPC.Center, type2));
                    }
                }
                ai3 = 0;
            }
        }
    }
}