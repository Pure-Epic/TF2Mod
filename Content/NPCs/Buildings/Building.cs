using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.Items.Consumables;
using TF2.Content.NPCs.Buildings.Dispenser;
using TF2.Content.NPCs.Buildings.SentryGun;
using TF2.Content.NPCs.Buildings.Teleporter;
using static TF2.TF2;

namespace TF2.Content.NPCs.Buildings
{
    public abstract class Building : ModNPC
    {
        internal int Timer
        {
            get => (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public int Metal
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        public float UpgradeCooldown
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        public int Owner
        {
            get => (int)NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        internal int Timer2
        {
            get => (int)NPC.localAI[0];
            set => NPC.localAI[0] = value;
        }

        public virtual string BuildingName => NPC.TypeName;

        protected virtual Asset<Texture2D> BuildingTexture => null;

        protected virtual Asset<Texture2D> BuildingSecondaryTexture => null;

        protected virtual Asset<Texture2D> BuildingTextureAir => null;

        protected virtual Asset<Texture2D> BuildingSecondaryTextureAir => null;

        protected Player Player => Main.player[Owner];

        public bool Initialized { get; protected set; }

        public virtual int InitialHealth => 1;

        public virtual int BuildingCooldown => Time(10);

        public virtual int BuildingCooldownHauled => BuildingCooldown;

        protected virtual int ScrapMetalAmount => 0;

        internal float healthMultiplier;
        internal int preConstructedDamage;
        internal bool hauled;
        internal bool air;
        internal int frame;
        internal float constructionSpeed = 1f;

        protected virtual void BuildingSpawn()
        { }

        protected virtual void BuildingDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        { }

        protected virtual void BuildingAI()
        { }

        protected virtual void BuildingDestroy()
        { }

        protected virtual void BuildingSendExtraAI(BinaryWriter writer)
        { }

        protected virtual void BuildingReceiveExtraAI(BinaryReader binaryReader)
        { }

        public static Building ConstructBuilding(int type = -1, int owner = 0, int direction = 1, int metal = 0, bool isAir = false)
        {
            Player player = Main.player[owner];
            NPC npc = NPC.NewNPCDirect(player.GetSource_FromThis(), (int)player.Center.X, (int)player.Center.Y, type, 0, 0, metal, 0, owner);
            SpawnNPCMultiplayer(player, npc, type);
            npc.Bottom = player.Bottom;
            npc.direction = direction;
            Building building = npc.ModNPC as Building;
            if (!Main.dedServ)
            {
                if (building is TF2Sentry)
                    building.air = isAir;
                if (building is TF2Sentry)
                    player.GetModPlayer<TF2Player>().sentryWhoAmI = npc.whoAmI;
                else if (building is TF2Dispenser)
                    player.GetModPlayer<TF2Player>().dispenserWhoAmI = npc.whoAmI;
                else if (building is TeleporterEntrance)
                    player.GetModPlayer<TF2Player>().teleporterEntranceWhoAmI = npc.whoAmI;
                else if (building is TeleporterExit)
                    player.GetModPlayer<TF2Player>().teleporterExitWhoAmI = npc.whoAmI;
            }
            else
                building.ConstuctBuildingMultiplayer(isAir);
            return building;
        }

        protected void UpgradeBuilding(int upgradedNPC)
        {
            if (Metal >= 200)
            {
                Metal -= 200;
                int metal = Metal;
                if (this is TF2Sentry sentry)
                {
                    int ammo = sentry.Ammo;
                    bool isAir = air;
                    NPC.Transform(upgradedNPC);
                    NPC.ModNPC.OnSpawn(NPC.GetSource_FromAI());
                    Metal = metal;
                    sentry.Ammo = ammo;
                    sentry.air = isAir;
                    if (SoundEngine.TryGetActiveSound(sentry.sentrySoundSlot, out var scanSound))
                        scanSound?.Stop();
                }
                else if (this is TF2Dispenser dispenser)
                {
                    int reservedMetal = dispenser.ReservedMetal;
                    NPC.Transform(upgradedNPC);
                    NPC.ModNPC.OnSpawn(NPC.GetSource_FromAI());
                    Metal = metal;
                    dispenser.ReservedMetal = reservedMetal;
                }
                else if (this is TF2Teleporter teleporter)
                {
                    NPC.Transform(upgradedNPC);
                    NPC.ModNPC.OnSpawn(NPC.GetSource_FromAI());
                    Metal = metal;
                    if (teleporter is TeleporterEntrance)
                    {
                        (teleporter as TeleporterEntrance).FindTeleporterExit(out TeleporterExit exit);
                        if (exit != null && SoundEngine.TryGetActiveSound(exit.teleporterSoundSlot, out var exitSound))
                            exitSound?.Stop();
                        if (SoundEngine.TryGetActiveSound(teleporter.teleporterSoundSlot, out var entranceSound))
                            entranceSound?.Stop();
                    }
                    else if (teleporter is TeleporterExit)
                    {
                        (teleporter as TeleporterExit).FindTeleporterEntrance(out TeleporterEntrance entrance);
                        if (entrance != null && SoundEngine.TryGetActiveSound(entrance.teleporterSoundSlot, out var entranceSound))
                            entranceSound?.Stop();
                        if (SoundEngine.TryGetActiveSound(teleporter.teleporterSoundSlot, out var exitSound))
                            exitSound?.Stop();
                    }
                }
            }
        }

        public static bool BaseLevel(Building building) => building.NPC.type == ModContent.NPCType<SentryLevel1>() || building.NPC.type == ModContent.NPCType<MiniSentry>() || building.NPC.type == ModContent.NPCType<DispenserLevel1>() || building.NPC.type == ModContent.NPCType<TeleporterEntranceLevel1>() || building.NPC.type == ModContent.NPCType<TeleporterExitLevel1>();

        public static bool MaxLevel(Building building) => building.NPC.type == ModContent.NPCType<SentryLevel3>() || building.NPC.type == ModContent.NPCType<MiniSentry>() || building.NPC.type == ModContent.NPCType<DispenserLevel3>() || building.NPC.type == ModContent.NPCType<TeleporterEntranceLevel3>() || building.NPC.type == ModContent.NPCType<TeleporterExitLevel3>();

        public override bool CheckActive() => false;

        public override void OnSpawn(IEntitySource source)
        {
            healthMultiplier = Player.GetModPlayer<TF2Player>().healthMultiplier;
            NPC.life = InitialHealth;
            NPC.lifeMax = Round(NPC.lifeMax * healthMultiplier);
            BuildingSpawn();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D sprite = !air ? BuildingTexture.Value : BuildingTextureAir.Value;
            int height = sprite.Height / Main.npcFrameCount[NPC.type];
            spriteBatch.Draw(sprite, NPC.position - screenPos, new Rectangle(0, height * frame, sprite.Width, height), drawColor, 0f, new Vector2(NPC.direction == 1 ? sprite.Width - NPC.width : 0, height - NPC.height), NPC.scale, (SpriteEffects)((NPC.direction == 1) ? 1 : 0), 0f);
            BuildingDraw(spriteBatch, screenPos, drawColor);
            return false;
        }

        public override void AI() => BuildingAI();

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (!Initialized)
            {
                preConstructedDamage = damageDone;
                NPC.netUpdate = true;
            }
        }

        public override void OnKill()
        {
            BuildingDestroy();
            if (ScrapMetalAmount > 0)
                CreateScrapMetal(NPC, ScrapMetalAmount);
        }

        public static void CreateScrapMetal(NPC npc, int amount)
        {
            BuildingPoint scrapMetal = Main.item[Item.NewItem(npc.GetSource_Loot(), (int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<BuildingPoint>())].ModItem as BuildingPoint;
            scrapMetal.metal = amount;
            if (Main.dedServ)
                NetMessage.SendData(MessageID.SyncItem, number: scrapMetal.Item.whoAmI);
        }

        public void Repair(int amount)
        {
            if (amount > 0)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    NPC.life += amount;
                    NPC.HealEffect(amount);
                }
                else
                {
                    ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
                    packet.Write((byte)MessageType.RepairBuilding);
                    packet.Write((byte)NPC.whoAmI);
                    packet.Write(amount);
                    packet.Send(-1, Main.myPlayer);
                }
            }
        }

        public void SyncBuilding()
        {
            if (Main.netMode == NetmodeID.SinglePlayer) return;
            ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
            packet.Write((byte)MessageType.SyncBuilding);
            packet.Write((byte)NPC.whoAmI);
            packet.Write(Timer);
            packet.Write(Metal);
            packet.Write(UpgradeCooldown);
            packet.Write(Timer2);
            packet.Write(preConstructedDamage);
            packet.Write(constructionSpeed);
            packet.Send(-1, Main.myPlayer);
        }

        public void ConstuctBuildingMultiplayer(bool isAir)
        {
            if (Main.netMode == NetmodeID.SinglePlayer) return;
            ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
            packet.Write((byte)MessageType.ConstructBuilding);
            packet.Write((byte)NPC.whoAmI);
            packet.Write(isAir);
            packet.Send(-1, Main.myPlayer);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Timer2);
            writer.Write(Initialized);
            writer.Write(healthMultiplier);
            writer.Write(preConstructedDamage);
            writer.Write(hauled);
            writer.Write(air);
            writer.Write(frame);
            writer.Write(constructionSpeed);
            BuildingSendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Timer2 = reader.ReadInt32();
            Initialized = reader.ReadBoolean();
            healthMultiplier = reader.ReadSingle();
            preConstructedDamage = reader.ReadInt32();
            hauled = reader.ReadBoolean();
            air = reader.ReadBoolean();
            frame = reader.ReadInt32();
            constructionSpeed = reader.ReadSingle();
            BuildingReceiveExtraAI(reader);
        }
    }

