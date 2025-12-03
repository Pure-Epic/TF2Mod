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
using TF2.Content.Items;
using TF2.Content.Items.Weapons.Engineer;
using TF2.Content.Projectiles.NPCs;
using static TF2.TF2;

namespace TF2.Content.NPCs.Buildings.SentryGun
{
    public abstract class TF2Sentry : Building
    {
        public int Ammo
        {
            get => (int)NPC.localAI[1];
            set => NPC.localAI[1] = value;
        }

        public int RocketAmmo
        {
            get => (int)NPC.localAI[2];
            set => NPC.localAI[2] = value;
        }

        protected int AttackAnimationTimer
        {
            get => (int)NPC.localAI[3];
            set => NPC.localAI[3] = value;
        }

        public virtual int Rounds => 150;

        protected virtual SoundStyle SentrySound => new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_scan");

        public override string BuildingName => Language.GetTextValue("Mods.TF2.NPCs.SentryGun");

        public override int BuildingCooldown => Time(10.5);

        public override int BuildingCooldownHauled => Time(3.5);

        protected override int ScrapMetalAmount => 65;

        protected int rocketAttackAnimationTimer;
        public bool wrangled;
        protected int wranglerCooldown;
        protected int scanTimer;
        protected bool playTargetSound;
        internal SlotId sentrySoundSlot = new SlotId();
        private static Asset<Texture2D> wranglerShield;

        public override void Load()
        {
            if (!Main.dedServ)
                wranglerShield = ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/SentryGun/WranglerShield");
        }

        protected virtual void SentrySpawn()
        { }

        protected virtual void SentryDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        { }

