using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.NPCs;
using TF2.Content.Projectiles.Pyro;

namespace TF2.Content.NPCs
{
    public abstract class ClassNPC : ModNPC
    {
        public int npcOwner = 0;

        public bool lifeInitialized = false;

        public override bool CheckActive() => false;

        public override bool PreAI()
        {
            if (!lifeInitialized)
            {
                TF2Player p = Main.player[npcOwner].GetModPlayer<TF2Player>();
                NPC.life = (int)(NPC.lifeMax * p.classMultiplier);
                lifeInitialized = true;
            }
            NPC.spriteDirection = NPC.direction;
            return true;
        }

        public override void PostAI()
        {
            if (NPC.defense > 100)
                NPC.defense = 100;
        }

        public void Kill() => NPC.life = 0;
    }

    public class ScoutNPC : ClassNPC
    {
        private int ai;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            ContentSamples.NpcBestiaryRarityStars[Type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 72;
            NPC.aiStyle = 7;
            NPC.damage = 15;
            NPC.stepSpeed = 2f;
            NPC.defense = 0;
            NPC.lifeMax = 125;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/scout_painsevere01");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/scout_paincriticaldeath01");
            NPC.friendly = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("The youngest of eight boys from the south side of Boston, the Scout learned early how to solve problems with his fists. With seven older brothers on his side, fights tended to end before the runt of the litter could maneuver into punching distance, so the Scout trained himself to run. He ran everywhere, all the time, until he could beat his pack of mad dog siblings to the fray."),
            });
        }

        public override void AI()
        {
            TF2Player p = Main.player[npcOwner].GetModPlayer<TF2Player>();
            NPC.lifeMax = (int)(125 * p.classMultiplier);
            NPC.defense = (int)(20 * p.classMultiplier);
            ai += 1;
            float distanceFromTarget = 700f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && ai >= 9) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                // This code is required either way, used for finding a target
                foreach (NPC targetNPC in Main.npc)
                {
                    ai = 0;
                    if (targetNPC.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(targetNPC.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNPC.position, targetNPC.width, targetNPC.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = targetNPC.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = NPC.DirectionTo(targetCenter);
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 10f;
                    int type = ModContent.ProjectileType<Bullet>();
                    int damage = (int)(15 * p.classMultiplier);
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/pistol_shoot"), NPC.Center);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        TF2.CreateProjectile(null, projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner);
                    else
                        NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner));
                }
            }
        }
    }

    public class SoldierNPC : ClassNPC
    {
        private int ai;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            ContentSamples.NpcBestiaryRarityStars[Type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 56;
            NPC.height = 102;
            NPC.aiStyle = 7;
            NPC.damage = 90;
            NPC.defense = 0;
            NPC.lifeMax = 200;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/soldier_painsevere01");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/soldier_paincriticaldeath01");
            NPC.friendly = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Though he wanted desperately to fight in World War 2, the Soldier was rejected from every branch of the U.S. military. Undaunted, he bought his own ticket to Europe. After arriving and finally locating Poland, the Soldier taught himself how to load and fire a variety of weapons before embarking on a Nazi killing spree for which he was awarded several medals that he designed and made himself. His rampage ended immediately upon hearing about the end of the war in 1949."),
            });
        }

        public override void AI()
        {
            TF2Player p = Main.player[npcOwner].GetModPlayer<TF2Player>();
            NPC.lifeMax = (int)(200 * p.classMultiplier);
            NPC.defense = (int)(20 * p.classMultiplier);
            ai += 1;
            float distanceFromTarget = 700f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && ai >= 48) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                // This code is required either way, used for finding a target
                foreach (NPC targetNPC in Main.npc)
                {
                    ai = 0;
                    if (targetNPC.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(targetNPC.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNPC.position, targetNPC.width, targetNPC.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = targetNPC.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = NPC.DirectionTo(targetCenter);
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 25f;
                    int type = ModContent.ProjectileType<SentryRocket>();
                    int damage = (int)(90 * p.classMultiplier);
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/rocket_shoot"), NPC.Center);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        TF2.CreateProjectile(null, projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner);
                    else
                        NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner));
                }
            }
        }
    }

    public class PyroNPC : ClassNPC
    {
        private int ai;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            ContentSamples.NpcBestiaryRarityStars[Type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 80;
            NPC.aiStyle = 7;
            NPC.damage = 30;
            NPC.defense = 0;
            NPC.lifeMax = 175;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/pyro_painsevere01");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/pyro_paincriticaldeath01");
            NPC.friendly = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Only two things are known for sure about the mysterious Pyro: he sets things on fire and he doesn't speak. In fact, only the part about setting things on fire is undisputed. Some believe his occasional rasping wheeze may be an attempt to communicate through a mouth obstructed by a filter and attached to lungs ravaged by constant exposure to his asbestos-lined suit. Either way, he's a fearsome, inscrutable, on - fire Frankenstein of a man.If he even is a man."),
            });
        }

        public override void AI()
        {
            TF2Player p = Main.player[npcOwner].GetModPlayer<TF2Player>();
            NPC.lifeMax = (int)(175 * p.classMultiplier);
            NPC.defense = (int)(20 * p.classMultiplier);
            ai += 1;
            float distanceFromTarget = 700f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && ai >= 36) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                // This code is required either way, used for finding a target
                foreach (NPC targetNPC in Main.npc)
                {
                    ai = 0;
                    if (targetNPC.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(targetNPC.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNPC.position, targetNPC.width, targetNPC.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = targetNPC.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = NPC.DirectionTo(targetCenter);
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 25f;
                    int type = ModContent.ProjectileType<Flare>();
                    int damage = (int)(30 * p.classMultiplier);
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(SoundID.Item11, NPC.Center);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        TF2.CreateProjectile(null, projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner);
                    else
                        NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner));
                }
            }
        }
    }

    public class DemomanNPC : ClassNPC
    {
        private int ai;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            ContentSamples.NpcBestiaryRarityStars[Type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 63;
            NPC.height = 89;
            NPC.aiStyle = 7;
            NPC.damage = 100;
            NPC.defense = 0;
            NPC.lifeMax = 175;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/demoman_painsevere01");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/demoman_paincriticaldeath01");
            NPC.friendly = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("A fierce temper, a fascination with all things explosive, and a terrible plan to kill the Loch Ness Monster cost the six year old Demoman his original set of adoptive parents. Later, at the Crypt Grammar School for Orphans near Ullapool in the Scottish Highlands, the boy's bomb-making skills improved dramatically. His disposition and total number of intact eyeballs, however, did not. Word of his proficiency with explosives spread, and it was not long before Crypt Grammar received two visitors; the Demoman's real parents, who lovingly explained that all Demomen are abandoned at birth until their skills manifest themselves, a long-standing, cruel, and wholly unnecessary tradition among the Highland Demolition Men. His unhappy childhood had ended, but his training had just begun."),
            });
        }

        public override void AI()
        {
            TF2Player p = Main.player[npcOwner].GetModPlayer<TF2Player>();
            NPC.lifeMax = (int)(175 * p.classMultiplier);
            NPC.defense = (int)(20 * p.classMultiplier);
            ai += 1;
            float distanceFromTarget = 700f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && ai >= 36) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                // This code is required either way, used for finding a target
                foreach (NPC targetNPC in Main.npc)
                {
                    ai = 0;
                    if (targetNPC.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(targetNPC.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNPC.position, targetNPC.width, targetNPC.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = targetNPC.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = NPC.DirectionTo(targetCenter);
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 12.5f;
                    int type = ModContent.ProjectileType<GrenadeNPC>();
                    int damage = (int)(100 * p.classMultiplier);
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/grenade_launcher_shoot"), NPC.Center);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        TF2.CreateProjectile(null, projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner);
                    else
                        NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner));
                }
            }
        }
    }

    public class HeavyNPC : ClassNPC
    {
        private int ai;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            ContentSamples.NpcBestiaryRarityStars[Type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 68;
            NPC.height = 92;
            NPC.aiStyle = 7;
            NPC.damage = 9;
            NPC.defense = 0;
            NPC.lifeMax = 300;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/heavy_painsevere01");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/heavy_paincriticaldeath01");
            NPC.friendly = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Like a hibernating bear, the Heavy appears to be a gentle giant. Also like a bear, confusing his deliberate, sleepy demeanor with gentleness will get you ripped limb from limb. Though he speaks simply and moves with an economy of energy that's often confused with napping, the Heavy isn't dumb; he's not your big friend and he generally wishes that you would just shut up before he has to make you shut up."),
            });
        }

        public override void AI()
        {
            TF2Player p = Main.player[npcOwner].GetModPlayer<TF2Player>();
            NPC.lifeMax = (int)(300 * p.classMultiplier);
            NPC.defense = (int)(20 * p.classMultiplier);
            ai += 1;
            float distanceFromTarget = 700f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && ai >= 6) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                // This code is required either way, used for finding a target
                foreach (NPC targetNPC in Main.npc)
                {
                    ai = 0;
                    if (targetNPC.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(targetNPC.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNPC.position, targetNPC.width, targetNPC.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = targetNPC.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = NPC.DirectionTo(targetCenter);
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 10f;
                    Vector2 newVelocity = shootVel.RotatedByRandom(MathHelper.ToRadians(10f));
                    int type = ModContent.ProjectileType<Bullet>();
                    int damage = (int)(9 * p.classMultiplier);
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(SoundID.Item11, NPC.Center);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        TF2.CreateProjectile(null, projectileSource, NPC.Center, newVelocity * speed, type, damage, 0f, npcOwner);
                    else
                        NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner));
                }
            }
        }
    }

    public class EngineerNPC : ClassNPC
    {
        private int ai;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            ContentSamples.NpcBestiaryRarityStars[Type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 55;
            NPC.height = 90;
            NPC.aiStyle = 7;
            NPC.damage = 6;
            NPC.defense = 0;
            NPC.lifeMax = 125;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/engineer_painsevere01");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/engineer_paincriticaldeath01");
            NPC.friendly = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("This amiable, soft-spoken good ol' boy from tiny Bee Cave, Texas loves barbeque, guns, and higher education. Natural curiosity, ten years as a roughneck in the west Texas oilfields, and eleven hard science PhDs have trained him to design, build and repair a variety of deadly contraptions."),
            });
        }

        public override void AI()
        {
            TF2Player p = Main.player[npcOwner].GetModPlayer<TF2Player>();
            NPC.lifeMax = (int)(125 * p.classMultiplier);
            NPC.defense = (int)(20 * p.classMultiplier);
            ai += 1;
            float distanceFromTarget = 700f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && ai >= 38) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                // This code is required either way, used for finding a target
                foreach (NPC targetNPC in Main.npc)
                {
                    ai = 0;
                    if (targetNPC.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(targetNPC.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNPC.position, targetNPC.width, targetNPC.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = targetNPC.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = NPC.DirectionTo(targetCenter);
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 10f;
                    int type = ModContent.ProjectileType<Bullet>();
                    int damage = (int)(6 * p.classMultiplier);
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/shotgun_shoot"), NPC.Center);
                    for (int i = 0; i < 10; i++)
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Vector2 newVelocity = shootVel.RotatedByRandom(MathHelper.ToRadians(12f));
                            TF2.CreateProjectile(null, projectileSource, NPC.Center, newVelocity * speed, type, damage, 0f, npcOwner);
                        }
                        else
                        {
                            Vector2 newVelocity = shootVel.RotatedByRandom(MathHelper.ToRadians(12f));
                            NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, newVelocity * speed, type, damage, 0f, npcOwner));
                        }
                    }
                }
            }
        }
    }

    public class MedicNPC : ClassNPC
    {
        private int ai;
        private int heal;
        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            ContentSamples.NpcBestiaryRarityStars[Type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.aiStyle = 3;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 150;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/medic_painsevere01");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/medic_paincriticaldeath01");
            NPC.friendly = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("What he lacks in compassion for the sick, respect for human dignity, and any sort of verifiable formal training in medicine, the Medic more than makes up for with a bottomless supply of giant needles and a trembling enthusiasm for plunging them into exposed flesh. Raised in Stuttgart, Germany during an era when the Hippocratic oath had been downgraded to an optional Hippocratic suggestion, the Medic considers healing a generally unintended side effect of satisfying his own morbid curiosity."),
            });
        }

        public override void AI()
        {
            TF2Player p = Main.player[npcOwner].GetModPlayer<TF2Player>();
            NPC.lifeMax = (int)(150 * p.classMultiplier);
            NPC.defense = (int)(20 * p.classMultiplier);
            heal += 1;
            ai += 1;
            if (heal >= 12 && NPC.life < NPC.lifeMax)
            {
                heal = 0;
                NPC.life += (int)p.classMultiplier;
                if (NPC.life > NPC.lifeMax)
                    NPC.life = NPC.lifeMax;
            }
            float distanceFromTarget = 700f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && ai >= 96) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                // This code is required either way, used for finding a target
                foreach (Player targetPlayer in Main.player)
                {
                    if (targetPlayer.active)
                    {
                        ai = 0;
                        float between = Vector2.Distance(targetPlayer.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetPlayer.position, targetPlayer.width, targetPlayer.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = targetPlayer.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = NPC.DirectionTo(targetCenter);
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 25f;
                    int type = ModContent.ProjectileType<SyringeNPC>();
                    int damage = 75;
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/syringegun_shoot"), NPC.Center);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        TF2.CreateProjectile(null, projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner);
                    else
                        NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner));
                }
            }
        }
    }

    public class SniperNPC : ClassNPC
    {
        private int ai;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            ContentSamples.NpcBestiaryRarityStars[Type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 69;
            NPC.height = 95;
            NPC.aiStyle = 7;
            NPC.damage = 450;
            NPC.stepSpeed = 2f;
            NPC.defense = 0;
            NPC.lifeMax = 125;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/sniper_painsevere01");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/sniper_paincriticaldeath01");
            NPC.friendly = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Half rugged outdoorsman, half alien observer, this taciturn strip of beef jerky has spent the better part of his life alone in the bush, slow baking under the Australian sun."),
            });
        }

        public override void AI()
        {
            TF2Player p = Main.player[npcOwner].GetModPlayer<TF2Player>();
            NPC.lifeMax = (int)(125 * p.classMultiplier);
            NPC.defense = (int)(20 * p.classMultiplier);
            ai += 1;
            float distanceFromTarget = 1400f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && ai >= 600) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                // This code is required either way, used for finding a target
                foreach (NPC targetNPC in Main.npc)
                {
                    ai = 0;
                    if (targetNPC.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(targetNPC.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNPC.position, targetNPC.width, targetNPC.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = targetNPC.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = NPC.DirectionTo(targetCenter);
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 10f;
                    int type = ModContent.ProjectileType<Bullet>();
                    int damage = (int)(450 * p.classMultiplier);
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/pistol_shoot"), NPC.Center);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        TF2.CreateProjectile(null, projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner);
                    else
                        NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner));
                }
            }
        }
    }

    public class SpyNPC : ClassNPC
    {
        private int ai;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            ContentSamples.NpcBestiaryRarityStars[Type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 47;
            NPC.height = 85;
            NPC.aiStyle = 7;
            NPC.damage = 240;
            NPC.defense = 0;
            NPC.lifeMax = 125;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/spy_painsevere01");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/spy_paincriticaldeath01");
            NPC.friendly = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("He is a puzzle, wrapped in an enigma, shrouded in riddles, lovingly sprinkled with intrigue, express mailed to Mystery, Alaska, and LOOK OUT BEHIND YOU! but it is too late. You're dead. For he is the Spy - globetrotting rogue, lady killer (metaphorically) and mankiller (for real)."),
            });
        }

        public override void AI()
        {
            TF2Player p = Main.player[npcOwner].GetModPlayer<TF2Player>();
            NPC.lifeMax = (int)(125 * p.classMultiplier);
            NPC.defense = (int)(20 * p.classMultiplier);
            ai += 1;
            float distanceFromTarget = 350f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && ai >= 48) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                // This code is required either way, used for finding a target
                foreach (NPC targetNPC in Main.npc)
                {
                    ai = 0;
                    if (targetNPC.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(targetNPC.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNPC.position, targetNPC.width, targetNPC.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = targetNPC.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = NPC.DirectionTo(targetCenter);
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 0f;
                    int type = ModContent.ProjectileType<KnifeProjectileNPC>();
                    int damage = (int)(240 * p.classMultiplier);
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/knife_swing"), NPC.Center);
                    if ((targetCenter - NPC.Center).Y >= 0f)
                        NPC.velocity = new Vector2(12.5f * NPC.direction, 12.5f);
                    if ((targetCenter - NPC.Center).Y <= 0f)
                        NPC.velocity = new Vector2(12.5f * NPC.direction, -12.5f);
                    TF2Projectile projectile = TF2.CreateProjectile(null, projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner);
                    KnifeProjectileNPC spawnedModProjectile = (KnifeProjectileNPC)projectile;
                    spawnedModProjectile.thisNPC = NPC;
                    NetMessage.SendData(MessageID.SyncProjectile, number: projectile.Projectile.whoAmI);
                }
            }
        }
    }
}