using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Spy
{
    public class SapperProjectile : TF2Projectile
    {
        public override string Texture => "TF2/Content/Items/Weapons/Spy/Sapper";

        public int TargetWhoAmI
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public int GravityDelayTimer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public float StickTimer
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        public bool StickOnEnemy
        {
            get => Projectile.localAI[0] == 1f;
            set => Projectile.localAI[0] = value ? 1f : 0f;
        }


        private const int maxSappers = 1;
        private readonly Point[] activeSappers = new Point[maxSappers];
        private readonly int gravityDelay = TF2.Time(0.75);
        private readonly int stickTime = TF2.Time(10);

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(10, 10);
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = TF2.Time(10);
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override void ProjectileAI()
        {
            if (!StickOnEnemy)
                StartingAI();
            else
                SapAI();
        }

        private void StartingAI()
        {
            if (homing)
            {
                float ProjectileSqrt = (float)Math.Sqrt(Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y);
                float ai = Projectile.localAI[0];
                if (ai == 0f)
                {
                    Projectile.localAI[0] = ProjectileSqrt;
                    ai = ProjectileSqrt;
                }
                float projectileX = Projectile.position.X;
                float projectileY = Projectile.position.Y;
                float maxDetectRadius = Player.GetModPlayer<TF2Player>().homingPower switch
                {
                    0 => 250f,
                    1 => 1250f,
                    2 => 2500f,
                    _ => 0f,
                };
                bool canSeek = false;
                int nextAI = 0;
                if (Projectile.ai[1] == 0f)
                {
                    for (int i = 0; i < 200; i++)
                    {
                        if (Main.npc[i].CanBeChasedBy(this) && (Projectile.ai[1] == 0f || Projectile.ai[1] == i + 1))
                        {
                            float npcX = Main.npc[i].position.X + (Main.npc[i].width / 2);
                            float npcY = Main.npc[i].position.Y + (Main.npc[i].height / 2);
                            float closestNPCDistance = Math.Abs(Projectile.position.X + (Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (Projectile.height / 2) - npcY);
                            if (closestNPCDistance < maxDetectRadius && Collision.CanHit(new Vector2(Projectile.position.X + (Projectile.width / 2), Projectile.position.Y + (Projectile.height / 2)), 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
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
                        Projectile.ai[1] = nextAI + 1;
                    canSeek = false;
                }
                if (Projectile.ai[1] > 0f)
                {
                    int previousAI = (int)(Projectile.ai[1] - 1f);
                    if (Main.npc[previousAI].active && Main.npc[previousAI].CanBeChasedBy(this, ignoreDontTakeDamage: true) && !Main.npc[previousAI].dontTakeDamage)
                    {
                        float npcX = Main.npc[previousAI].position.X + (Main.npc[previousAI].width / 2);
                        float npcY = Main.npc[previousAI].position.Y + (Main.npc[previousAI].height / 2);
                        if (Math.Abs(Projectile.position.X + (Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (Projectile.height / 2) - npcY) < 1000f)
                        {
                            canSeek = true;
                            projectileX = Main.npc[previousAI].position.X + (Main.npc[previousAI].width / 2);
                            projectileY = Main.npc[previousAI].position.Y + (Main.npc[previousAI].height / 2);
                        }
                    }
                    else
                        Projectile.ai[1] = 0f;
                }
                if (canSeek)
                {
                    float newAI = ai;
                    Vector2 vector25 = new(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                    float newProjectileX = projectileX - vector25.X;
                    float newProjectileY = projectileY - vector25.Y;
                    float newProjectileSqrt = (float)Math.Sqrt(newProjectileX * newProjectileX + newProjectileY * newProjectileY);
                    newProjectileSqrt = newAI / newProjectileSqrt;
                    newProjectileX *= newProjectileSqrt;
                    newProjectileY *= newProjectileSqrt;
                    int swerveDistance = 10;
                    Projectile.velocity.X = (Projectile.velocity.X * (swerveDistance - 1) + newProjectileX) / swerveDistance;
                    Projectile.velocity.Y = (Projectile.velocity.Y * (swerveDistance - 1) + newProjectileY) / swerveDistance;
                }
            }
            Projectile.netUpdate = true;
            GravityDelayTimer++;
            if (GravityDelayTimer >= gravityDelay)
            {
                GravityDelayTimer = gravityDelay;
                Projectile.velocity.X *= 0.98f;
                Projectile.velocity.Y += 0.12f;
            }
            SetRotation();
        }

        private void SapAI()
        {
            Projectile.tileCollide = false;
            StickTimer += 1f;
            int npcTarget = TargetWhoAmI;
            if (StickTimer >= stickTime || npcTarget < 0 || npcTarget >= 200)
                Projectile.Kill();
            else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage)
            {
                Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
                Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
            }
            else
                Projectile.Kill();
        }

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            SappedPlayer sappedPlayer = target.GetModPlayer<SappedPlayer>();
            sappedPlayer.damageMultiplier = p.damageMultiplier;
            target.AddBuff(ModContent.BuffType<Sapped>(), TF2.Time(10), true);
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            SappedNPC npc = target.GetGlobalNPC<SappedNPC>();
            npc.damageMultiplier = p.damageMultiplier;
            target.AddBuff(ModContent.BuffType<Sapped>(), TF2.Time(10));
            StickOnEnemy = true;
            TargetWhoAmI = target.whoAmI;
            Projectile.netUpdate = true;
            Projectile.damage = 0;
            Projectile.KillOldestJavelin(Projectile.whoAmI, Type, target.whoAmI, activeSappers);
        }

        protected override void ProjectileDestroy(int timeLeft)
        {
            if (StickTimer < stickTime) return;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Vector2 usePos = Projectile.position;
            Vector2 rotationVector = (Projectile.rotation - MathHelper.ToRadians(0f)).ToRotationVector2(); // rotation vector to use for dust velocity
            usePos += rotationVector * 16f;
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(usePos, Projectile.width, Projectile.height, DustID.Electric);
                dust.position = (dust.position + Projectile.Center) / 2f;
                dust.velocity += rotationVector * 2f;
                dust.velocity *= 0.5f;
                dust.noGravity = true;
                usePos -= rotationVector * 8f;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (StickOnEnemy)
            {
                int npcIndex = TargetWhoAmI;
                if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
                {
                    if (Main.npc[npcIndex].behindTiles)
                        behindNPCsAndTiles.Add(index);
                    else
                        behindNPCsAndTiles.Add(index);
                    return;
                }
            }
            behindNPCsAndTiles.Add(index);
        }
    }
}