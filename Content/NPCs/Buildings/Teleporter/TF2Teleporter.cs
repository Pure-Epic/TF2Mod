using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items;
using static TF2.TF2;

namespace TF2.Content.NPCs.Buildings.Teleporter
{
    public abstract class TF2Teleporter : Building
    {
        public override int BuildingCooldown => Time(21);

        public override int BuildingCooldownHauled => Time(5.25);

        protected override int ScrapMetalAmount => 25;

        protected override Asset<Texture2D> BuildingTexture => BuildingTextures.TeleporterTexture;

        public override string Texture => "TF2/Content/NPCs/Buildings/Teleporter/Teleporter";

        protected virtual SoundStyle TeleporterSound => new SoundStyle("TF2/Content/Sounds/SFX/NPCs/teleporter_spin");

        internal SlotId teleporterSoundSlot = new SlotId();

        protected virtual void TeleporterSpawn()
        { }

        protected virtual void TeleporterAI()
        { }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            });
            NPC.netAlways = true;
        }

        protected override void BuildingSpawn()
        {
            UpgradeCooldown = BuildingCooldown;
            Timer2 = Time(5.12);
            TeleporterSpawn();
        }

        protected override void BuildingAI()
        {
            if (!Initialized)
            {
                int buildDuration = !hauled ? BuildingCooldown : BuildingCooldownHauled;
                int health = Round(Utils.Lerp(InitialHealth * Player.GetModPlayer<TF2Player>().healthMultiplier, NPC.lifeMax, (float)(buildDuration - UpgradeCooldown) / buildDuration)) - preConstructedDamage;
                if (InitialHealth != Round(NPC.lifeMax / Player.GetModPlayer<TF2Player>().healthMultiplier))
                    NPC.life = health;
                if (NPC.life < 0)
                    NPC.checkDead();
                UpgradeCooldown -= constructionSpeed;
                if (UpgradeCooldown < 0)
                    UpgradeCooldown = 0;
                if (UpgradeCooldown <= 0)
                {
                    NPC.life = NPC.lifeMax - preConstructedDamage;
                    if (this is TeleporterEntrance)
                    {
                        (this as TeleporterEntrance).FindTeleporterExit(out TeleporterExit exit);
                        if (!Initialized && exit != null && !hauled)
                        {
                            Metal = exit.Metal;
                            if (exit is TeleporterExitLevel2 && this is TeleporterEntranceLevel1)
                                SyncTeleporter(ModContent.NPCType<TeleporterEntranceLevel2>(), exit);
                            else if (exit is TeleporterExitLevel3 && (this is TeleporterEntranceLevel2 || this is TeleporterEntranceLevel1))
                                SyncTeleporter(ModContent.NPCType<TeleporterEntranceLevel3>(), exit);
                        }
                    }
                    else if (this is TeleporterExit)
                    {
                        (this as TeleporterExit).FindTeleporterEntrance(out TeleporterEntrance entrance);
                        if (!Initialized && entrance != null && !hauled)
                        {
                            Metal = entrance.Metal;
                            if (entrance is TeleporterEntranceLevel2 && this is TeleporterExitLevel1)
                                SyncTeleporter(ModContent.NPCType<TeleporterExitLevel2>(), entrance);
                            else if (entrance is TeleporterEntranceLevel3 && (this is TeleporterExitLevel2 || this is TeleporterExitLevel1))
                                SyncTeleporter(ModContent.NPCType<TeleporterExitLevel3>(), entrance);
                        }
                    }
                    Initialized = true;
                    NPC.netUpdate = true;
                }
                return;
            }
            if (UpgradeCooldown > 0)
            {
                Timer = 0;
                Timer2 = 0;
                return;
            }
            UpgradeCooldown--;
            if (UpgradeCooldown < 0)
                UpgradeCooldown = 0;
            TeleporterAI();
        }

        internal void SyncTeleporter(int newTeleporter, TF2Teleporter oldTeleporter = null)
        {
            Player player = Main.player[Owner];
            int i = NPC.NewNPC(player.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, newTeleporter, 0, 0, Metal, 0, Owner);
            NPC npc = Main.npc[i];
            npc.direction = NPC.direction;
            Building building = (Building)npc.ModNPC;
            if (oldTeleporter != null)
                building.Metal = oldTeleporter.Metal;
            else
                building.UpgradeCooldown = 0;
            npc.netUpdate = true;
            if (building is TeleporterEntrance)
                player.GetModPlayer<TF2Player>().teleporterEntranceWhoAmI = i;
            else if (building is TeleporterExit)
                player.GetModPlayer<TF2Player>().teleporterExitWhoAmI = i;
            KillNPC(NPC);
        }

        public void HaulTeleporter(int metal)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                UpgradeCooldown = BuildingCooldownHauled;
                hauled = true;
            }
            else
            {
                ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
                packet.Write((byte)MessageType.SyncTeleporter);
                packet.Write((byte)NPC.whoAmI);
                packet.Write(metal);
                packet.Send(-1, Main.myPlayer);
            }
        }

    }

    public class TeleporterStatistics
    {
        public int Type { get; set; }

        public int Metal { get; set; }
    }

    public abstract class TeleporterEntrance : TF2Teleporter
    {
        protected virtual int TeleporterCooldown => Time(10);

        protected virtual int NextUpgrade => ModContent.NPCType<TeleporterEntranceLevel2>();

        public override string BuildingName => Language.GetTextValue("Mods.TF2.NPCs.TeleporterEntrance");

        protected override void BuildingDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D sprite = BuildingTexture.Value;
            int height = sprite.Height / 4;
            float frameWidth = 0;
            float frameHeight = height - NPC.height;
            spriteBatch.Draw(sprite, NPC.position - screenPos, new Rectangle(0, height * frame, sprite.Width, height), drawColor, 0f, new Vector2(frameWidth, frameHeight), NPC.scale, SpriteEffects.None, 0f);
        }

        protected override void TeleporterSpawn() => Timer = TeleporterCooldown;

        protected override void TeleporterAI()
        {
            if (NPC.type != ModContent.NPCType<TeleporterEntranceLevel3>())
                UpgradeBuilding(NextUpgrade);
            FindTeleporterExit(out TeleporterExit exit);
            if (exit == null || !exit.Initialized) return;
            Timer++;
            Timer2++;
            if (Timer < TeleporterCooldown)
                frame = (Timer % Utils.Clamp(TeleporterCooldown - Timer, Time(0.1), TeleporterCooldown) == 0) ? 1 : 0;
            else
                frame = Timer % Time(0.1) == 0 ? 3 : 2;
            NPC.netUpdate = true;
            if (Timer >= TeleporterCooldown)
            {
                foreach (Player player in Main.ActivePlayers)
                {
                    if (NPC.Hitbox.Intersects(player.Hitbox) && !player.dead && !player.hostile && Timer >= TeleporterCooldown)
                    {
                        player.Center = new Vector2(exit.NPC.Center.X, exit.NPC.position.Y);
                        player.fallStart = (int)(player.position.Y / 16f);
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/teleporter_send"), NPC.Center);
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/teleporter_receive"), exit.NPC.Center);
                        Timer = 0;
                        NPC.netUpdate = true;
                        if (SoundEngine.TryGetActiveSound(teleporterSoundSlot, out var entranceSound))
                        {
                            entranceSound?.Stop();
                            Timer2 = 0;
                        }
                        if (SoundEngine.TryGetActiveSound(exit.teleporterSoundSlot, out var exitSound))
                            exitSound?.Stop();
                    }
                }
                if (Timer2 >= Time(5.12))
                {
                    teleporterSoundSlot = SoundEngine.PlaySound(TeleporterSound, NPC.position);
                    if (exit != null)
                        exit.teleporterSoundSlot = SoundEngine.PlaySound(TeleporterSound, NPC.position);
                    Timer2 = 0;
                }
            }
        }

        protected override void BuildingDestroy()
        {
            if (SoundEngine.TryGetActiveSound(teleporterSoundSlot, out var entranceSound))
                entranceSound?.Stop();
            FindTeleporterExit(out TeleporterExit exit);
            if (exit != null)
            {
                if (SoundEngine.TryGetActiveSound(exit.teleporterSoundSlot, out var exitSound))
                    exitSound?.Stop();
                exit?.SyncTeleporter(ModContent.NPCType<TeleporterExitLevel1>());
                exit.Metal = 0;
            }
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (p.currentClass == TF2Item.Engineer)
                p.teleporterEntranceWhoAmI = -1;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/engineer_autodestroyedteleporter01"), Player.Center);
        }

        public void FindTeleporterExit(out TeleporterExit exit)
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.ModNPC is TeleporterExit teleporterExit && teleporterExit.Owner == Owner)
                {
                    exit = teleporterExit;
                    return;
                }
            }
            exit = null;
        }
    }

    public class TeleporterEntranceLevel1 : TeleporterEntrance
    {
        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 16;
            NPC.aiStyle = -1;
            NPC.lifeMax = 150;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/NPCs/teleporter_explode");
        }
    }

    public class TeleporterEntranceLevel2 : TeleporterEntrance
    {
        protected override int TeleporterCooldown => Time(5);

        protected override SoundStyle TeleporterSound => new SoundStyle("TF2/Content/Sounds/SFX/NPCs/teleporter_spin2");

        protected override int NextUpgrade => ModContent.NPCType<TeleporterEntranceLevel3>();

        public override int InitialHealth => 180;

        public override int BuildingCooldown => Time(1.6);

        protected override int Level => 2;

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 16;
            NPC.aiStyle = -1;
            NPC.lifeMax = 180;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/NPCs/teleporter_explode");
        }
    }

    public class TeleporterEntranceLevel3 : TeleporterEntrance
    {
        protected override int TeleporterCooldown => Time(3);

        protected override SoundStyle TeleporterSound => new SoundStyle("TF2/Content/Sounds/SFX/NPCs/teleporter_spin3");

        public override int InitialHealth => 216;

        public override int BuildingCooldown => Time(1.6);

        protected override int Level => 3;

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 16;
            NPC.aiStyle = -1;
            NPC.lifeMax = 216;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/NPCs/teleporter_explode");
        }
    }

    public abstract class TeleporterExit : TF2Teleporter
    {
        public override string BuildingName => Language.GetTextValue("Mods.TF2.NPCs.TeleporterExit");

        protected virtual int NextUpgrade => ModContent.NPCType<TeleporterExitLevel2>();

        protected override void BuildingDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D sprite = BuildingTexture.Value;
            int height = sprite.Height / 4;
            float frameWidth = 0;
            float frameHeight = height - NPC.height;
            spriteBatch.Draw(sprite, NPC.position - screenPos, new Rectangle(0, height * frame, sprite.Width, height), drawColor, 0f, new Vector2(frameWidth, frameHeight), NPC.scale, SpriteEffects.None, 0f);
        }

        protected override void TeleporterAI()
        {
            if (NPC.type != ModContent.NPCType<TeleporterExitLevel3>())
                UpgradeBuilding(NextUpgrade);
            FindTeleporterEntrance(out TeleporterEntrance entrance);
            if (entrance != null && entrance.Initialized)
            {
                Timer = entrance.Timer;
                frame = entrance.frame;
            }
            else return;
            NPC.netUpdate = true;
        }

        protected override void BuildingDestroy()
        {
            if (SoundEngine.TryGetActiveSound(teleporterSoundSlot, out var exitSound))
                exitSound?.Stop();
            FindTeleporterEntrance(out TeleporterEntrance entrance);
            if (entrance != null)
            {
                if (SoundEngine.TryGetActiveSound(entrance.teleporterSoundSlot, out var entranceSound))
                    entranceSound?.Stop();
                entrance?.SyncTeleporter(ModContent.NPCType<TeleporterEntranceLevel1>());
                entrance.Metal = 0;
            }
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (p.currentClass == TF2Item.Engineer)
                p.teleporterExitWhoAmI = -1;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/engineer_autodestroyedteleporter01"), Player.Center);
        }

        public void FindTeleporterEntrance(out TeleporterEntrance entrance)
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.ModNPC is TeleporterEntrance teleporterEntrance && teleporterEntrance.Owner == Owner)
                {
                    entrance = teleporterEntrance;
                    return;
                }
            }
            entrance = null;
        }
    }

    public class TeleporterExitLevel1 : TeleporterExit
    {
        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 16;
            NPC.aiStyle = -1;
            NPC.lifeMax = 150;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/NPCs/teleporter_explode");
        }
    }

    public class TeleporterExitLevel2 : TeleporterExit
    {
        protected override SoundStyle TeleporterSound => new SoundStyle("TF2/Content/Sounds/SFX/NPCs/teleporter_spin2");

        protected override int NextUpgrade => ModContent.NPCType<TeleporterExitLevel3>();

        public override int InitialHealth => 180;

        public override int BuildingCooldown => Time(1.6);

        protected override int Level => 2;

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 16;
            NPC.aiStyle = -1;
            NPC.lifeMax = 180;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/NPCs/teleporter_explode");
        }
    }

    public class TeleporterExitLevel3 : TeleporterExit
    {
        protected override SoundStyle TeleporterSound => new SoundStyle("TF2/Content/Sounds/SFX/NPCs/teleporter_spin3");

        public override int InitialHealth => 216;

        public override int BuildingCooldown => Time(1.6);

        protected override int Level => 3;

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 16;
            NPC.aiStyle = -1;
            NPC.lifeMax = 216;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/NPCs/teleporter_explode");
        }
    }
}