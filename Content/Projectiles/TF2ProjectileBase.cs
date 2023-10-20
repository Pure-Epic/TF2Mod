using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TF2.Common;

// Purposely part of Content, not Common because it only affects TF2 projectiles
namespace TF2.Content.Projectiles
{
    public class TF2ProjectileBase : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public int timer;
        public bool spawnedFromNPC;
        public int owner;
        public bool miniCrit;
        public bool crit;
        public bool healingProjectile;
        public bool sniperMiniCrit; // Exclusive to the Sydney Sleeper
        public bool sniperCrit; // Exclusive to any Sniper Rifle (except for the Sydney Sleeper)
        public bool lEtrangerProjectile;
        public bool homing;
        public float shootSpeed = 10f;

        public override void AI(Projectile projectile)
        {
            timer++;
            /*
            if ((sniperCrit || crit) && !spawnedFromNPC)
                Main.player[projectile.owner].GetModPlayer<TF2Player>().crit = true;
            if ((sniperMiniCrit || miniCrit) && !spawnedFromNPC)
                Main.player[projectile.owner].GetModPlayer<TF2Player>().miniCrit = true;
            */
            if (homing)
            {
                /*
                for (int num178 = 0; num178 < 10; num178++)
                {
                    float x2 = projectile.position.X - projectile.velocity.X / 10f * num178;
                    float y2 = projectile.position.Y - projectile.velocity.Y / 10f * num178;
                    int num179 = Dust.NewDust(new Vector2(x2, y2), 1, 1, DustID.CursedTorch);
                    Main.dust[num179].alpha = projectile.alpha;
                    Main.dust[num179].position.X = x2;
                    Main.dust[num179].position.Y = y2;
                    Main.dust[num179].velocity *= 0f;
                    Main.dust[num179].noGravity = true;
                }
                */
                float projectileSqrt = (float)Math.Sqrt(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y);
                float ai = projectile.localAI[0];
                if (ai == 0f)
                {
                    projectile.localAI[0] = projectileSqrt;
                    ai = projectileSqrt;
                }
                if (projectile.alpha > 0)
                    projectile.alpha -= 25;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
                float projectileX = projectile.position.X;
                float projectileY = projectile.position.Y;
                float maxDetectRadius = Main.player[projectile.owner].GetModPlayer<TF2Player>().homingPower switch
                {
                    0 => 250f,
                    1 => 1250f,
                    2 => 2500f,
                    _ => 0f,
                };
                bool canSeek = false;
                int nextAI = 0;
                if (projectile.ai[1] == 0f)
                {
                    for (int i = 0; i < 200; i++)
                    {
                        if (Main.npc[i].CanBeChasedBy(this) && (projectile.ai[1] == 0f || projectile.ai[1] == i + 1))
                        {
                            float npcX = Main.npc[i].position.X + (Main.npc[i].width / 2);
                            float npcY = Main.npc[i].position.Y + (Main.npc[i].height / 2);
                            float closestNPCDistance = Math.Abs(projectile.position.X + (projectile.width / 2) - npcX) + Math.Abs(projectile.position.Y + (projectile.height / 2) - npcY);
                            if (closestNPCDistance < maxDetectRadius && Collision.CanHit(new Vector2(projectile.position.X + (projectile.width / 2), projectile.position.Y + (projectile.height / 2)), 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
                            {
                                maxDetectRadius = closestNPCDistance;
                                projectileX = npcX;
                                projectileY = npcY;
                                canSeek = true;
                                nextAI = i;
                            }
                        }
                    }
                    if (canSeek)
                        projectile.ai[1] = nextAI + 1;
                    canSeek = false;
                }
                if (projectile.ai[1] > 0f)
                {
                    int previousAI = (int)(projectile.ai[1] - 1f);
                    if (Main.npc[previousAI].active && Main.npc[previousAI].CanBeChasedBy(this, ignoreDontTakeDamage: true) && !Main.npc[previousAI].dontTakeDamage)
                    {
                        float npcX = Main.npc[previousAI].position.X + (Main.npc[previousAI].width / 2);
                        float npcY = Main.npc[previousAI].position.Y + (Main.npc[previousAI].height / 2);
                        if (Math.Abs(projectile.position.X + (projectile.width / 2) - npcX) + Math.Abs(projectile.position.Y + (projectile.height / 2) - npcY) < 1000f)
                        {
                            canSeek = true;
                            projectileX = Main.npc[previousAI].position.X + (Main.npc[previousAI].width / 2);
                            projectileY = Main.npc[previousAI].position.Y + (Main.npc[previousAI].height / 2);
                        }
                    }
                    else
                        projectile.ai[1] = 0f;
                }
                if (canSeek)
                {
                    float newAI = ai;
                    Vector2 vector25 = new(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
                    float newProjectileX = projectileX - vector25.X;
                    float newProjectileY = projectileY - vector25.Y;
                    float newProjectileSqrt = (float)Math.Sqrt(newProjectileX * newProjectileX + newProjectileY * newProjectileY);
                    newProjectileSqrt = newAI / newProjectileSqrt;
                    newProjectileX *= newProjectileSqrt;
                    newProjectileY *= newProjectileSqrt;
                    int swerveDistance = 10;
                    projectile.velocity.X = (projectile.velocity.X * (swerveDistance - 1) + newProjectileX) / swerveDistance;
                    projectile.velocity.Y = (projectile.velocity.Y * (swerveDistance - 1) + newProjectileY) / swerveDistance;
                }
            }
            projectile.netUpdate = true;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC)
                spawnedFromNPC = true;
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if ((sniperCrit || crit) && !spawnedFromNPC)
                Main.player[projectile.owner].GetModPlayer<TF2Player>().crit = true;
            if ((sniperMiniCrit || miniCrit) && !spawnedFromNPC)
                Main.player[projectile.owner].GetModPlayer<TF2Player>().miniCrit = true;
        }

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info) => homing = false;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) => homing = false;

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(timer);
            bitWriter.WriteBit(homing);
            bitWriter.WriteBit(spawnedFromNPC);
            bitWriter.WriteBit(crit);
            bitWriter.WriteBit(miniCrit);
            bitWriter.WriteBit(sniperCrit);
            bitWriter.WriteBit(sniperMiniCrit);
            bitWriter.WriteBit(lEtrangerProjectile);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            timer = binaryReader.ReadInt32();
            homing = bitReader.ReadBit();
            spawnedFromNPC = bitReader.ReadBit();
            crit = bitReader.ReadBit();
            miniCrit = bitReader.ReadBit();
            sniperCrit = bitReader.ReadBit();
            sniperMiniCrit = bitReader.ReadBit();
            sniperMiniCrit = bitReader.ReadBit();
        }
    }
}