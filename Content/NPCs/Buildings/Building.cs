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

        protected virtual string BuildingTexture => "";

        protected virtual string BuildingSecondaryTexture => "";

        protected Player Player => Main.player[Owner];

        public bool Initialized { get; protected set; }

        public virtual int InitialHealth => 1;

        public virtual int BuildingCooldown => TF2.Time(10);

        public virtual int BuildingCooldownHauled => BuildingCooldown;

        protected virtual int ScrapMetalAmount => 0;

        protected Asset<Texture2D> spriteSheet;
        protected Asset<Texture2D> spriteSheet2;
        protected Asset<Texture2D> spriteSheetAir;
        protected Asset<Texture2D> spriteSheetAir2;
        public int preConstructedDamage;
        public bool hauled;
        protected bool air;
        public int frame;
        public float constructionSpeed = 1f;

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

        public static Building ConstructBuilding(int npc = -1, int owner = 0, int direction = 1, int metal = 0, bool hauled = false, bool isAir = false)
        {
            Player player = Main.player[owner];
            int i = NPC.NewNPC(player.GetSource_FromThis(), (int)player.Center.X, (int)player.Center.Y, npc, 0, 0, metal, 0, owner);
            Main.npc[i].Bottom = player.Bottom;
            Main.npc[i].direction = direction;
            Building building = (Building)Main.npc[i].ModNPC;
            building.hauled = hauled;
            if (building.hauled)
                building.UpgradeCooldown = building.BuildingCooldownHauled;
            if (building is TF2Sentry)
                building.air = isAir;
            Main.npc[i].netUpdate = true;
            if (building is TF2Sentry)
                player.GetModPlayer<TF2Player>().sentryWhoAmI = i;
            else if (building is TF2Dispenser)
                player.GetModPlayer<TF2Player>().dispenserWhoAmI = i;
            else if (building is TeleporterEntrance)
                player.GetModPlayer<TF2Player>().teleporterEntranceWhoAmI = i;
            else if (building is TeleporterExit)
                player.GetModPlayer<TF2Player>().teleporterExitWhoAmI = i;
            return building;
        }

        protected void UpgradeBuilding(int upgradedNPC)
        {
            if (Metal >= 200)
            {
                Metal -= 200;
                Player player = Main.player[Owner];
                int i = NPC.NewNPC(player.GetSource_FromThis(), (int)NPC.position.X, (int)NPC.position.Y, upgradedNPC, 0, 0, Metal, 0, Owner);
                Main.npc[i].Bottom = NPC.Bottom;
                Main.npc[i].direction = NPC.direction;
                Building building = (Building)Main.npc[i].ModNPC;
                if (building is TF2Sentry)
                    building.air = air;
                if (building is TF2Teleporter teleporter)
                {
                    building.hauled = hauled;
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
                building.Metal = Metal;
                if (this is TF2Sentry sentry && building is TF2Sentry newSentry)
                    newSentry.Ammo = sentry.Ammo;
                Main.npc[i].netUpdate = true;
                if (building is TF2Sentry)
                    player.GetModPlayer<TF2Player>().sentryWhoAmI = i;
                else if (building is TF2Dispenser)
                    player.GetModPlayer<TF2Player>().dispenserWhoAmI = i;
                else if (building is TeleporterEntrance)
                    player.GetModPlayer<TF2Player>().teleporterEntranceWhoAmI = i;
                else if (building is TeleporterExit)
                    player.GetModPlayer<TF2Player>().teleporterExitWhoAmI = i;
                NPC.active = false;
                NPC.netUpdate = true;
            }
        }

        public static bool BaseLevel(Building building) => building.NPC.type == ModContent.NPCType<SentryLevel1>() || building.NPC.type == ModContent.NPCType<MiniSentry>() || building.NPC.type == ModContent.NPCType<DispenserLevel1>() || building.NPC.type == ModContent.NPCType<TeleporterEntranceLevel1>() || building.NPC.type == ModContent.NPCType<TeleporterExitLevel1>();

        public static bool MaxLevel(Building building) => building.NPC.type == ModContent.NPCType<SentryLevel3>() || building.NPC.type == ModContent.NPCType<MiniSentry>() || building.NPC.type == ModContent.NPCType<DispenserLevel3>() || building.NPC.type == ModContent.NPCType<TeleporterEntranceLevel3>() || building.NPC.type == ModContent.NPCType<TeleporterExitLevel3>();

        public override bool CheckActive() => false;

        public override void OnSpawn(IEntitySource source)
        {
            spriteSheet = ModContent.Request<Texture2D>(BuildingTexture);
            if (BuildingSecondaryTexture != "" && this is TF2Sentry)
                spriteSheet2 = ModContent.Request<Texture2D>(BuildingSecondaryTexture);
            if (this is TF2Sentry)
            {
                spriteSheetAir = ModContent.Request<Texture2D>(BuildingTexture + "_Air");
                if (BuildingSecondaryTexture != "")
                    spriteSheetAir2 = ModContent.Request<Texture2D>(BuildingSecondaryTexture + "_Air");
            }
            NPC.life = InitialHealth;
            NPC.lifeMax = TF2.Round(NPC.lifeMax * Player.GetModPlayer<TF2Player>().healthMultiplier);
            BuildingSpawn();
            NPC.netUpdate = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D sprite = !air ? spriteSheet.Value : spriteSheetAir.Value;
            int height = sprite.Height / Main.npcFrameCount[NPC.type];
            spriteBatch.Draw(sprite, NPC.position - screenPos, new Rectangle(0, height * frame, sprite.Width, height), drawColor, 0f, new Vector2(NPC.direction == 1 ? sprite.Width - NPC.width : 0, height - NPC.height), NPC.scale, (SpriteEffects)((NPC.direction == 1) ? 1 : 0), 0f);
            BuildingDraw(spriteBatch, screenPos, drawColor);
            return false;
        }

        public override void AI() => BuildingAI();

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            preConstructedDamage = damageDone;
            NPC.netUpdate = true;
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
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncItem, number: scrapMetal.Item.whoAmI);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Timer2);
            writer.Write(Initialized);
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
    }
}