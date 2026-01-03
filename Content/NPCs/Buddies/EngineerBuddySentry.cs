using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.NPCs.Buildings;
using TF2.Content.Projectiles.NPCs;

namespace TF2.Content.NPCs.Buddies
{
    public class EngineerBuddySentry : Building
    {
        public override string Texture => "TF2/Content/NPCs/Buildings/SentryGun/SentryLevel1";

        protected override Asset<Texture2D> BuildingTexture => BuildingTextures.SentryLevel1Texture;

        protected override Asset<Texture2D> BuildingTextureAir => BuildingTextures.SentryLevel1AirTexture;

        public int Ammo
        {
            get => (int)NPC.localAI[1];
            set => NPC.localAI[1] = value;
        }

        protected int AttackAnimationTimer
        {
            get => (int)NPC.localAI[2];
            set => NPC.localAI[2] = value;
        }

        public override string BuildingName => Language.GetTextValue("Mods.TF2.NPCs.SentryGun");

        public override int BuildingCooldown => TF2.Time(10.5);

        protected override int ScrapMetalAmount => 65;

        protected override bool CanBeUpgraded => false;

        private int scanTimer;
        private bool playTargetSound;
        private SlotId sentrySoundSlot = new SlotId();

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            });
            NPC.netAlways = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 24;
            NPC.aiStyle = -1;
            NPC.lifeMax = 150;
            NPC.damage = 16;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_explode");
        }

        protected override void BuildingSpawn()
        {
            Ammo = 150;
            UpgradeCooldown = BuildingCooldown;
            Timer = TF2.Time(0.2);
        }

        protected override void BuildingAI()
        {
            NPC.noGravity = air;
            if (!Initialized)
            {
                int buildDuration = !hauled ? BuildingCooldown : BuildingCooldownHauled;
                int health = TF2.Round(Utils.Lerp(InitialHealth * Player.GetModPlayer<TF2Player>().healthMultiplier, NPC.lifeMax, (float)(buildDuration - UpgradeCooldown) / buildDuration)) - preConstructedDamage;
                if (InitialHealth != TF2.Round(NPC.lifeMax / Player.GetModPlayer<TF2Player>().healthMultiplier))
                    NPC.life = health;
                if (NPC.life < 0)
                    NPC.checkDead();
                UpgradeCooldown -= constructionSpeed;
                if (UpgradeCooldown < 0)
                    UpgradeCooldown = 0;
                if (UpgradeCooldown <= 0)
                {
                    NPC.life = NPC.lifeMax - preConstructedDamage;
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
            SentryAI();
        }

        private void SentryAI()
        {
            frame = AttackAnimationTimer <= 0 ? 0 : 1;
            Timer++;
            if (AttackAnimationTimer > 0)
                AttackAnimationTimer--;
            float distanceFromTarget = 500f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && Timer >= TF2.Time(0.2))
            {
                foreach (NPC targetNPC in Main.ActiveNPCs)
                {
                    if (targetNPC.CanBeChasedBy())
                    {
                        float distance = Vector2.Distance(targetNPC.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > distance;
                        bool inRange = distance < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNPC.position, targetNPC.width, targetNPC.height);
                        bool closeThroughWall = distance < 100f;
                        if ((1000f >= distance) && (closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = distance;
                            targetCenter = targetNPC.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget && Ammo > 0)
                {
                    if (!playTargetSound)
                    {
                        if (SoundEngine.TryGetActiveSound(sentrySoundSlot, out var scanSound))
                            scanSound?.Stop();
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_spot"), NPC.Center);
                        playTargetSound = true;
                    }
                    Vector2 shootVel = NPC.DirectionTo(targetCenter);
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 10f;
                    int type = ModContent.ProjectileType<SentryBullet>();
                    int damage = TF2.Round(16 * Player.GetModPlayer<TF2Player>().damageMultiplier);
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_shoot"), NPC.Center);
                    TF2.NPCCreateProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, Owner);
                    AttackAnimationTimer = TF2.Time(0.25);
                    Ammo--;
                    Timer = 0;
                    NPC.netUpdate = true;
                }
                else
                    playTargetSound = false;
            }
            if (!playTargetSound)
            {
                scanTimer++;
                if (scanTimer % TF2.Time(2) == 0)
                {
                    sentrySoundSlot = SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_scan"), NPC.Center);
                    scanTimer = 0;
                }
            }
        }

        protected override void BuildingSendExtraAI(BinaryWriter writer)
        {
            writer.Write(Ammo);
            writer.Write(scanTimer);
            writer.Write(playTargetSound);
        }

        protected override void BuildingReceiveExtraAI(BinaryReader reader)
        {
            Ammo = reader.ReadInt32();
            scanTimer = reader.ReadInt32();
            playTargetSound = reader.ReadBoolean();
        }
    }
}
