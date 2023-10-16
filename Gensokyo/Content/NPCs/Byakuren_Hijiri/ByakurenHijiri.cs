using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items.Consumables;
using TF2.Content.Projectiles;
using TF2.Gensokyo.Common;
using TF2.Gensokyo.Content.Items.Consumables;
using TF2.Gensokyo.Content.Items.Pyro;
using TF2.Gensokyo.Content.Items.Scout;
using TF2.Gensokyo.Content.Items.Sniper;
using TF2.Gensokyo.Content.Items.Soldier;
using TF2.Gensokyo.Content.Items.Spy;
using TF2.Gensokyo.Content.Projectiles.NPCs.Byakuren_Hijiri;

namespace TF2.Gensokyo.Content.NPCs.Byakuren_Hijiri
{
    [ExtendsFromMod("Gensokyo")]
    [AutoloadBossHead]
    public class ByakurenHijiri : GensokyoBoss
    {

        public new int State
        {
            get => base.State;
            set => base.State = value;
        }

        public new int Phase
        {
            get => base.Phase;
            set => base.Phase = value;
        }

        public new int Stage => base.Stage;

        public static int GetBasicAttackPhase => Phase_BasicAttackPhase;

        public static int SpellcardAttackPhase => Phase_DefaultStageAttack;

        private const int Phase_BasicAttackPhase = 100;

        private static Asset<Texture2D> spriteSheet;
        private int horizontalFrame;
        private int verticalFrame;
        private int[] BasicAttackDuration;
        private bool startTimer;
        private int burstCounter;
        private int burstDirection = 1;
        private float angleOffset;
        private bool maxOffset;
        public Vector2 targetCenter;
        private int preOmenofPurpleCloudsCounter;
        private bool usedBasicAttack;

        public override void Load()
        {
            if (!Main.dedServ)
                spriteSheet = ModContent.Request<Texture2D>("TF2/Gensokyo/Content/NPCs/Byakuren_Hijiri/ByakurenHijiri_Spritesheet", AssetRequestMode.AsyncLoad);
        }

        public override void Unload() => spriteSheet = null;

        public override void FindFrame(int frameHeight)
        {
            NPC npc = NPC;
            double index = npc.frameCounter + 1.0;
            npc.frameCounter = index;
            if (index >= 12.0)
            {
                NPC.frameCounter = 0.0;
                int index2 = horizontalFrame + 1;
                horizontalFrame = index2;
                if (index2 >= 4)
                {
                    horizontalFrame = 0;
                }
            }
            verticalFrame = Stage == 6 ? 1 : 0;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D sprite = spriteSheet.Value;
            int width = sprite.Width / 4;
            int height = sprite.Height / 2;
            float frameWidth = NPC.direction == 1 ? width - NPC.width : 0;
            float frameHeight = height - NPC.height;
            spriteBatch.Draw(sprite, NPC.position - screenPos, new Rectangle?(new Rectangle(horizontalFrame * width, verticalFrame * height, width, height)), Color.Lerp(drawColor, Color.White, 0.3f), 0f, new Vector2(frameWidth, frameHeight), NPC.scale, (SpriteEffects)((NPC.direction == 1) ? 1 : 0), 0f);
            return false;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 50;
            NPC.damage = 100;
            NPC.defense = 125; // It's recommended that you should use TF2 weapons.
            NPC.lifeMax = 350000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
            NPC.value = Item.buyPrice(platinum: 1); // Touhou girl for only one platinum?
            NPC.boss = true;
            NPC.npcSlots = 25f; // Take up open spawn slots, preventing random NPCs from spawning during the fight
            NPC.aiStyle = -1; // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
            Tier = 8;

            // Custom boss bar
            NPC.BossBar = ModContent.GetInstance<GensokyoBossHealthBar>();

            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Gensokyo/Content/Sounds/Music/byakurentheme");
                SceneEffectPriority = (SceneEffectPriority)9;
            }

