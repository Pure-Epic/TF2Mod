using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using TF2.Common;

namespace TF2.Content.Projectiles.Spy
{
    public class YourEternalRewardBeam : TF2Projectile
    {
        protected override void ProjectileStatistics()
        {
            Projectile.CloneDefaults(ProjectileID.Bullet);
            Projectile.width = 69;
            Projectile.height = 16;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            AIType = ProjectileID.Bullet;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override bool ProjectilePreAI()
        {
            if (projectileInitialized) return true;
            TF2Player p = Player.GetModPlayer<TF2Player>();
            Projectile.penetrate = p.pierce;
            homing = true;
            projectileInitialized = true;
            return true;
        }

        protected override void ProjectileAI()
        {
            SetRotation();
            for (int num178 = 0; num178 < 10; num178++)
            {
                float x2 = Projectile.Center.X - Projectile.velocity.X / 10f * num178;
                float y2 = Projectile.Center.Y - Projectile.velocity.Y / 10f * num178;
                int num179 = Dust.NewDust(new Vector2(x2, y2), 1, 1, DustID.IceTorch);
                Main.dust[num179].alpha = Projectile.alpha;
                Main.dust[num179].position.X = x2;
                Main.dust[num179].position.Y = y2;
                Main.dust[num179].velocity *= 0f;
                Main.dust[num179].noGravity = true;
            }
            if (homing)
            {
                float projectileSqrt = (float)Math.Sqrt(Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y);
                float ai = Projectile.localAI[0];
                if (ai == 0f)
                {
                    Projectile.localAI[0] = projectileSqrt;
                    ai = projectileSqrt;
                }
                if (Projectile.alpha > 0)
                    Projectile.alpha -= 25;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
                float projectileX = Projectile.position.X;
                float projectileY = Projectile.position.Y;
                float maxDetectRadius = 100f;
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
        }

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info) => homing = false;

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => homing = false;
    }
}