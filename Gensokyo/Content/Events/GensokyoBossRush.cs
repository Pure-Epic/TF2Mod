using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;
using Terraria.Chat;
using Terraria.Audio;
using System.Linq;
using Terraria.GameContent;
using TF2.Gensokyo.Content.NPCs.Byakuren_Hijiri;
using TF2.Gensokyo.Common;

namespace TF2.Gensokyo.Events
{
    [ExtendsFromMod("Gensokyo")]
    public class GensokyoBossRush : ModSystem
    {
        private static Player targetPlayer;
        private int timer;
        private static bool bossActive;
        private static int bossesDefeated;
        private const int totalBosses = 19;
        private static readonly List<ModNPC> bossList = new();

        private static readonly int spawnRangeX = (int)(NPC.sWidth / 16 * 0.7);
        private static readonly int spawnRangeY = (int)(NPC.sHeight / 16 * 0.7);
        private static readonly int spawnSpaceX = 3;
        private static readonly int spawnSpaceY = 3;

        public static void GetPlayer(Player player) => targetPlayer = player;

        public override void PostUpdateWorld()
        {
            if (GensokyoDLC.bossRush)
            {
                CheckBossRushProgress();
                UpdateBossRush();
            }
        }

        public override void OnWorldUnload()
        {
            GensokyoDLC.bossRush = false;
            bossActive = false;
        }

        public static void StartBossRush()
        {
            BossList();
            // Main.invasionType = -1;
            // Main.invasionProgressIcon = 69;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (!Main.npc[i].friendly)
                    Main.npc[i].active = false;
            }
            GensokyoDLC.bossRush = true;
            bossesDefeated = 0;
        }

        public static void EndBossRush()
        {
            GensokyoDLC.bossRush = false;
            bossActive = false;
            // Main.invasionType = 0;
            // Main.invasionDelay = 0;
            // Main.invasionProgressDisplayLeft = 0;
        }