    public class BuildingPoint : SmallAmmoBox
    {
        public override string Texture => "TF2/Content/Items/Consumables/SmallAmmoPoint";

        public int metal;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.White;
            Item.ResearchUnlockCount = 0;
        }

        public override void GrabRange(Player player, ref int grabRange) => grabRange = 2500;

        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup"), player.Center);
            if (player.GetModPlayer<TF2Player>().currentClass == TF2Item.Engineer)
                player.GetModPlayer<TF2Player>().metal += metal;
            Item.TurnToAir();
            return true;
        }

        public override void NetSend(BinaryWriter writer) => writer.Write(metal);

        public override void NetReceive(BinaryReader reader) => metal = reader.ReadInt32();
    }

    public class BuildingTextures : ModSystem
    {
        public static Asset<Texture2D> SentryLevel1Texture => buildingTextures[0];

        public static Asset<Texture2D> SentryLevel1AirTexture => buildingTextures[1];

        public static Asset<Texture2D> SentryLevel2Texture => buildingTextures[2];

        public static Asset<Texture2D> SentryLevel2AirTexture => buildingTextures[3];

        public static Asset<Texture2D> SentryLevel3Texture => buildingTextures[4];

        public static Asset<Texture2D> SentryLevel3AirTexture => buildingTextures[5];

        public static Asset<Texture2D> SentryLevel3SecondaryTexture => buildingTextures[6];

        public static Asset<Texture2D> SentryLevel3SecondaryAirTexture => buildingTextures[7];

        public static Asset<Texture2D> MiniSentryTexture => buildingTextures[8];

        public static Asset<Texture2D> MiniSentryAirTexture => buildingTextures[9];

        public static Asset<Texture2D> MiniSentrySecondaryTexture => buildingTextures[10];

        public static Asset<Texture2D> MiniSentrySecondaryAirTexture => buildingTextures[11];

        public static Asset<Texture2D> DispenserLevel1Texture => buildingTextures[12];

        public static Asset<Texture2D> DispenserLevel2Texture => buildingTextures[13];

        public static Asset<Texture2D> DispenserLevel3Texture => buildingTextures[14];

        public static Asset<Texture2D> TeleporterTexture => buildingTextures[15];

        internal static Asset<Texture2D>[] buildingTextures;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                buildingTextures =
                [
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/SentryGun/SentryLevel1"),
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/SentryGun/SentryLevel1_Air"),
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/SentryGun/SentryLevel2"),
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/SentryGun/SentryLevel2_Air"),
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/SentryGun/SentryLevel3"),
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/SentryGun/SentryLevel3_Air"),
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/SentryGun/SentryLevel3_Rockets"),
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/SentryGun/SentryLevel3_Rockets_Air"),
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/SentryGun/MiniSentry"),
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/SentryGun/MiniSentry_Air"),
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/SentryGun/MiniSentry_Glowmask"),
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/SentryGun/MiniSentry_Glowmask_Air"),
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/Dispenser/DispenserLevel1"),
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/Dispenser/DispenserLevel2"),
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/Dispenser/DispenserLevel3"),
                    ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/Teleporter/Teleporter")
                ];
            }
        }
    }
}