            MovespeedMax = 100f;
            NumStages = 6;
            MovePhaseDuration = new int[]
            {
                60,
                240,
                120,
                300,
                120,
                120
            };
            BasicAttackDuration = new int[]
            {
                120,
                1200,
                360,
                1200,
                0,
                0
            };
            AttackPhaseDuration = new int[]
            {
                240,
                900,
                1000,
                1920,
                600,
                3000
            };
            enableRevengeanceDamageResistance = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Initially seeking eternal youth, the Buddhist nun Byakuren Hijiri protected youkai and tried bringing them harmony with humankind. Viewed as a traitor to humans, she was then imprisoned to Makai. Eventually, the youkai freed their savior and she is now the chief priest of the Myouren Temple.")
            });
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
                bool revengeance = (bool)calamity.Call("GetDifficultyActive", "revengeance");
                if (Main.expertMode && !Main.masterMode && !revengeance)
                {
                    NPC.lifeMax = 500000;
                    NPC.defense += 25;
                }
                else if (Main.expertMode && revengeance && !Main.masterMode)
                {
                    NPC.lifeMax = 600000;
                    NPC.defense += 35;
                    damageScale = 1.25f;
                }
                else if (Main.masterMode && !revengeance)
                {
                    NPC.lifeMax = 750000;
                    NPC.defense += 50;
                }
                else if (Main.masterMode && revengeance)
                {
                    NPC.lifeMax = 900000;
                    NPC.defense += 65;
                    damageScale = 1.25f;
                }
            }
            else
            {
                if (Main.expertMode && !Main.masterMode)
                {
                    NPC.lifeMax = 500000;
                    NPC.defense += 25;
                }
                else if (Main.masterMode)
                {
                    NPC.lifeMax = 750000;
                    NPC.defense += 50;
                }
            }
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            /*
            if (CheckFinalSpellCardDamageResistance())
                modifiers.SourceDamage.Base *= 0.8f;
            if (Overkill())
                modifiers.SourceDamage.Base *= 0.2f;
            else if (attackType == 0 && SpellCard == 0 && preOmenofPurpleCloudsCounter < 5)
                modifiers.SourceDamage.Base *= 0.2f;
            else if (!usedNonSpellCardDanmaku && attackType == 0 && SpellCard > 0)
                modifiers.SourceDamage.Base *= 0.2f;
            */
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            /*
            if (projectile.Name == "Boss Tester_ Projectile")
                return;
            if (CheckFinalSpellCardDamageResistance())
                modifiers.SourceDamage.Base *= 0.8f;
            if (Overkill())
                modifiers.SourceDamage.Base *= 0.2f;
            else if (attackType == 0 && SpellCard == 0 && preOmenofPurpleCloudsCounter < 5)
                modifiers.SourceDamage.Base *= 0.2f;
            else if (!usedNonSpellCardDanmaku && attackType == 0 && SpellCard > 0)
                modifiers.SourceDamage.Base *= 0.2f;
            */
        }

        protected override void Decision()
        {
            if (NewStage)
                usedBasicAttack = false;

            if (JustSpawned)
            {
                PhaseQueue.Add(Phase_SpawnMove);
                PhaseQueue.Add(SwitchAttack() ? Phase_DefaultStageAttack : Phase_BasicAttackPhase);
            }

            if (NewStage && !JustSpawned)
            {
                MoveRetryCounter = 0;
                PhaseQueue.Clear();
            }

            if (PhaseQueue.Count == 0)
            {
                PhaseQueue.Add(Phase_DefaultStageMove);
                PhaseQueue.Add(SwitchAttack() ? Phase_DefaultStageAttack : Phase_BasicAttackPhase);
            }

            if (NewStage && NPC.velocity.Length() > 0f)
                PhaseQueue.Insert(0, Phase_NewStageSlowdown);

            NewStage = false;
            JustSpawned = false;
        }

        protected override bool HasSpellCardAnnouncement() => usedBasicAttack;

        protected override void AnnounceSpellCard()
        {
            string text;
            switch (Stage)
            {
                case 0:
                    text = "Magic \"Omen of Purple Clouds\"";
                    break;

                case 1:
                    text = "Magic \"Mystic Fragrance of a Makai Butterfly\"";
                    break;

                case 2:
                    text = "Light Magic \"Star Maelstrom\"";
                    break;

                case 3:
                    text = "Great Magic \"Devil's Recitation\"";
                    break;

                case 4:
                    text = "\"Amagimi Hijiri's Air Scroll\"";
                    break;

                case 5:
                    text = "Flying Bowl \"Flying Fantastica\"";
                    break;

                default:
                    return;
            }
            Main.NewText(text, Color.DarkMagenta);
        }

        protected override int GetAttackPhaseDuration() => State == State_Attack && Phase != Phase_BasicAttackPhase ? AttackPhaseDuration[Stage] : BasicAttackDuration[Stage];

        private bool SwitchAttack()
        {
            if (usedBasicAttack && Stage >= 0 && Stage <= 3 && !Overkill())
                return true;
            else if (Stage >= 4)
                return true;
            return false;
        }

        /*
        public override void BasicAttack()
        {
            switch (SpellCard)
            {
                case 0:
                    PreOmenofPurpleClouds();
                    return;

                case 1:
                    PreMysticFragranceofaMakaiButterfly();
                    return;

                case 2:
                    PreStarMaelstrom();
                    return;

                case 3:
                    PreDevilsRecitation();
                    return;

                case 4:
                    return;

                case 5:
                    return;

                default:
                    return;
            }
        }
        */

        protected override bool HasAttack()
        {
            return Phase switch
            {
                Phase_BasicAttackPhase => true,
                Phase_DefaultStageAttack => true,
                _ => base.HasAttack(),
            };
        }

        protected override void Attack()
        {
            switch (Stage)
            {
                case 0:
                    if (Phase == Phase_BasicAttackPhase)
                        PreOmenofPurpleClouds();
                    else
                        OmenofPurpleClouds();
                    return;

                case 1:
                    if (Phase == Phase_BasicAttackPhase)
                        PreMysticFragranceofaMakaiButterfly();
                    else
                        MysticFragranceofaMakaiButterfly();
                    return;

                case 2:
                    if (Phase == Phase_BasicAttackPhase)
                        PreStarMaelstrom();
                    else
                        StarMaelstrom();
                    return;

                case 3:
                    if (Phase == Phase_BasicAttackPhase)
                        PreDevilsRecitation();
                    else
                        DevilsRecitation();
                    return;

                case 4:
                    AmagimiHijirisAirScroll();
                    return;

                case 5:
                    FlyingFantastica();
                    return;

                default:
                    return;
            }
        }

        #region Attacks

        private void PreOmenofPurpleClouds()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            int damage = (int)(30 * damageScale);
            if (Timer % 10 == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/shoot"), NPC.Center);
                for (int i = 0; i < 14; i++)
                {
                    Vector2 velocity = Utils.RotatedBy(Vector2.UnitX, MathHelper.ToRadians(i * 25.7142857143f + angleOffset)) * 10f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<PreOmenofPurpleClouds1>(), damage, 0f, NPC.target);
                }
                for (int i = 0; i < 14; i++)
                {
                    Vector2 velocity = Utils.RotatedBy(Vector2.UnitX, MathHelper.ToRadians(i * 25.7142857143f - angleOffset)) * 12.5f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<PreOmenofPurpleClouds2>(), damage, 0f, NPC.target);
                }
                for (int i = 0; i < 14; i++)
                {
                    Vector2 velocity = Utils.RotatedBy(Vector2.UnitX, MathHelper.ToRadians(i * 25.7142857143f)) * 10f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<PreOmenofPurpleClouds3>(), damage, 0f, NPC.target);
                }
                angleOffset += 5f;
            }
        }

        private void PreMysticFragranceofaMakaiButterfly()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            int damage = (int)(30 * damageScale);
            if (Timer == 0)
                CreateWings();
            if (Timer % 15 == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/shoot"), NPC.Center);
                for (int i = 0; i < 12; i++)
                {
                    Vector2 velocity = Utils.RotatedBy(Vector2.UnitX, MathHelper.ToRadians(i * 15f + angleOffset)) * 10f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<PreMysticFragranceofaMakaiButterfly1>(), damage, 0f, NPC.target);
                }
                AngleOffset(-45f, 52.5f, 7.5f);
            }
        }

        private void PreStarMaelstrom()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            int damage = (int)(30 * damageScale);
            if (Timer == 0)
                CreateWings();
            if (Timer % 2 == 0)
                SoundEngine.PlaySound(SoundID.Item9, NPC.Center);
            if (Timer % 60 == 0)
            {
                int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX * 15f, ModContent.ProjectileType<PreStarMaelstrom1>(), damage, 0f, NPC.target);
                Main.projectile[projectile].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile);

                int projectile2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX, ModContent.ProjectileType<PreStarMaelstrom2>(), damage, 0f, NPC.target);
                Main.projectile[projectile2].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                PreStarMaelstrom2 butterflyWing = (PreStarMaelstrom2)Main.projectile[projectile2].ModProjectile;
                butterflyWing.center = NPC.Center + new Vector2(-200f, -200f);
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile2);

                int projectile3 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX, ModContent.ProjectileType<PreStarMaelstrom2>(), damage, 0f, NPC.target);
                Main.projectile[projectile3].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                PreStarMaelstrom2 butterflyWing2 = (PreStarMaelstrom2)Main.projectile[projectile3].ModProjectile;
                butterflyWing2.center = NPC.Center + new Vector2(200f, -200f);
                butterflyWing2.ProjectileAI = 1;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile3);

                int projectile4 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX, ModContent.ProjectileType<PreStarMaelstrom3>(), damage, 0f, NPC.target);
                Main.projectile[projectile4].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                PreStarMaelstrom3 butterflyWing3 = (PreStarMaelstrom3)Main.projectile[projectile4].ModProjectile;
                butterflyWing3.center = NPC.Center + new Vector2(-500f, 375f);
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile4);

                int projectile5 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX, ModContent.ProjectileType<PreStarMaelstrom3>(), damage, 0f, NPC.target);
                Main.projectile[projectile5].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                PreStarMaelstrom3 butterflyWing4 = (PreStarMaelstrom3)Main.projectile[projectile5].ModProjectile;
                butterflyWing4.center = NPC.Center + new Vector2(500f, 375f);
                butterflyWing4.ProjectileAI = 1;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile5);
            }
        }

        private void PreDevilsRecitation()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            if (Timer == 0)
                CreateWings();
        }

        private void OmenofPurpleClouds()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            int damage = (int)(50 * damageScale);
            if (Timer % 5 == 0)
                SoundEngine.PlaySound(SoundID.Item9, NPC.Center);
            if (Timer == 0)
            {
                int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX * 15f, ModContent.ProjectileType<OmenofPurpleClouds1>(), damage, 0f, NPC.target);
                Main.projectile[projectile].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile);

                int projectile2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX * 15f, ModContent.ProjectileType<OmenofPurpleClouds1>(), damage, 0f, NPC.target);
                Main.projectile[projectile2].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                OmenofPurpleClouds1 omenofPurpleClouds1 = (OmenofPurpleClouds1)Main.projectile[projectile2].ModProjectile;
                omenofPurpleClouds1.ProjectileAI = 1;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile2);

                int projectile3 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX * 15f, ModContent.ProjectileType<OmenofPurpleClouds3>(), damage, 0f, NPC.target);
                Main.projectile[projectile3].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile3);

                int projectile4 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX * 15f, ModContent.ProjectileType<OmenofPurpleClouds3>(), damage, 0f, NPC.target);
                Main.projectile[projectile4].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                OmenofPurpleClouds3 omenofPurpleClouds3 = (OmenofPurpleClouds3)Main.projectile[projectile4].ModProjectile;
                omenofPurpleClouds3.ProjectileAI = 1;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile4);
            }
        }

        private void MysticFragranceofaMakaiButterfly()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            int damage = (int)(35 * damageScale);
            if (Timer == 0)
            {
                CreateWings();

                int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX, ModContent.ProjectileType<MysticFragranceofaMakaiButterfly1>(), damage, 0f, NPC.target);
                Main.projectile[projectile].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                MysticFragranceofaMakaiButterfly1 makaiButterflyWing = (MysticFragranceofaMakaiButterfly1)Main.projectile[projectile].ModProjectile;
                makaiButterflyWing.center = NPC.Center + new Vector2(-200f, -200f);
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile);

                int projectile2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX, ModContent.ProjectileType<MysticFragranceofaMakaiButterfly1>(), damage, 0f, NPC.target);
                Main.projectile[projectile2].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                MysticFragranceofaMakaiButterfly1 makaiButterflyWing2 = (MysticFragranceofaMakaiButterfly1)Main.projectile[projectile2].ModProjectile;
                makaiButterflyWing2.center = NPC.Center + new Vector2(200f, -200f);
                makaiButterflyWing2.ProjectileAI = 1;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile2);

                int projectile3 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX, ModContent.ProjectileType<MysticFragranceofaMakaiButterfly1>(), damage, 0f, NPC.target);
                Main.projectile[projectile3].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                MysticFragranceofaMakaiButterfly1 makaiButterflyWing3 = (MysticFragranceofaMakaiButterfly1)Main.projectile[projectile3].ModProjectile;
                makaiButterflyWing3.center = NPC.Center + new Vector2(-500f, 375f);
                makaiButterflyWing3.ProjectileAI = 2;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile3);

                int projectile4 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX, ModContent.ProjectileType<MysticFragranceofaMakaiButterfly1>(), damage, 0f, NPC.target);
                Main.projectile[projectile4].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                MysticFragranceofaMakaiButterfly1 makaiButterflyWing4 = (MysticFragranceofaMakaiButterfly1)Main.projectile[projectile4].ModProjectile;
                makaiButterflyWing4.center = NPC.Center + new Vector2(500f, 375f);
                makaiButterflyWing4.ProjectileAI = 3;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile4);
            }

            if (Timer % 90 == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/laser"), NPC.Center);
                float offset = Main.rand.Next(-5, 6);
                for (int i = 0; i < 12; i++)
                {
                    Vector2 vector = Utils.RotatedBy(Vector2.UnitX, MathHelper.ToRadians(30f * i + offset));
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vector, ModContent.ProjectileType<MysticFragranceofaMakaiButterfly3>(), damage, 0f, NPC.target);
                }
            }
        }

        private void StarMaelstrom()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            int damage = (int)(35 * damageScale);
            if (Timer == 0)
            {
                CreateWings();
                for (int i = 0; i < 5; i++)
                {
                    float distance = 1000;
                    Vector2 center = NPC.Center;
                    switch (i)
                    {
                        case 0:
                            distance = 500;
                            center = NPC.Center;
                            break;

                        case 1:
                            center = NPC.Center + new Vector2(-200f, -200f);
                            break;

                        case 2:
                            center = NPC.Center + new Vector2(200f, -200f);
                            break;

                        case 3:
                            center = NPC.Center + new Vector2(-500f, 375f);
                            break;

                        case 4:
                            center = NPC.Center + new Vector2(500f, 375f);
                            break;
                    }
                    for (int j = 0; j < 12; j++)
                    {
                        int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX, ModContent.ProjectileType<StarMaelstrom1>(), damage, 0f, NPC.target);
                        Main.projectile[projectile].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                        StarMaelstrom1 starMaelstrom = (StarMaelstrom1)Main.projectile[projectile].ModProjectile;
                        starMaelstrom.center = center;
                        starMaelstrom.angleOffset = 30f * j;
                        starMaelstrom.distance = distance;
                        NetMessage.SendData(MessageID.SyncProjectile, number: projectile);
                    }
                }
            }
        }

        private void DevilsRecitation()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            int damage = (int)(75 * damageScale);
            if (Timer == 0)
            {
                CreateWings();
                SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/charge"), NPC.Center);
                Vector2 center = NPC.Center;
                for (int i = 0; i < 4; i++)
                {
                    switch (i)
                    {
                        case 0:
                            center = NPC.Center + new Vector2(-200f, -200f);
                            break;

                        case 1:
                            center = NPC.Center + new Vector2(200f, -200f);
                            break;

                        case 2:
                            center = NPC.Center + new Vector2(-500f, 375f);
                            break;

                        case 3:
                            center = NPC.Center + new Vector2(500f, 375f);
                            break;
                    }
                    int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(), center, Vector2.UnitY, ModContent.ProjectileType<DevilsRecitation1>(), damage * 2, 0f, NPC.target);
                    Main.projectile[projectile].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                    DevilsRecitation1 laser = (DevilsRecitation1)Main.projectile[projectile].ModProjectile;
                    laser.ProjectileAI = i;
                    NetMessage.SendData(MessageID.SyncProjectile, number: projectile);
                }
            }

            if (Timer == 120)
                SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/superlaser"), NPC.Center);
            if (Timer >= 120 && Timer <= 210)
                NPC.position.Y -= 5f;

            if (Timer >= 150 && Timer % 120 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item9, NPC.Center);
                for (int i = -1; i < 2; i++)
                {
                    Vector2 velocity = NPC.DirectionTo(TargetCenter);
                    velocity = Utils.RotatedBy(velocity, MathHelper.ToRadians(i * 45f));
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity * 5f, ModContent.ProjectileType<DevilsRecitation3>(), 50, 0f, NPC.target);
                }
            }

            if (Timer >= 270 && Timer % 30 == 0)
            {
                Vector2 offset = NPC.Center;
                offset.X = Main.rand.Next((int)(NPC.Center.X - 1500), (int)(NPC.Center.X + 1501));
                offset.Y = Main.rand.Next((int)(NPC.Center.Y - 501), (int)NPC.Center.Y);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), offset, Vector2.UnitY * 5f, ModContent.ProjectileType<DevilsRecitation4>(), 35, 0f, NPC.target);
            }

            if (Timer == 840 || Timer == 1080 || Timer == 1320 || Timer == 1560 || Timer == 1800)
            {
                burstDirection = 1;
                startTimer = true;
            }
            else if (Timer == 960 || Timer == 1200 || Timer == 1440 || Timer == 1680 || Timer == 1920)
            {
                burstDirection = -1;
                startTimer = true;
            }
            DevilsRecitationSideAttack(burstDirection);

            if (Timer >= 1560 && Timer % 3 == 0)
            {
                Vector2 velocity = Utils.RotatedBy(Vector2.UnitX, MathHelper.ToRadians(angleOffset * 10f)) * 10f;
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<DevilsRecitation7>(), 30, 0f, NPC.target);
                AngleOffset(0f, 18f);
            }
        }

        private void AmagimiHijirisAirScroll()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            int damage = (int)(40 * damageScale);
            if (Timer % 120 == 0)
            {
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                Dash(TargetCenter, 25f);
            }
            if (Timer % 2 == 0)
            {
                int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<AmagimiHijirisAirScroll>(), damage, 0f, NPC.target);
                Main.projectile[projectile].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile);
            }
        }

        private void FlyingFantastica()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            if (Timer == 0)
                CreateWings();
        }

        #endregion

        private void CreateWings()
        {
            Vector2 center = NPC.Center;
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        center = NPC.Center + new Vector2(-200, -200f);
                        break;

                    case 1:
                        center = NPC.Center + new Vector2(200, -200f);
                        break;

                    case 2:
                        center = NPC.Center + new Vector2(-500, 375f);
                        break;

                    case 3:
                        center = NPC.Center + new Vector2(500, 375f);
                        break;
                }
                int wing = Projectile.NewProjectile(NPC.GetSource_FromAI(), center, Vector2.UnitX, ModContent.ProjectileType<ButterflyWing>(), 0, 0f, NPC.target);
                Main.projectile[wing].GetGlobalProjectile<TF2ProjectileBase>().owner = NPC.whoAmI;
                NetMessage.SendData(MessageID.SyncProjectile, number: wing);
            }
        }

        private void AngleOffset(float minAngleOffset, float maxAngleOffset, float speed = 1)
        {
            if (angleOffset >= maxAngleOffset)
                maxOffset = true;
            else if (angleOffset <= minAngleOffset)
                maxOffset = false;
            if (angleOffset < maxAngleOffset && !maxOffset)
                angleOffset += speed;
            else
                angleOffset -= speed;
        }

        private void DevilsRecitationSideAttack(int direction)
        {
            if (startTimer && Timer % 5 == 0 && burstCounter < 8)
            {
                Vector2 velocity = Utils.RotatedBy(-Vector2.UnitX * direction, MathHelper.ToRadians(burstCounter * 22.5f * direction)) * 5f;
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<DevilsRecitation5>(), 30, 0f, NPC.target);
                burstCounter++;
                if (burstCounter == 8)
                {
                    startTimer = false;
                    burstCounter = 0;
                }
            }
        }

        private void Dash(Vector2 targetPosition, float speed)
        {
            targetCenter = TargetCenter;
            NPC.direction = TargetCenter.X >= NPC.Center.X ? 1 : -1;
            Vector2 vector = targetPosition - NPC.Center;
            NPC.velocity = Vector2.Normalize(vector) * speed;
        }

        protected override bool HasExtraAttackPreparations() => true;

        protected override void ExtraAttackPreparations()
        {
            if ((Phase == Phase_BasicAttackPhase && Stage > 0) || Phase == Phase_BasicAttackPhase && preOmenofPurpleCloudsCounter >= 4)
                usedBasicAttack = true;
        }

        protected override void PostAttack()
        {
            angleOffset = 0f;
            burstCounter = 0;
            if (Stage == 0 && preOmenofPurpleCloudsCounter < 4 && Phase == Phase_BasicAttackPhase)
                preOmenofPurpleCloudsCounter++;
            else if (Stage > 0)
                preOmenofPurpleCloudsCounter = 0;
        }

        public override void OnKill()
        {
            if (!DownedGensokyoBoss.downedByakurenHijiri)
                NPC.SetEventFlagCleared(ref DownedGensokyoBoss.downedByakurenHijiri, -1);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
                calamity.TryFind("SupremeHealingPotion", out ModItem potion);
                potionType = potion.Type;
            }
            else
                potionType = ModContent.ItemType<MediumHealthPotion>();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ByakurenBossBag>()));
            LeadingConditionRule leadingConditionRule = new LeadingConditionRule(new Conditions.NotExpert());
            Chains.OnSuccess(leadingConditionRule, ItemDropRule.OneFromOptions(1, new int[]
            {
                ModContent.ItemType<AdvancedScoutRifle>(),
                ModContent.ItemType<HeadhunterPistols>(),
                ModContent.ItemType<ManualInferno>(),
                ModContent.ItemType<HarshPunisher>(),
                ModContent.ItemType<OffensiveRocketSystem>()
            }), false);
            if (GensokyoDLC.gensokyoLoaded)
            {
                GensokyoDLC.Gensokyo.TryFind("PointItem", out ModItem pointItem);
                Chains.OnSuccess(leadingConditionRule, ItemDropRule.Common(pointItem.Type, 1, 150, 200));
                GensokyoDLC.Gensokyo.TryFind("PowerItem", out ModItem powerItem);
                Chains.OnSuccess(leadingConditionRule, ItemDropRule.Common(powerItem.Type, 1, 100, 120));
            }
            npcLoot.Add(leadingConditionRule);
        }
    }
}