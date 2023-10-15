using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Ammo;
using TF2.Content.Items.Consumables;

namespace TF2.Content.NPCs
{
    public abstract class Dispenser : ModNPC
    {
        public int npcOwner = 0;

        public float npcPower = 1f;

        public bool lifeInitialized = false;

        public override bool CheckActive() => false;

        public override bool PreAI()
        {
            if (!lifeInitialized)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    TF2Player p = Main.player[npcOwner].GetModPlayer<TF2Player>();
                    NPC.life = (int)(NPC.lifeMax * p.classMultiplier);
                }
                else
                {
                    npcOwner = 0;
                    MultiplayerScaling();
                    NPC.life = (int)(NPC.lifeMax * npcPower);
                }
                lifeInitialized = true;
            }
            return true;
        }

        public override void PostAI()
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NPC.life += (int)(5 * npcPower);
            }
            if (NPC.life >= NPC.lifeMax)
            {
                NPC.life = NPC.lifeMax;
                return;
            }
        }

        public void MultiplayerScaling() // if else chain incoming
        {
            if (NPC.downedMoonlord)
                npcPower = 20f;
            else if (NPC.downedAncientCultist)
                npcPower = 15f;
            else if (NPC.downedPlantBoss)
                npcPower = 7.5f;
            else if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                npcPower = 5f;
            else if (NPC.downedMechBossAny)
                npcPower = 4f;
            else if (Main.hardMode)
                npcPower = 3f;
            else if (NPC.downedBoss3)
                npcPower = 1.25f;
            else if (NPC.downedBoss2)
                npcPower = 1f;
            else if (NPC.downedBoss1)
                npcPower = 0.75f;
            else
                npcPower = 0.5f;
        }
    }

    public class DispenserLevel1 : Dispenser
    {
        private int ai;
        private int ai2;
        private int ai3;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 63;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.lifeMax = 150;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/wrench_hit_build_success1");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/dispenser_explode");
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
            TF2Player p = Main.player[npcOwner].GetModPlayer<TF2Player>();
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                NPC.lifeMax = (int)(150 * p.classMultiplier);
                NPC.defense = (int)(20 * p.classMultiplier);
            }
            else
            {
                NPC.lifeMax = (int)(150 * npcPower);
                NPC.defense = (int)(20 * npcPower);
            }
            ai += 1;
            ai2 += 1;
            ai3 += 1;
            if (ai >= 120)
            {
                IEntitySource heartSource = NPC.GetSource_FromAI();
                int type = ModContent.ItemType<SmallHealth>();
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Item.NewItem(heartSource, NPC.Center, type);
                else
                    NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(heartSource, NPC.Center, type));
                ai = 0;
            }
            if (ai2 >= 750)
            {
                IEntitySource metalSource = NPC.GetSource_FromAI();
                int type = ModContent.ItemType<Metal>();
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/dispenser_generate_metal"), NPC.Center);
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Item.NewItem(metalSource, NPC.Center, type);
                else
                    NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(metalSource, NPC.Center, type));
                ai2 = 0;
            }
            if (ai3 >= 60)
            {
                for (int i = 0; i < 10; i++)
                {
                    IEntitySource ammoSource = NPC.GetSource_FromAI();
                    int type = ModContent.ItemType<PrimaryAmmo>();
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Item.NewItem(ammoSource, NPC.Center, type);
                    else
                        NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(ammoSource, NPC.Center, type));
                    int type2 = ModContent.ItemType<SecondaryAmmo>();
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Item.NewItem(ammoSource, NPC.Center, type2);
                    else
                        NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(ammoSource, NPC.Center, type2));
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
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 63;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.lifeMax = 180;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/wrench_hit_build_success1");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/dispenser_explode");
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
            TF2Player p = Main.player[npcOwner].GetModPlayer<TF2Player>();
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                NPC.lifeMax = (int)(180 * p.classMultiplier);
                NPC.defense = (int)(30 * p.classMultiplier); ;
            }
            else
            {
                NPC.lifeMax = (int)(180 * npcPower);
                NPC.defense = (int)(30 * npcPower);
            }
            ai += 1;
            ai2 += 1;
            ai3 += 1;
            if (ai >= 90)
            {
                IEntitySource heartSource = NPC.GetSource_FromAI();
                int type = ModContent.ItemType<SmallHealth>();
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Item.NewItem(heartSource, NPC.Center, type);
                else
                    NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(heartSource, NPC.Center, type));
                ai = 0;
            }
            if (ai2 >= 600)
            {
                IEntitySource metalSource = NPC.GetSource_FromAI();
                int type = ModContent.ItemType<Metal>();
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/dispenser_generate_metal"), NPC.Center);
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Item.NewItem(metalSource, NPC.Center, type);
                else
                    NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(metalSource, NPC.Center, type));
                ai2 = 0;
            }
            if (ai3 >= 45)
            {
                for (int i = 0; i < 10; i++)
                {
                    IEntitySource ammoSource = NPC.GetSource_FromAI();
                    int type = ModContent.ItemType<PrimaryAmmo>();
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Item.NewItem(ammoSource, NPC.Center, type);
                    else
                        NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(ammoSource, NPC.Center, type));
                    int type2 = ModContent.ItemType<SecondaryAmmo>();
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Item.NewItem(ammoSource, NPC.Center, type2);
                    else
                        NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(ammoSource, NPC.Center, type2));
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
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 71;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.lifeMax = 216;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/wrench_hit_build_success1");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/dispenser_explode");
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
            TF2Player p = Main.player[npcOwner].GetModPlayer<TF2Player>();
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                NPC.lifeMax = (int)(216 * p.classMultiplier);
                NPC.defense = (int)(40 * p.classMultiplier); ;
            }
            else
            {
                NPC.lifeMax = (int)(216 * npcPower);
                NPC.defense = (int)(40 * npcPower);
            }
            ai += 1;
            ai2 += 1;
            ai3 += 1;
            if (ai >= 60)
            {
                IEntitySource heartSource = NPC.GetSource_FromAI();
                int type = ModContent.ItemType<SmallHealth>();
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Item.NewItem(heartSource, NPC.Center, type);
                else
                    NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(heartSource, NPC.Center, type));
                ai = 0;
            }
            if (ai2 >= 375)
            {
                IEntitySource metalSource = NPC.GetSource_FromAI();
                int type = ModContent.ItemType<Metal>();
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/dispenser_generate_metal"), NPC.Center);
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Item.NewItem(metalSource, NPC.Center, type);
                else
                    NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(metalSource, NPC.Center, type));
                ai2 = 0;
            }
            if (ai3 >= 30)
            {
                for (int i = 0; i < 10; i++)
                {
                    IEntitySource ammoSource = NPC.GetSource_FromAI();
                    int type = ModContent.ItemType<PrimaryAmmo>();
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Item.NewItem(ammoSource, NPC.Center, type);
                    else
                        NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(ammoSource, NPC.Center, type));
                    int type2 = ModContent.ItemType<SecondaryAmmo>();
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Item.NewItem(ammoSource, NPC.Center, type2);
                    else
                        NetMessage.SendData(MessageID.SyncItem, number: Item.NewItem(ammoSource, NPC.Center, type2));
                }
                ai3 = 0;
            }
        }
    }
}