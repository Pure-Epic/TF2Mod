using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using TF2.Content.Items.Buddies;
using TF2.Content.Items.Consumables;
using TF2.Content.Items.Consumables.Bundles;
using TF2.Content.Items.Weapons.Demoman;
using TF2.Content.Items.Weapons.Engineer;
using TF2.Content.Items.Weapons.Heavy;
using TF2.Content.Items.Weapons.Medic;
using TF2.Content.Items.Weapons.MultiClass;
using TF2.Content.Items.Weapons.Pyro;
using TF2.Content.Items.Weapons.Scout;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.Items.Weapons.Soldier;
using TF2.Content.Items.Weapons.Spy;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.NPCs;
using TF2.Content.UI.MannCoStore;

namespace TF2.Content.NPCs.TownNPCs
{
    [AutoloadHead]
    public class SaxtonHale : ModNPC
    {
        public static Dictionary<MannCoStoreCategory, List<MannCoStoreItem>> Inventory = new Dictionary<MannCoStoreCategory, List<MannCoStoreItem>>();
        public readonly MannCoStoreCategory Category;
        private static Asset<Texture2D> spriteSheet;
        public int ai;
        private int attackTime;
        private int direction;

        public override void Load()
        {
            if (!Main.dedServ)
                spriteSheet = ModContent.Request<Texture2D>("TF2/Content/NPCs/TownNPCs/SaxtonHale");
        }

