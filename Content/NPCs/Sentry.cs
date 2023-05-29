using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.NPCs;
using TF2.Common;

namespace TF2.Content.NPCs
{
    public abstract class Sentry : ModNPC
    {
        public int npcOwner = 0;
        public int cooldown;

        public float npcPower = 1;

        public bool lifeInitialized = false;

        public bool wrangled;

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

            if (Main.player[npcOwner].HeldItem.ModItem is Items.Engineer.Wrangler)
            {
                wrangled = true;
            }
            else if (wrangled)
            {
                cooldown++;
                if (cooldown >= 180)
                {
                    wrangled = false;
                    cooldown = 0;
                }
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

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (wrangled)
                damage = (int)(damage * 0.66f);
        }

        public override void OnKill()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.player[npcOwner].GetModPlayer<Items.Engineer.FrontierJusticePlayer>().revenge += 3;
        }

        public void Kill() => NPC.life = 0;

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

    public class SentryLevel1 : Sentry
    {
        private int ai;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sentry Gun Level 1");
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 50;
            NPC.aiStyle = -1;
            NPC.damage = 16;
            NPC.lifeMax = 150;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/wrench_hit_build_success1");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/sentry_explode");
            NPC.friendly = true;
            NPC.GetGlobalNPC<TF2GlobalNPC>().building = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("The answer: use a gun."),
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
            float distanceFromTarget = 700f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if ((!foundTarget && ai >= 12 && !wrangled) || (ai >= 6 && wrangled && Main.player[npcOwner].channel)) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    ai = 0;
                    NPC targetNpc = Main.npc[i];
                    if (targetNpc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(targetNpc.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNpc.position, targetNpc.width, targetNpc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            if (!wrangled)
                                targetCenter = targetNpc.Center;
                            else
                                targetCenter = Main.MouseWorld;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = targetCenter - NPC.Center;
                    if (shootVel == Vector2.Zero)
                        shootVel = Vector2.UnitY;
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 10f;
                    int type = ModContent.ProjectileType<Bullet>();
                    int damage;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        damage = (int)(16 * p.classMultiplier);
                    else
                        damage = (int)(16 * npcPower);
                    var projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/sentry_shoot"), NPC.Center);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f);
                    else
                        NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f));
                }
            }
        }
    }

    public class SentryLevel2 : Sentry
    {
        private int ai;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sentry Gun Level 2");
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 63;
            NPC.aiStyle = -1;
            NPC.damage = 16;
            NPC.lifeMax = 180;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/wrench_hit_build_success1");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/sentry_explode");
            NPC.friendly = true;
            NPC.GetGlobalNPC<TF2GlobalNPC>().building = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("And if that don't work... use more gun."),
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
            float distanceFromTarget = 700f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if ((!foundTarget && ai >= 6 && !wrangled) || (ai >= 3 && wrangled && Main.player[npcOwner].channel))
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    ai = 0;
                    NPC targetNpc = Main.npc[i];
                    if (targetNpc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(targetNpc.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNpc.position, targetNpc.width, targetNpc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            if (!wrangled)
                                targetCenter = targetNpc.Center;
                            else
                                targetCenter = Main.MouseWorld;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = targetCenter - NPC.Center;
                    if (shootVel == Vector2.Zero)
                        shootVel = Vector2.UnitY;
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 10f;
                    int type = ModContent.ProjectileType<Bullet>();
                    int damage;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        damage = (int)(16 * p.classMultiplier);
                    else
                        damage = (int)(16 * npcPower);
                    var projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/sentry_shoot"), NPC.Center);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f);
                    else
                        NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f));
                }
            }
        }
    }

    public class SentryLevel3 : Sentry
    {
        private int ai;
        private int ai2;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sentry Gun Level 3");
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 69;
            NPC.aiStyle = -1;
            NPC.damage = 16;
            NPC.lifeMax = 216;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/wrench_hit_build_success1");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/sentry_explode");
            NPC.friendly = true;
            NPC.GetGlobalNPC<TF2GlobalNPC>().building = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("More gun and four rockets? This is insane!"),
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
            float distanceFromTarget = 700f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            bool foundTarget2 = false;
            if ((!foundTarget && ai >= 6 && !wrangled) || (ai >= 3 && wrangled && Main.player[npcOwner].channel && Main.player[npcOwner].altFunctionUse != 2))// && Main.netMode != NetmodeID.MultiplayerClient
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    ai = 0;
                    NPC targetNpc = Main.npc[i];
                    if (targetNpc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(targetNpc.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNpc.position, targetNpc.width, targetNpc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            if (!wrangled)
                                targetCenter = targetNpc.Center;
                            else
                                targetCenter = Main.MouseWorld;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = targetCenter - NPC.Center;
                    if (shootVel == Vector2.Zero)
                        shootVel = Vector2.UnitY;
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;

                    float speed = 10f;
                    int type = ModContent.ProjectileType<Bullet>();
                    int damage;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        damage = (int)(16 * p.classMultiplier);
                    else
                        damage = (int)(16 * npcPower);
                    var projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/sentry_shoot"), NPC.Center);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f);
                    else
                        NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f));
                }


            }

            if ((!foundTarget && ai2 >= 180 && !wrangled) || (ai2 >= 90 && wrangled && Main.player[npcOwner].channel && Main.player[npcOwner].altFunctionUse == 2))// && Main.netMode != NetmodeID.MultiplayerClient
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    ai2 = 0;
                    NPC targetNpc = Main.npc[i];
                    if (targetNpc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(targetNpc.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNpc.position, targetNpc.width, targetNpc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget2) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            if (!wrangled)
                                targetCenter = targetNpc.Center;
                            else
                                targetCenter = Main.MouseWorld;
                            foundTarget2 = true;
                        }
                    }
                }
                if (foundTarget2)
                {
                    Vector2 shootVel = targetCenter - NPC.Center;
                    if (shootVel == Vector2.Zero)
                        shootVel = Vector2.UnitY;
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 10f;
                    int type = ModContent.ProjectileType<SentryRocket>();
                    int damage;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        damage = (int)(100 * p.classMultiplier);
                    else
                        damage = (int)(100 * npcPower);
                    var projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/sentry_rocket"), NPC.Center);
                    for (int i = 0; i < 4; i++)
                    {
                        //Main.NewText("Rocket", Color.White);
                        Vector2 newVelocity = shootVel.RotatedByRandom(MathHelper.ToRadians(15f));
                        if (Main.netMode == NetmodeID.SinglePlayer)
                            Projectile.NewProjectile(projectileSource, NPC.Center, newVelocity * speed, type, damage, 0f, npcOwner, 0f, 0f);
                        else
                            NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f));
                    }
                }
            }
        }
    }

    public class MiniSentry : Sentry
    {
        private int ai;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mini-Sentry Gun");
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 35;
            NPC.height = 35;
            NPC.aiStyle = -1;
            NPC.damage = 8;
            NPC.defense = 0;
            NPC.lifeMax = 100;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/wrench_hit_build_success1");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/sentry_explode");
            NPC.friendly = true;
            NPC.GetGlobalNPC<TF2GlobalNPC>().building = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("I love that little gun!"),
            });
        }

        public override void AI()
        {
            TF2Player p = Main.player[npcOwner].GetModPlayer<TF2Player>();
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                NPC.lifeMax = (int)(100 * p.classMultiplier);
                NPC.defense = (int)(20 * p.classMultiplier);
            }
            else
            {
                NPC.lifeMax = (int)(100 * npcPower);
                NPC.defense = (int)(20 * npcPower);
            }
            ai += 1;
            float distanceFromTarget = 700f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if ((!foundTarget && ai >= 9 && !wrangled) || (ai >= 4 && wrangled && Main.player[npcOwner].channel)) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    ai = 0;
                    NPC targetNpc = Main.npc[i];
                    if (targetNpc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(targetNpc.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNpc.position, targetNpc.width, targetNpc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            if (!wrangled)
                                targetCenter = targetNpc.Center;
                            else
                                targetCenter = Main.MouseWorld;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = targetCenter - NPC.Center;
                    if (shootVel == Vector2.Zero)
                        shootVel = Vector2.UnitY;
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 10f;
                    int type = ModContent.ProjectileType<Bullet>();
                    int damage;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        damage = (int)(8 * p.classMultiplier);
                    else
                        damage = (int)(8 * npcPower);
                    var projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/sentry_shoot"), NPC.Center);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f);
                    else
                        NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f));
                }
            }
        }
    }
}