        public static void BossRushResult()
        {
            string text = "";
            if (bossesDefeated == totalBosses)
                text = "Boss Rush Completed.";
            else if (FailCheck())
                text = "You failed!";
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(text, 175, 75, 255);
                return;
            }
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(25, -1, -1, NetworkText.FromLiteral(text), 255, 175f, 75f, 255f, 0, 0, 0);
        }

        public void UpdateBossRush()
        {
            // If the custom invasion is up
            if (GensokyoDLC.bossRush)
            {
                // End invasion if all bosses are defeated or the player dies
                if (bossesDefeated == 19 || FailCheck())
                {
                    BossRushResult();
                    EndBossRush();
                    if (bossesDefeated == 19)
                    {
                        if (!DownedGensokyoBoss.downedBossRush)
                            DownedGensokyoBoss.downedBossRush = true;
                        SpawnByakuren.targetPlayer = targetPlayer;
                        SpawnByakuren.startSpawningByakuren = true;
                    }
                }

                if (BossCooldown())
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        SpawnBoss(bossList[bossesDefeated].NPC);
                }
            }
        }

        public static void CheckBossRushProgress() => Main.invasionProgressNearInvasion = true;

        public static bool FailCheck()
        {
            if (targetPlayer.dead && Main.netMode == NetmodeID.SinglePlayer)
                return true;
            else if (Main.player.Take(Main.maxPlayers).Where(x => x.active).All(x => x.dead || x.ghost) && Main.netMode != NetmodeID.SinglePlayer)
                return true;
            else
                return false;
        }

        public static void NextBoss()
        {
            bossActive = false;
            bossesDefeated++;
        }

        public static void SpawnBoss(NPC npc)
        {
            bool flag = false;
            int num10 = 0;
            int num11 = 0;
            int num12 = (int)(targetPlayer.position.X / 16f) - spawnRangeX * 2;
            int num13 = (int)(targetPlayer.position.X / 16f) + spawnRangeX * 2;
            int num14 = (int)(targetPlayer.position.Y / 16f) - spawnRangeY * 2;
            int num15 = (int)(targetPlayer.position.Y / 16f) + spawnRangeY * 2;
            int num16 = (int)(targetPlayer.position.X / 16f) - NPC.safeRangeX;
            int num17 = (int)(targetPlayer.position.X / 16f) + NPC.safeRangeX;
            int num18 = (int)(targetPlayer.position.Y / 16f) - NPC.safeRangeY;
            int num19 = (int)(targetPlayer.position.Y / 16f) + NPC.safeRangeY;
            if (num12 < 0)
            {
                num12 = 0;
            }
            if (num13 > Main.maxTilesX)
            {
                num13 = Main.maxTilesX;
            }
            if (num14 < 0)
            {
                num14 = 0;
            }
            if (num15 > Main.maxTilesY)
            {
                num15 = Main.maxTilesY;
            }
            for (int m = 0; m < 1000; m++)
            {
                for (int n = 0; n < 100; n++)
                {
                    int num20 = Main.rand.Next(num12, num13);
                    int num21 = Main.rand.Next(num14, num15);
                    if (!Main.tile[num20, num21].IsActuated || !Main.tileSolid[Main.tile[num20, num21].TileType])
                    {
                        if (Main.wallHouse[Main.tile[num20, num21].WallType] && m < 999)
                            continue;
                        for (int num22 = num21; num22 < Main.maxTilesY; num22++)
                        {
                            if (!Main.tile[num20, num22].IsActuated && Main.tileSolid[Main.tile[num20, num22].TileType])
                            {
                                if ((num20 < num16 || num20 > num17 || num22 < num18 || num22 > num19 || m == 999) && ((num20 >= num12 && num20 <= num13 && num22 >= num14 && num22 <= num15) || m == 999))
                                {
                                    _ = Main.tile[num20, num22].TileType;
                                    num10 = num20;
                                    num11 = num22;
                                    flag = true;
                                }
                                break;
                            }
                        }
                        if (flag && m < 999)
                        {
                            int num24 = num10 - spawnSpaceX / 2;
                            int num25 = num10 + spawnSpaceX / 2;
                            int num26 = num11 - spawnSpaceY;
                            int num27 = num11;
                            if (num24 < 0)
                            {
                                flag = false;
                            }
                            if (num25 > Main.maxTilesX)
                            {
                                flag = false;
                            }
                            if (num26 < 0)
                            {
                                flag = false;
                            }
                            if (num27 > Main.maxTilesY)
                            {
                                flag = false;
                            }
                            if (flag)
                            {
                                for (int num28 = num24; num28 < num25; num28++)
                                {
                                    for (int num29 = num26; num29 < num27; num29++)
                                    {
                                        if (Main.tile[num28, num29].IsActuated && Main.tileSolid[Main.tile[num28, num29].TileType])
                                        {
                                            flag = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (flag || flag)
                    {
                        break;
                    }
                }
                if (flag && m < 999)
                {
                    Rectangle rectangle = new Rectangle(num10 * 16, num11 * 16, 16, 16);
                    for (int num30 = 0; num30 < 255; num30++)
                    {
                        if (Main.player[num30].active)
                        {
                            Rectangle rectangle2 = new Rectangle((int)(Main.player[num30].position.X + (float)(Main.player[num30].width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.player[num30].position.Y + (float)(Main.player[num30].height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                            if (rectangle.Intersects(rectangle2))
                            {
                                flag = false;
                            }
                        }
                    }
                }
                if (flag)
                {
                    break;
                }
            }
            if (flag)
            {
                int spawnPositionX = num10 * 16 + 8;
                int spawnPositionY = num11 * 16;
                int type = npc.type;
                int i = NPC.NewNPC(NPC.GetBossSpawnSource(targetPlayer.whoAmI), spawnPositionX, spawnPositionY, type);
                Main.npc[i].GetGlobalNPC<GensokyoBossRushMarker>().realBoss = true;
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue("Announcement.HasAwoken", Main.npc[i].TypeName), 175, 75);
                else if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[i].GetTypeNetName()), new Color(175, 75, 255));
                bossActive = true;
            }
        }

        private bool BossCooldown()
        {
            timer++;
            if (bossActive || !GensokyoDLC.bossRush)
            {
                timer = 0;
                return false;
            }
            if (timer >= 150)
            {
                timer = 0;
                return true;
            }
            else
                return false;
        }

        private static void BossList()
        {
            bossList.Clear();
            if (GensokyoDLC.Gensokyo.TryFind("LilyWhite", out ModNPC lily))
                bossList.Add(lily);
            if (GensokyoDLC.Gensokyo.TryFind("Rumia", out ModNPC rumia))
                bossList.Add(rumia);
            if (GensokyoDLC.Gensokyo.TryFind("EternityLarva", out ModNPC larva))
                bossList.Add(larva);
            if (GensokyoDLC.Gensokyo.TryFind("Nazrin", out ModNPC nazrin))
                bossList.Add(nazrin);
            if (GensokyoDLC.Gensokyo.TryFind("HinaKagiyama", out ModNPC hina))
                bossList.Add(hina);
            if (GensokyoDLC.Gensokyo.TryFind("Sekibanki", out ModNPC sekibanki))
                bossList.Add(sekibanki);
            if (GensokyoDLC.Gensokyo.TryFind("Seiran", out ModNPC seiran))
                bossList.Add(seiran);
            if (GensokyoDLC.Gensokyo.TryFind("NitoriKawashiro", out ModNPC nitori))
                bossList.Add(nitori);
            if (GensokyoDLC.Gensokyo.TryFind("MedicineMelancholy", out ModNPC medicine))
                bossList.Add(medicine);
            if (GensokyoDLC.Gensokyo.TryFind("Cirno", out ModNPC cirno))
                bossList.Add(cirno);
            if (GensokyoDLC.Gensokyo.TryFind("MinamitsuMurasa", out ModNPC minamitsu))
                bossList.Add(minamitsu);
            if (GensokyoDLC.Gensokyo.TryFind("AliceMargatroid", out ModNPC alice))
                bossList.Add(alice);
            if (GensokyoDLC.Gensokyo.TryFind("SakuyaIzayoi", out ModNPC sakuya))
                bossList.Add(sakuya);
            if (GensokyoDLC.Gensokyo.TryFind("SeijaKijin", out ModNPC seija))
                bossList.Add(seija);
            if (GensokyoDLC.Gensokyo.TryFind("MayumiJoutouguu", out ModNPC mayumi))
                bossList.Add(mayumi);
            if (GensokyoDLC.Gensokyo.TryFind("ToyosatomimiNoMiko", out ModNPC miko))
                bossList.Add(miko);
            if (GensokyoDLC.Gensokyo.TryFind("KaguyaHouraisan", out ModNPC kaguya))
                bossList.Add(kaguya);
            if (GensokyoDLC.Gensokyo.TryFind("UtsuhoReiuji", out ModNPC okuu))
                bossList.Add(okuu);
            if (GensokyoDLC.Gensokyo.TryFind("TenshiHinanawi", out ModNPC tenshi))
                bossList.Add(tenshi);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Invasion Progress Bars"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "TF2: Invasion Progress Bars",
                    delegate
                    {
                        DrawInterface_15_InvasionProgressBars();
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        private static void DrawInterface_15_InvasionProgressBars()
        {
            DrawInvasionProgress();
            if (Main.HealthBarDrawSettings != 0)
                Main.BigBossProgressBar.Draw(Main.spriteBatch);
        }

        public static void DrawInvasionProgress()
        {
            if (!GensokyoDLC.bossRush)
                return;
            if (Main.invasionProgressMode == 2 && Main.invasionProgressNearInvasion && Main.invasionProgressDisplayLeft < 160)
                Main.invasionProgressDisplayLeft = 160;
            if (!Main.gamePaused && Main.invasionProgressDisplayLeft > 0)
                Main.invasionProgressDisplayLeft--;
            if (Main.invasionProgressDisplayLeft > 0)
                Main.invasionProgressAlpha += 0.05f;
            else
                Main.invasionProgressAlpha -= 0.05f;
            if (Main.invasionProgressMode == 0)
            {
                Main.invasionProgressDisplayLeft = 0;
                Main.invasionProgressAlpha = 0f;
            }
            if (Main.invasionProgressAlpha < 0f)
                Main.invasionProgressAlpha = 0f;
            if (Main.invasionProgressAlpha > 1f)
                Main.invasionProgressAlpha = 1f;
            if (Main.invasionProgressAlpha <= 0f)
                return;
            float num = 0.5f + Main.invasionProgressAlpha * 0.5f;
            Texture2D value = (Texture2D)ModContent.Request<Texture2D>("TF2/Gensokyo/Content/Events/BossRushIcon");
            string text = " Boss Rush";
            Color c = new Color(255, 64, 64) * 0.5f;

            int num7 = (int)(200f * num);
            int num8 = (int)(45f * num);
            Vector2 vector3 = new Vector2(Main.screenWidth - 120, Main.screenHeight - 40);
            Utils.DrawInvBG(R: new Rectangle((int)vector3.X - num7 / 2, (int)vector3.Y - num8 / 2, num7, num8), sb: Main.spriteBatch, c: new Color(63, 65, 151, 255) * 0.785f);
            string text3;
            text3 = ((totalBosses != 0) ? ((int)(bossesDefeated * 100f / totalBosses) + "%") : bossesDefeated.ToString());
            text3 = Language.GetTextValue("Game.WaveCleared", text3);
            Texture2D value3 = TextureAssets.ColorBar.Value;
            // TextureAssets.ColorBlip.Value;
            if (totalBosses != 0)
            {
                Main.spriteBatch.Draw(value3, vector3, null, Color.White * Main.invasionProgressAlpha, 0f, new Vector2(value3.Width / 2, 0f), num, SpriteEffects.None, 0f);
                float num9 = MathHelper.Clamp((float)bossesDefeated / totalBosses, 0f, 1f);
                Vector2 vector4 = FontAssets.MouseText.Value.MeasureString(text3);
                float num10 = num;
                if (vector4.Y > 22f)
                    num10 *= 22f / vector4.Y;
                float num11 = 169f * num;
                float num12 = 8f * num;
                Vector2 vector5 = vector3 + Vector2.UnitY * num12 + Vector2.UnitX * 1f;
                Utils.DrawBorderString(Main.spriteBatch, text3, vector5 + new Vector2(0f, -4f), Color.White * Main.invasionProgressAlpha, num10, 0.5f, 1f);
                vector5 += Vector2.UnitX * (num9 - 0.5f) * num11;
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, vector5, new Rectangle(0, 0, 1, 1), new Color(255, 241, 51) * Main.invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(num11 * num9, num12), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, vector5, new Rectangle(0, 0, 1, 1), new Color(255, 165, 0, 127) * Main.invasionProgressAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, num12), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, vector5, new Rectangle(0, 0, 1, 1), Color.Black * Main.invasionProgressAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(num11 * (1f - num9), num12), SpriteEffects.None, 0f);
            }

            Vector2 vector6 = FontAssets.MouseText.Value.MeasureString(text);
            float num13 = 120f;
            if (vector6.X > 200f)
                num13 += vector6.X - 200f;
            Rectangle r3 = Utils.CenteredRectangle(new Vector2((float)Main.screenWidth - num13, Main.screenHeight - 80), (vector6 + new Vector2(value.Width + 12, 6f)) * num);
            Utils.DrawInvBG(Main.spriteBatch, r3, c);
            Main.spriteBatch.Draw(value, r3.Left() + Vector2.UnitX * num * 8f, null, Color.White * Main.invasionProgressAlpha, 0f, new Vector2(0f, value.Height / 2), num * 0.8f, SpriteEffects.None, 0f);
            Utils.DrawBorderString(Main.spriteBatch, text, r3.Right() + Vector2.UnitX * num * -22f, Color.White * Main.invasionProgressAlpha, num * 0.9f, 1f, 0.4f);
        }
    }

    [ExtendsFromMod("Gensokyo")]
    public class GensokyoBossRushMarker : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool realBoss;
        private bool bossScalingInitialized;

        public override void OnKill(NPC npc)
        {
            if (realBoss)
                GensokyoBossRush.NextBoss();
        }

        public override void AI(NPC npc)
        {
            if (realBoss)
            {
                if (!GensokyoDLC.bossRush)
                    npc.active = false;
                ModifyBossStats(npc, "Lily White", 25000, 35000);
                ModifyBossStats(npc, "Rumia", 22000, 30000);
                ModifyBossStats(npc, "Eternity Larva", 27000, 37000);
                ModifyBossStats(npc, "Nazrin", 30000, 42000);
                ModifyBossStats(npc, "Hina Kagiyama", 32000, 48000);
                ModifyBossStats(npc, "Sekibanki", 35000, 50000);
                ModifyBossStats(npc, "Seiran", 33000, 44000);
                ModifyBossStats(npc, "Nitori Kawashiro", 42000, 60000);
                ModifyBossStats(npc, "Medicine Melancholy", 40000, 55000);
                ModifyBossStats(npc, "Cirno", 41000, 52000);
                ModifyBossStats(npc, "Minamitsu Murasa", 60000, 80000);
                ModifyBossStats(npc, "Alice Margatroid", 50000, 65000);
                ModifyBossStats(npc, "Sakuya Izayoi", 65000, 85000);
                ModifyBossStats(npc, "Seija Kijin", 75000, 100000);
                ModifyBossStats(npc, "Mayumi Joutouguu", 60000, 75000);
                ModifyBossStats(npc, "Toyosatomimi no Miko", 100000, 150000);
                ModifyBossStats(npc, "Kaguya Houraisan", 120000, 175000);
                ModifyBossStats(npc, "Utsuho Reiuji", 150000, 225000);
                ModifyBossStats(npc, "Tenshi Hinanawi", 200000, 300000);
            }
        }

        private void ModifyBossStats(NPC npc, string name, int newHealth, int newExpertHealth)
        {
            if (npc.TypeName == name && !bossScalingInitialized)
            {
                if (!Main.expertMode)
                {
                    npc.lifeMax = newHealth;
                    npc.life = npc.lifeMax;
                    bossScalingInitialized = true;
                }
                else
                {
                    npc.lifeMax = newExpertHealth;
                    npc.life = npc.lifeMax;
                    bossScalingInitialized = true;
                }
            }
        }
    }

    [ExtendsFromMod("Gensokyo")]
    public class BossRushSceneEffect : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Gensokyo/Content/Sounds/Music/Binah Raid Boss BGM - Endless Carnival");

        public override SceneEffectPriority Priority => (SceneEffectPriority)9; // This is higher than SceneEffectPriority.BossHigh

        public override bool IsSceneEffectActive(Player player) => GensokyoDLC.bossRush;
    }

    [ExtendsFromMod("Gensokyo")]
    public class SpawnByakuren : ModSystem
    {
        public static bool startSpawningByakuren;
        public static Player targetPlayer;
        public int timer;

        public override void PreUpdateWorld()
        {
            if (startSpawningByakuren)
                timer++;
            else
                timer = 0;

            if (timer == 150)
                TF2.Dialogue("Are you the hero that Gensokyo fears?", Color.DarkMagenta);
            else if (timer == 300)
                TF2.Dialogue("If so, you have proven yourself worthy.", Color.DarkMagenta);
            else if (timer == 450)
                TF2.Dialogue("Unfortunately, you are no match for me.", Color.DarkMagenta);
            else if (timer == 600)
                TF2.Dialogue("So, I will make my statement clear.", Color.DarkMagenta);
            else if (timer == 750)
                TF2.Dialogue("You know what I want with Youkai slayers, right darling?", Color.DarkMagenta);
            else if (timer == 900)
                TF2.Dialogue("I want them dead...", Color.DarkMagenta);
            else if (timer == 1050)
                TF2.Dialogue("Now!", Color.DarkMagenta);
            else if (timer == 1200)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/bell"), targetPlayer.position);
                int type = ModContent.NPCType<ByakurenHijiri>();
                NPC.SpawnOnPlayer(targetPlayer.whoAmI, type);
                startSpawningByakuren = false;
            }
        }
    }
}