        protected virtual void SentryAI()
        { }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            });
            NPC.netAlways = true;
        }

        protected override void BuildingSpawn()
        {
            if (NPC.ModNPC is SentryLevel1 || NPC.ModNPC is MiniSentry)
                Ammo = Rounds;
            RocketAmmo = 20;
            UpgradeCooldown = BuildingCooldown;
            SentrySpawn();
        }

        protected override void BuildingDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SentryDraw(spriteBatch, screenPos, drawColor);
            if (wrangled)
                spriteBatch.Draw(wranglerShield.Value, NPC.Center - screenPos - new Vector2(wranglerShield.Value.Width / 2f, wranglerShield.Value.Height / 2f), Color.White);
        }

        protected override void BuildingAI()
        {
            NPC.noGravity = air;
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
                    Initialized = true;
                    NPC.netUpdate = true;
                }
                return;
            }
            if (Player.HeldItem.ModItem is Wrangler)
            {
                wrangled = true;
                if (SoundEngine.TryGetActiveSound(sentrySoundSlot, out var scanSound))
                    scanSound?.Stop();
            }
            else if (wrangled)
            {
                wranglerCooldown++;
                if (wranglerCooldown >= Time(3))
                {
                    wrangled = false;
                    wranglerCooldown = 0;
                }
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

        protected override void BuildingDestroy()
        {
            if (SoundEngine.TryGetActiveSound(sentrySoundSlot, out var scanSound))
                scanSound?.Stop();
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (p.currentClass == TF2Item.Engineer)
            {
                p.sentryWhoAmI = -1;
                FrontierJusticePlayer f = Player.GetModPlayer<FrontierJusticePlayer>();
                f.revenge += 3;
            }
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/engineer_autodestroyedsentry01"), Player.Center);
        }

        public void HaulSentry(int metal, int ammo, int ammo2, bool isAir)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                UpgradeCooldown = BuildingCooldownHauled;
                hauled = true;
            }
            else
            {
                ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
                packet.Write((byte)MessageType.SyncSentry);
                packet.Write((byte)NPC.whoAmI);
                packet.Write(metal);
                packet.Write(ammo);
                packet.Write(ammo2);
                packet.Write(isAir);
                packet.Send(-1, Main.myPlayer);
            }
        }

        public void AddSentryAmmo(int ammo)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                Ammo += ammo;
            else
            {
                ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
                packet.Write((byte)MessageType.SyncSentryAmmo);
                packet.Write((byte)NPC.whoAmI);
                packet.Write(ammo);
                packet.Send(-1, Main.myPlayer);
            }
        }

        public void AddSentryRocketAmmo(int ammo)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                RocketAmmo += ammo;
            else
            {
                ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
                packet.Write((byte)MessageType.SyncSentryRocketAmmo);
                packet.Write((byte)NPC.whoAmI);
                packet.Write(ammo);
                packet.Send(-1, Main.myPlayer);
            }
        }

        protected override void BuildingSendExtraAI(BinaryWriter writer)
        {
            writer.Write(Ammo);
            writer.Write(RocketAmmo);
            writer.Write(AttackAnimationTimer);
            writer.Write(rocketAttackAnimationTimer);
            writer.Write(wrangled);
            writer.Write(wranglerCooldown);
            writer.Write(scanTimer);
            writer.Write(playTargetSound);
        }

        protected override void BuildingReceiveExtraAI(BinaryReader reader)
        {
            Ammo = reader.ReadInt32();
            RocketAmmo = reader.ReadInt32();
            AttackAnimationTimer = reader.ReadInt32();
            rocketAttackAnimationTimer = reader.ReadInt32();
            wrangled = reader.ReadBoolean();
            wranglerCooldown = reader.ReadInt32();
            scanTimer = reader.ReadInt32();
            playTargetSound = reader.ReadBoolean();
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (wrangled)
                modifiers.FinalDamage *= 0.66666f;
        }
    }

    public class SentryStatistics
    {
        public int Type { get; set; }

        public int Metal { get; set; }

        public int Ammo { get; set; }

        public int RocketAmmo { get; set; }
    }

    public class SentryLevel1 : TF2Sentry
    {
        protected override Asset<Texture2D> BuildingTexture => BuildingTextures.SentryLevel1Texture;

        protected override Asset<Texture2D> BuildingTextureAir => BuildingTextures.SentryLevel1AirTexture;

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

        protected override void SentrySpawn() => Timer = Time(0.2);

        protected override void SentryAI()
        {
            UpgradeBuilding(ModContent.NPCType<SentryLevel2>());
            frame = AttackAnimationTimer <= 0 ? 0 : 1;
            Timer++;
            if (AttackAnimationTimer > 0)
                AttackAnimationTimer--;
            float distanceFromTarget = 500f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && Timer >= Time(0.2) && !wrangled || (Timer >= Time(0.1) && wrangled && Player.controlUseItem && Player.HeldItem.ModItem is Wrangler))
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
                if (wrangled && Player.controlUseItem && Player.HeldItem.ModItem is Wrangler)
                {
                    targetCenter = Main.MouseWorld;
                    foundTarget = true;
                }
                if (foundTarget && Ammo > 0)
                {
                    if (!playTargetSound && !wrangled)
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
                    int damage = Round(16 * Player.GetModPlayer<TF2Player>().damageMultiplier);
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_shoot"), NPC.Center);
                    NPCCreateProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, Owner);
                    AttackAnimationTimer = Time(0.25);
                    Ammo--;
                    Timer = 0;
                    NPC.netUpdate = true;
                }
                else
                    playTargetSound = false;
            }
            if (!playTargetSound && !wrangled)
            {
                scanTimer++;
                if (scanTimer % Time(2) == 0)
                {
                    sentrySoundSlot = SoundEngine.PlaySound(SentrySound, NPC.Center);
                    scanTimer = 0;
                }
            }
        }
    }

    public class SentryLevel2 : TF2Sentry
    {
        protected override Asset<Texture2D> BuildingTexture => BuildingTextures.SentryLevel2Texture;

        protected override Asset<Texture2D> BuildingTextureAir => BuildingTextures.SentryLevel2AirTexture;

        protected override SoundStyle SentrySound => new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_scan2");

        public override int Rounds => 200;

        public override int InitialHealth => 180;

        public override int BuildingCooldown => Time(1.6);

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.aiStyle = -1;
            NPC.lifeMax = 180;
            NPC.damage = 16;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_explode");
        }

        protected override void SentrySpawn() => Timer = Time(0.1);

        protected override void SentryAI()
        {
            UpgradeBuilding(ModContent.NPCType<SentryLevel3>());
            frame = AttackAnimationTimer <= 0 ? 0 : 1;
            Timer++;
            if (AttackAnimationTimer > 0)
                AttackAnimationTimer--;
            float distanceFromTarget = 500f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && Timer >= Time(0.1) && !wrangled || (Timer >= Time(0.05) && wrangled && Player.controlUseItem && Player.HeldItem.ModItem is Wrangler))
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
                if (wrangled && Player.controlUseItem && Player.HeldItem.ModItem is Wrangler)
                {
                    targetCenter = Main.MouseWorld;
                    foundTarget = true;
                }
                if (foundTarget && Ammo > 0)
                {
                    if (!playTargetSound && !wrangled)
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
                    int damage = Round(16 * Player.GetModPlayer<TF2Player>().damageMultiplier);
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_shoot"), NPC.Center);
                    NPCCreateProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, Owner);
                    AttackAnimationTimer = Time(0.25);
                    Ammo--;
                    Timer = 0;
                    NPC.netUpdate = true;
                }
                else
                    playTargetSound = false;
            }
            if (!playTargetSound && !wrangled)
            {
                scanTimer++;
                if (scanTimer % Time(2) == 0)
                {
                    sentrySoundSlot = SoundEngine.PlaySound(SentrySound, NPC.Center);
                    scanTimer = 0;
                }
            }
        }
    }

    public class SentryLevel3 : TF2Sentry
    {
        protected override Asset<Texture2D> BuildingTexture => BuildingTextures.SentryLevel3Texture;

        protected override Asset<Texture2D> BuildingTextureAir => BuildingTextures.SentryLevel3AirTexture;

        protected override Asset<Texture2D> BuildingSecondaryTexture => BuildingTextures.SentryLevel3SecondaryTexture;

        protected override Asset<Texture2D> BuildingSecondaryTextureAir => BuildingTextures.SentryLevel3SecondaryAirTexture;

        protected override SoundStyle SentrySound => new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_scan3");

        public override int Rounds => 200;

        public override int InitialHealth => 216;

        public override int BuildingCooldown => Time(1.6);

        public override void SetDefaults()
        {
            NPC.width = 42;
            NPC.height = 42;
            NPC.aiStyle = -1;
            NPC.lifeMax = 216;
            NPC.damage = 16;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_explode");
        }

        protected override void SentryDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Timer2 >= Time(3) && !wrangled || Timer2 >= Time(1.5) && wrangled || rocketAttackAnimationTimer > 0)
            {
                Texture2D sprite = !air ? BuildingSecondaryTexture.Value : BuildingSecondaryTextureAir.Value;
                int height = sprite.Height / 2;
                spriteBatch.Draw(sprite, NPC.position - screenPos, new Rectangle(0, 0, sprite.Width, height), drawColor, 0f, new Vector2(NPC.direction == 1 ? sprite.Width - NPC.width : 0, height - NPC.height), NPC.scale, (SpriteEffects)((NPC.direction == 1) ? 1 : 0), 0f);
            }
        }

        protected override void SentrySpawn()
        {
            Timer = Time(0.1);
            Timer2 = Time(3);
        }

        protected override void SentryAI()
        {
            frame = AttackAnimationTimer <= 0 ? 0 : 1;
            Timer++;
            Timer2++;
            if (AttackAnimationTimer > 0)
                AttackAnimationTimer--;
            if (rocketAttackAnimationTimer > 0)
                rocketAttackAnimationTimer--;
            float distanceFromTarget = 500f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget)
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
                if (wrangled && Player.controlUseItem && Player.HeldItem.ModItem is Wrangler)
                {
                    targetCenter = Main.MouseWorld;
                    foundTarget = true;
                }
                if (foundTarget)
                {
                    if (!playTargetSound && (Ammo > 0 || RocketAmmo > 0) && !wrangled)
                    {
                        if (SoundEngine.TryGetActiveSound(sentrySoundSlot, out var scanSound))
                            scanSound?.Stop();
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_spot"), NPC.Center);
                        playTargetSound = true;
                    }
                    if (Ammo > 0 && (Timer >= Time(0.1) && !wrangled || (Timer >= Time(0.05) && wrangled && Main.mouseLeft && Player.HeldItem.ModItem is Wrangler)))
                    {
                        Vector2 shootVel = NPC.DirectionTo(targetCenter);
                        if ((targetCenter - NPC.Center).X > 0f)
                            NPC.spriteDirection = NPC.direction = 1;
                        else if ((targetCenter - NPC.Center).X < 0f)
                            NPC.spriteDirection = NPC.direction = -1;
                        float speed = 10f;
                        int type = ModContent.ProjectileType<SentryBullet>();
                        int damage = Round(16 * Player.GetModPlayer<TF2Player>().damageMultiplier);
                        IEntitySource projectileSource = NPC.GetSource_FromAI();
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_shoot"), NPC.Center);
                        NPCCreateProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, Owner);
                        AttackAnimationTimer = Time(0.25);
                        Ammo--;
                        Timer = 0;
                        NPC.netUpdate = true;
                    }
                    if (RocketAmmo > 0 && (Timer2 >= Time(3) && !wrangled || (Timer2 >= Time(1.5) && wrangled && Main.mouseRight && Player.HeldItem.ModItem is Wrangler)))
                    {
                        Vector2 shootVel = NPC.DirectionTo(targetCenter);
                        if ((targetCenter - NPC.Center).X > 0f)
                            NPC.spriteDirection = NPC.direction = 1;
                        else if ((targetCenter - NPC.Center).X < 0f)
                            NPC.spriteDirection = NPC.direction = -1;
                        float speed = 25f;
                        int type = ModContent.ProjectileType<RocketNPC>();
                        int damage = Round(100 * Player.GetModPlayer<TF2Player>().damageMultiplier);
                        IEntitySource projectileSource = NPC.GetSource_FromAI();
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_rocket"), NPC.Center);
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 newVelocity = shootVel.RotatedByRandom(MathHelper.ToRadians(15f));
                            NPCCreateProjectile(projectileSource, NPC.Center, newVelocity * speed, type, damage, 0f, Owner);
                        }
                        rocketAttackAnimationTimer = Time(1);
                        RocketAmmo--;
                        Timer2 = 0;
                        NPC.netUpdate = true;
                    }
                }
                if (!foundTarget || (Ammo <= 0 && RocketAmmo <= 0))
                    playTargetSound = false;
            }
            if (!playTargetSound && !wrangled)
            {
                scanTimer++;
                if (scanTimer % Time(2) == 0)
                {
                    sentrySoundSlot = SoundEngine.PlaySound(SentrySound, NPC.Center);
                    scanTimer = 0;
                }
            }
        }
    }

    public class MiniSentry : TF2Sentry
    {
        public override string BuildingName => Language.GetTextValue("Mods.TF2.NPCs.MiniSentryGun");

        protected override Asset<Texture2D> BuildingTexture => BuildingTextures.MiniSentryTexture;

        protected override Asset<Texture2D> BuildingTextureAir => BuildingTextures.MiniSentryAirTexture;

        protected override Asset<Texture2D> BuildingSecondaryTexture => BuildingTextures.MiniSentrySecondaryTexture;

        protected override Asset<Texture2D> BuildingSecondaryTextureAir => BuildingTextures.MiniSentrySecondaryAirTexture;

        public override int InitialHealth => 50;

        public override int BuildingCooldown => Time(4.2f);

        public override int BuildingCooldownHauled => Time(2.333);

        protected override int ScrapMetalAmount => 0;

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 24;
            NPC.aiStyle = -1;
            NPC.damage = 8;
            NPC.lifeMax = 100;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_explode");
        }

        protected override void SentryDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D sprite = !air ? BuildingSecondaryTexture.Value : BuildingSecondaryTextureAir.Value;
            int height = sprite.Height / 2;
            spriteBatch.Draw(sprite, NPC.position - screenPos, new Rectangle(0, height * frame, sprite.Width, sprite.Height / 2), Color.White, 0f, new Vector2(NPC.direction == 1 ? sprite.Width - NPC.width : 0, height - NPC.height), NPC.scale, (SpriteEffects)((NPC.direction == 1) ? 1 : 0), 0f);
        }

        protected override void SentrySpawn() => Timer = Time(0.15);

        protected override void SentryAI()
        {
            frame = AttackAnimationTimer <= 0 ? 0 : 1;
            Timer++;
            if (AttackAnimationTimer > 0)
                AttackAnimationTimer--;
            float distanceFromTarget = 500f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && Timer >= Time(0.15) && !wrangled || (Timer >= Time(0.075) && wrangled && Player.controlUseItem && Player.HeldItem.ModItem is Wrangler))
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
                if (wrangled && Player.controlUseItem && Player.HeldItem.ModItem is Wrangler)
                {
                    targetCenter = Main.MouseWorld;
                    foundTarget = true;
                }
                if (foundTarget && Ammo > 0)
                {
                    if (!playTargetSound && !wrangled)
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
                    int damage = Round(8 * Player.GetModPlayer<TF2Player>().damageMultiplier);
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_shoot_mini"), NPC.Center);
                    NPCCreateProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, Owner);
                    AttackAnimationTimer = Time(0.25);
                    Ammo--;
                    Timer = 0;
                    NPC.netUpdate = true;
                }
                else
                    playTargetSound = false;
            }
            if (!playTargetSound && !wrangled)
            {
                scanTimer++;
                if (scanTimer % Time(2) == 0)
                {
                    sentrySoundSlot = SoundEngine.PlaySound(SentrySound, NPC.Center);
                    scanTimer = 0;
                }
            }
        }
    }
}