        public override void Unload() => spriteSheet = null;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 1000;
            NPCID.Sets.AttackTime[Type] = 60;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 50;
            NPC.lifeMax = 1000;
            NPC.damage = 100;
            NPC.defense = 100;
            NPC.aiStyle = 7;
            NPC.friendly = true;
            NPC.townNPC = true;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.TF2.NPCs.SaxtonHale.BestiaryEntry"))
            ]);

        public override bool CanTownNPCSpawn(int numTownNPCs) => true;

        public override ITownNPCProfile TownNPCProfile() => new SaxtonHaleProfile();

        public override string GetChat()
        {
            WeightedRandom<string> chat = new();
            chat.Add(Language.GetTextValue("Mods.TF2.NPCs.SaxtonHale.Chat"), 10.0);
            chat.Add(Language.GetTextValue("Mods.TF2.NPCs.SaxtonHale.Chat2"));
            chat.Add(Language.GetTextValue("Mods.TF2.NPCs.SaxtonHale.Chat3"));
            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2) => button = "Mann Co. Store";

        public override void OnChatButtonClicked(bool firstButton, ref string shopName) => TF2.MannCoStore.SetState(new MannCoStoreUI());

        private static void ResetMannCoStore()
        {
            foreach (KeyValuePair<MannCoStoreCategory, List<MannCoStoreItem>> keyValuePair in Inventory)
                Inventory[keyValuePair.Key].Clear();
        }

        public static void CreateMannCoStore()
        {
            ResetMannCoStore();
            AddShopItem(true, MannCoStoreCategory.All, ModContent.ItemType<MannCoSupplyCrateKey>(), 2.49f);
            AddShopItem(true, MannCoStoreCategory.Scout, ModContent.ItemType<ScoutBundle>(), 9.99f);
            AddShopItem(true, MannCoStoreCategory.Soldier, ModContent.ItemType<SoldierBundle>(), 9.99f);
            AddShopItem(true, MannCoStoreCategory.Pyro, ModContent.ItemType<PyroBundle>(), 9.99f);
            AddShopItem(true, MannCoStoreCategory.Demoman, ModContent.ItemType<DemomanBundle>(), 9.99f);
            AddShopItem(true, MannCoStoreCategory.Heavy, ModContent.ItemType<HeavyBundle>(), 9.99f);
            AddShopItem(true, MannCoStoreCategory.Engineer, ModContent.ItemType<EngineerBundle>(), 9.99f);
            AddShopItem(true, MannCoStoreCategory.Medic, ModContent.ItemType<MedicBundle>(), 9.99f);
            AddShopItem(true, MannCoStoreCategory.Sniper, ModContent.ItemType<SniperBundle>(), 9.99f);
            AddShopItem(true, MannCoStoreCategory.Spy, ModContent.ItemType<SpyBundle>(), 9.99f);
            AddShopItem(true, MannCoStoreCategory.All, ModContent.ItemType<SmallAmmoPotion>(), 0.49f);
            AddShopItem(true, MannCoStoreCategory.All, ModContent.ItemType<MediumAmmoPotion>(), 1.49f);
            AddShopItem(true, MannCoStoreCategory.All, ModContent.ItemType<LargeAmmoPotion>(), 1.99f);
            AddShopItem(true, MannCoStoreCategory.All, ModContent.ItemType<SmallHealthPotion>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.All, ModContent.ItemType<MediumHealthPotion>(), 2.99f);
            AddShopItem(true, MannCoStoreCategory.All, ModContent.ItemType<LargeHealthPotion>(), 4.99f);
            AddShopItem(true, MannCoStoreCategory.Scout, ModContent.ItemType<ForceANature>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Scout, ModContent.ItemType<Shortstop>(), 1.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Scout, ModContent.ItemType<SodaPopper>(), 1.99f);
            AddShopItem(true, MannCoStoreCategory.Scout, ModContent.ItemType<BonkAtomicPunch>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Scout, ModContent.ItemType<CritaCola>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Scout, ModContent.ItemType<MadMilk>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Scout, ModContent.ItemType<Winger>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Scout, ModContent.ItemType<Sandman>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Scout, ModContent.ItemType<HolyMackerel>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Scout, ModContent.ItemType<CandyCane>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Scout, ModContent.ItemType<BostonBasher>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Scout, ModContent.ItemType<SunonaStick>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Scout, ModContent.ItemType<FanOWar>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Scout, ModContent.ItemType<Atomizer>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Soldier, ModContent.ItemType<DirectHit>(), 1.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Soldier, ModContent.ItemType<BlackBox>(), 1.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Soldier, ModContent.ItemType<RocketJumper>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Soldier, ModContent.ItemType<LibertyLauncher>(), 1.99f);
            AddShopItem(true, MannCoStoreCategory.Soldier, ModContent.ItemType<BuffBanner>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Soldier, ModContent.ItemType<Gunboats>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Soldier, ModContent.ItemType<BattalionsBackup>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Soldier, ModContent.ItemType<Concheror>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Soldier, ModContent.ItemType<ReserveShooter>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Soldier, ModContent.ItemType<Mantreads>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Soldier, ModContent.ItemType<Equalizer>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Soldier, ModContent.ItemType<PainTrain>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Soldier, ModContent.ItemType<MarketGardener>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Soldier, ModContent.ItemType<DisciplinaryAction>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Pyro, ModContent.ItemType<Backburner>(), 1.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Pyro, ModContent.ItemType<Degreaser>(), 1.99f);
            AddShopItem(true, MannCoStoreCategory.Pyro, ModContent.ItemType<FlareGun>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Pyro, ModContent.ItemType<Detonator>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Pyro, ModContent.ItemType<Axtinguisher>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Pyro, ModContent.ItemType<Homewrecker>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Pyro, ModContent.ItemType<Powerjack>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Pyro, ModContent.ItemType<BackScratcher>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Pyro, ModContent.ItemType<SharpenedVolcanoFragment>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Demoman, ModContent.ItemType<LochnLoad>(), 1.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Demoman, ModContent.ItemType<AliBabasWeeBooties>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Demoman, ModContent.ItemType<ScottishResistance>(), 1.99f);
            AddShopItem(true, MannCoStoreCategory.Demoman, ModContent.ItemType<CharginTarge>(), 1.99f);
            AddShopItem(true, MannCoStoreCategory.Demoman, ModContent.ItemType<SplendidScreen>(), 1.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Demoman, ModContent.ItemType<StickyJumper>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Demoman, ModContent.ItemType<Eyelander>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Demoman, ModContent.ItemType<ScotsmansSkullcutter>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Demoman, ModContent.ItemType<UllapoolCaber>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Demoman, ModContent.ItemType<ClaidheamhMor>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Demoman, ModContent.ItemType<HalfZatoichi>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Demoman, ModContent.ItemType<PersianPersuader>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Heavy, ModContent.ItemType<Natascha>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Heavy, ModContent.ItemType<BrassBeast>(), 1.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Heavy, ModContent.ItemType<Tomislav>(), 1.99f);
            AddShopItem(true, MannCoStoreCategory.Heavy, ModContent.ItemType<Sandvich>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Heavy, ModContent.ItemType<DalokohsBar>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Heavy, ModContent.ItemType<BuffaloSteakSandvich>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Heavy, ModContent.ItemType<FamilyBusiness>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Heavy, ModContent.ItemType<KillingGlovesOfBoxing>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Heavy, ModContent.ItemType<GlovesOfRunningUrgently>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Heavy, ModContent.ItemType<WarriorsSpirit>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Heavy, ModContent.ItemType<FistsOfSteel>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Heavy, ModContent.ItemType<EvictionNotice>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Engineer, ModContent.ItemType<FrontierJustice>(), 1.99f);
            AddShopItem(true, MannCoStoreCategory.Engineer, ModContent.ItemType<Wrangler>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Engineer, ModContent.ItemType<Gunslinger>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Engineer, ModContent.ItemType<SouthernHospitality>(), 1.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Engineer, ModContent.ItemType<Jag>(), 1.99f);
            AddShopItem(true, MannCoStoreCategory.Medic, ModContent.ItemType<Blutsauger>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Medic, ModContent.ItemType<CrusadersCrossbow>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Medic, ModContent.ItemType<Overdose>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Medic, ModContent.ItemType<Kritzkrieg>(), 1.99f);
            AddShopItem(true, MannCoStoreCategory.Medic, ModContent.ItemType<QuickFix>(), 1.99f);
            AddShopItem(true, MannCoStoreCategory.Medic, ModContent.ItemType<Ubersaw>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Medic, ModContent.ItemType<VitaSaw>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Medic, ModContent.ItemType<Amputator>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Medic, ModContent.ItemType<SolemnVow>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Sniper, ModContent.ItemType<Huntsman>(), 1);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Sniper, ModContent.ItemType<SydneySleeper>(), 1.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Sniper, ModContent.ItemType<BazaarBargain>(), 1.99f);
            AddShopItem(true, MannCoStoreCategory.Sniper, ModContent.ItemType<Jarate>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Sniper, ModContent.ItemType<Razorback>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Sniper, ModContent.ItemType<DarwinsDangerShield>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Sniper, ModContent.ItemType<TribalmansShiv>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Sniper, ModContent.ItemType<Bushwacka>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Sniper, ModContent.ItemType<Shahanshah>(), 0.99f);
            AddShopItem(true, MannCoStoreCategory.Spy, ModContent.ItemType<Ambassador>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Spy, ModContent.ItemType<LEtranger>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Spy, ModContent.ItemType<Enforcer>(), 0.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Spy, ModContent.ItemType<YourEternalReward>(), 1.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Spy, ModContent.ItemType<ConniversKunai>(), 1.99f);
            AddShopItem(Main.hardMode, MannCoStoreCategory.Spy, ModContent.ItemType<BigEarner>(), 1.99f);
            AddShopItem(true, MannCoStoreCategory.Spy, ModContent.ItemType<CloakAndDagger>(), 1.99f);
            AddShopItem(true, MannCoStoreCategory.Spy, ModContent.ItemType<DeadRinger>(), 1.99f);
            AddShopItem(true, MannCoStoreCategory.Buddy, ModContent.ItemType<ScoutBuddy>(), 4.99f);
            AddShopItem(true, MannCoStoreCategory.Buddy, ModContent.ItemType<SoldierBuddy>(), 6.99f);
            AddShopItem(true, MannCoStoreCategory.Buddy, ModContent.ItemType<PyroBuddy>(), 6.99f);
            AddShopItem(true, MannCoStoreCategory.Buddy, ModContent.ItemType<DemomanBuddy>(), 5.99f);
            AddShopItem(true, MannCoStoreCategory.Buddy, ModContent.ItemType<HeavyBuddy>(), 9.99f);
            AddShopItem(true, MannCoStoreCategory.Buddy, ModContent.ItemType<EngineerBuddy>(), 9.99f);
            AddShopItem(true, MannCoStoreCategory.Buddy, ModContent.ItemType<MedicBuddy>(), 9.99f);
            AddShopItem(true, MannCoStoreCategory.Buddy, ModContent.ItemType<SniperBuddy>(), 6.99f);
            AddShopItem(true, MannCoStoreCategory.Buddy, ModContent.ItemType<SpyBuddy>(), 5.99f);
            MannCoStoreUI.UpdateItemGrid();
        }

        public static void AddShopItem(bool visible, MannCoStoreCategory category, int item = 0, float cost = 0f, int amount = 1, int item2 = 0, int amount2 = 1)
        {
            if (item == 0) return;
            if (!Inventory.ContainsKey(category))
                Inventory.Add(category, new List<MannCoStoreItem>());
            Item item3 = new Item(item, 1, 0) { stack = amount };
            Item item4 = new Item(item2, 1, 0) { stack = amount2 };
            Inventory[category].Add(new MannCoStoreItem
            {
                Item = item3,
                Item2 = item4,
                Cost = cost,
                Visible = visible
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        { }

        public override bool CanGoToStatue(bool toKingStatue) => true;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (attackTime > 0)
            {
                int height = spriteSheet.Value.Height / 25;
                spriteBatch.Draw(spriteSheet.Value, NPC.position - screenPos, new Rectangle(0, height * 24 - 2, spriteSheet.Value.Width, height), drawColor, 0f, new Vector2(NPC.direction == 1 ? spriteSheet.Value.Width - NPC.width : 0, height - NPC.height), NPC.scale, (SpriteEffects)((NPC.direction == 1) ? 1 : 0), 0f);
                return false;
            }
            else return true;
        }

        public override void AI()
        {
            ai++;
            if (attackTime > 0)
            {
                NPC.spriteDirection = NPC.direction = direction;
                attackTime--;
            }
            NPC.netUpdate = true;
            float distanceFromTarget = NPCID.Sets.DangerDetectRange[Type];
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && ai >= TF2.Time(1) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                foreach (NPC targetNPC in Main.npc)
                {
                    ai = 0;
                    if (targetNPC.CanBeChasedBy() && targetNPC.type != NPCID.TargetDummy)
                    {
                        float between = Vector2.Distance(targetNPC.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNPC.position, targetNPC.width, targetNPC.height);
                        bool closeThroughWall = between < 100f;
                        if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = targetNPC.Center;
                            foundTarget = true;
                        }
                    }
                    NPC.netUpdate = true;
                }
                if (foundTarget)
                {
                    Vector2 shootVel = NPC.DirectionTo(targetCenter);
                    direction = (targetCenter - NPC.Center).X > 0f ? 1 : -1;
                    float speed = 10f;
                    int type = ModContent.ProjectileType<KnifeProjectileNPC>();
                    int damage = NPC.damage;
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/melee_swing"), NPC.Center);
                    if ((targetCenter - NPC.Center).Y >= 0f)
                        NPC.velocity = new Vector2(25f * direction, 15f);
                    if ((targetCenter - NPC.Center).Y <= 0f)
                        NPC.velocity = new Vector2(25f * direction, -15f);
                    TF2Projectile projectile = TF2.CreateProjectile(null, projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    KnifeProjectileNPC spawnedModProjectile = (KnifeProjectileNPC)projectile;
                    spawnedModProjectile.thisNPC = NPC;
                    NetMessage.SendData(MessageID.SyncProjectile, number: projectile.Projectile.whoAmI);
                    NPC.spriteDirection = NPC.direction = NPC.velocity.X >= 0f ? 1 : -1;
                    attackTime += TF2.Time(1);
                    NPC.netUpdate = true;
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ai);
            writer.Write(attackTime);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            reader.ReadInt32();
            reader.ReadInt32();
        }

        public override bool UsesPartyHat() => false;
    }

    public class SaxtonHaleProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;

        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
        {
            if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
                return ModContent.Request<Texture2D>("TF2/Content/NPCs/TownNPCs/SaxtonHale");

            if (npc.altTexture == 1)
                return ModContent.Request<Texture2D>("TF2/Content/NPCs/TownNPCs/SaxtonHale");

            return ModContent.Request<Texture2D>("TF2/Content/NPCs/TownNPCs/SaxtonHale");
        }

        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("TF2/Content/NPCs/TownNPCs/SaxtonHale_Head");
    }

    public class MannCoStoreItem
    {
        public Item Item { get; set; }

        public Item Item2 { get; set; }

        public float Cost { get; set; }

        public bool Visible { get; set; }
    }

    public enum MannCoStoreCategory
    {
        All,
        Scout,
        Soldier,
        Pyro,
        Demoman,
        Heavy,
        Engineer,
        Medic,
        Sniper,
        Spy,
        Buddy
    }
}