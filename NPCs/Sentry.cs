using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Projectiles;
using TF2.Projectiles.NPCs;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;

namespace TF2.NPCs
{
    public abstract class Sentry : ModNPC
    {
        public int npcOwner = 0;

        public bool lifeInitialized = false;

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.Name == "Wrench_TF2")
            {
                damage = 0;
                if (NPC.life >= NPC.lifeMax)
                {
                    NPC.life = NPC.lifeMax;
                    return;
                }
                TFClass p = Main.player[projectile.owner].GetModPlayer<TFClass>();
                int cost = 102;
                if (!(p.metal >= cost / 3)) { return; }
                NPC.life += cost * (int)p.classMultiplier;
                p.metal -= cost / 3;
            }
        }

        public override bool CheckActive() => false;

        public override bool PreAI()
        {
            if (!lifeInitialized)
            {
                TFClass p = Main.player[npcOwner].GetModPlayer<TFClass>();
                NPC.life = (int)(NPC.lifeMax * p.classMultiplier);
                lifeInitialized = true;
            }
            return true;
        }

        public override void PostAI()
        {
            if (NPC.life >= NPC.lifeMax)
            {
                NPC.life = NPC.lifeMax;
                return;
            }
        }

        public void Kill()
        {
            NPC.life = 0;
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
        }
        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 50;
            NPC.aiStyle = -1;
            NPC.damage = 6;
            NPC.defense = 10;
            NPC.lifeMax = 150;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Sounds/SFX/wrench_hit_build_success1");
            NPC.DeathSound = new SoundStyle("TF2/Sounds/SFX/sentry_explode");
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
            TFClass p = Main.player[npcOwner].GetModPlayer<TFClass>();
            NPC.lifeMax = (int)(150 * p.classMultiplier);
            NPC.defense = (int)(20 * p.classMultiplier);
            ai += 1;
            float distanceFromTarget = 700f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && ai >= 12) //&& Main.netMode != NetmodeID.MultiplayerClient
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
                            targetCenter = targetNpc.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = targetCenter - NPC.Center;
                    if (shootVel == Vector2.Zero)
                    {
                        shootVel = new Vector2(0f, 1f);
                    }
                    if ((targetCenter - NPC.Center).X > 0f)
                    {
                        NPC.spriteDirection = NPC.direction = 1;
                    }
                    else if ((targetCenter - NPC.Center).X < 0f)
                    {
                        NPC.spriteDirection = NPC.direction = -1;
                    }
                    float speed = 10f;
                    int type = ModContent.ProjectileType<Bullet>();
                    int damage = (int)(12 * p.classMultiplier);
                    var projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/sentry_shoot"), NPC.Center);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f));
                    }
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
        }
        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 63;
            NPC.aiStyle = -1;
            NPC.damage = 6;
            NPC.defense = 15;
            NPC.lifeMax = 180;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Sounds/SFX/wrench_hit_build_success1");
            NPC.DeathSound = new SoundStyle("TF2/Sounds/SFX/sentry_explode");
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
            TFClass p = Main.player[npcOwner].GetModPlayer<TFClass>();
            NPC.lifeMax = (int)(180 * p.classMultiplier);
            NPC.defense = (int)(30 * p.classMultiplier);
            ai += 1;
            float distanceFromTarget = 700f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && ai >= 6)
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
                            targetCenter = targetNpc.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = targetCenter - NPC.Center;
                    if (shootVel == Vector2.Zero)
                    {
                        shootVel = new Vector2(0f, 1f);
                    }
                    if ((targetCenter - NPC.Center).X > 0f)
                    {
                        NPC.spriteDirection = NPC.direction = 1;
                    }
                    else if ((targetCenter - NPC.Center).X < 0f)
                    {
                        NPC.spriteDirection = NPC.direction = -1;
                    }
                    float speed = 10f;
                    int type = ModContent.ProjectileType<Bullet>();
                    int damage = (int)(12 * p.classMultiplier);
                    var projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/sentry_shoot"), NPC.Center);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f));
                    }
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
        }
        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 69;
            NPC.aiStyle = -1;
            NPC.damage = 6;
            NPC.defense = 20;
            NPC.lifeMax = 216;
            NPC.knockBackResist = 0f;
            NPC.HitSound = new SoundStyle("TF2/Sounds/SFX/wrench_hit_build_success1");
            NPC.DeathSound = new SoundStyle("TF2/Sounds/SFX/sentry_explode");
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
            TFClass p = Main.player[npcOwner].GetModPlayer<TFClass>();
            NPC.lifeMax = (int)(216 * p.classMultiplier);
            NPC.defense = (int)(40 * p.classMultiplier);
            ai += 1;
            ai2 += 1;
            float distanceFromTarget = 700f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            bool foundTarget2 = false;
            if (!foundTarget && ai >= 6)// && Main.netMode != NetmodeID.MultiplayerClient
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
                            targetCenter = targetNpc.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = targetCenter - NPC.Center;
                    if (shootVel == Vector2.Zero)
                    {
                        shootVel = new Vector2(0f, 1f);
                    }
                    if ((targetCenter - NPC.Center).X > 0f)
                    {
                        NPC.spriteDirection = NPC.direction = 1;
                    }
                    else if ((targetCenter - NPC.Center).X < 0f)
                    {
                        NPC.spriteDirection = NPC.direction = -1;
                    }

                    float speed = 10f;
                    int type = ModContent.ProjectileType<Bullet>();
                    int damage = (int)(12 * p.classMultiplier);
                    var projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/sentry_shoot"), NPC.Center);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f));
                    }
                }


            }

            if (!foundTarget2 && ai2 >= 180)// && Main.netMode != NetmodeID.MultiplayerClient
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
                            targetCenter = targetNpc.Center;
                            foundTarget2 = true;
                        }
                    }
                }
                if (foundTarget2)
                {
                    Vector2 shootVel = targetCenter - NPC.Center;
                    if (shootVel == Vector2.Zero)
                    {
                        shootVel = new Vector2(0f, 1f);
                    }
                    if ((targetCenter - NPC.Center).X > 0f)
                    {
                        NPC.spriteDirection = NPC.direction = 1;
                    }
                    else if ((targetCenter - NPC.Center).X < 0f)
                    {
                        NPC.spriteDirection = NPC.direction = -1;
                    }
                    float speed = 10f;
                    int type = ModContent.ProjectileType<SentryRocket>();
                    int damage = (int)(100 * p.classMultiplier);
                    var projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/sentry_rocket"), NPC.Center);
                    for (int i = 0; i < 4; i++)
                    {
                        //Main.NewText("Rocket", Color.White);
                        Vector2 newVelocity = shootVel.RotatedByRandom(MathHelper.ToRadians(15));
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f);
                        }
                        else
                        {
                            NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, npcOwner, 0f, 0f));
                        }
                    }
                }
            }
        }
    }
}