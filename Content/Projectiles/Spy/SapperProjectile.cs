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
    public class SapperProjectile : ModProjectile
    {
        public bool StickOnEnemy
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }

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

        private const int GravityDelay = 45;

        public override void SetDefaults()
        {
            Projectile.width = 32;                  // The width of Projectile hitbox
            Projectile.height = 32;                 // The height of Projectile hitbox
            Projectile.aiStyle = 0;                 // The ai style of the Projectile, please reference the source code of Terraria
            Projectile.friendly = true;             // Can the Projectile deal damage to enemies?
            Projectile.hostile = false;             // Can the Projectile deal damage to the player?
            Projectile.penetrate = 2;               // How many monsters the Projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600;              // The live time for the Projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 255;                 // The transparency of the Projectile, 255 for completely transparent. (aiStyle 1 quickly fades the Projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your Projectile is invisible.
            Projectile.ignoreWater = true;          // Does the Projectile's speed be influenced by water?
            Projectile.tileCollide = true;          // Can the Projectile collide with tiles?
            Projectile.hide = true;                 // Makes the Projectile completely invisible. We need this to draw our Projectile behind enemies/tiles in DrawBehind()
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool PreDraw(ref Color lightColor) => TF2.DrawProjectile(Projectile, ref lightColor);

        public override void AI()
        {
            UpdateAlpha();
            // Run either the sapping AI or passive AI
            // Separating into different methods helps keeps your AI clean
            if (!StickOnEnemy)
                StartingAI();
            else
                SapAI();
        }

        private void StartingAI()
        {
            if (Projectile.GetGlobalProjectile<TF2ProjectileBase>().homing)
            {
                float ProjectileSqrt = (float)Math.Sqrt(Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y);
                float ai = Projectile.localAI[0];
                if (ai == 0f)
                {
                    Projectile.localAI[0] = ProjectileSqrt;
                    ai = ProjectileSqrt;
                }
                if (Projectile.alpha > 0)
                    Projectile.alpha -= 25;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
                float ProjectileX = Projectile.position.X;
                float ProjectileY = Projectile.position.Y;
                float maxDetectRadius = Main.player[Projectile.owner].GetModPlayer<TF2Player>().homingPower switch
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
                                ProjectileX = npcX;
                                ProjectileY = npcY;
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
                            ProjectileX = Main.npc[previousAI].position.X + (Main.npc[previousAI].width / 2);
                            ProjectileY = Main.npc[previousAI].position.Y + (Main.npc[previousAI].height / 2);
                        }
                    }
                    else
                        Projectile.ai[1] = 0f;
                }
                if (canSeek)
                {
                    float newAI = ai;
                    Vector2 vector25 = new(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                    float newProjectileX = ProjectileX - vector25.X;
                    float newProjectileY = ProjectileY - vector25.Y;
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

            GravityDelayTimer++; // Doesn't make sense.

            // For a little while, the sapper will travel with the same speed, but after this, the javelin drops velocity very quickly.
            if (GravityDelayTimer >= GravityDelay)
            {
                GravityDelayTimer = GravityDelay;

                // Wind resistance
                Projectile.velocity.X *= 0.98f;
                // Gravity
                Projectile.velocity.Y += 0.12f;
            }

            // Offset the rotation by 90 degrees because the sprite is oriented vertiacally.
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        private const int stickTime = 600;

        private void SapAI()
        {
            Projectile.ignoreWater = true; // Make sure the Projectile ignores water
            Projectile.tileCollide = false; // Make sure the Projectile doesn't collide with tiles anymore
            StickTimer += 1f;

            int npcTarget = TargetWhoAmI;
            if (StickTimer >= stickTime || npcTarget < 0 || npcTarget >= 200)
                Projectile.Kill(); // If the index is past its limits, kill it
            else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage)
            {
                // If the target is active and can take damage
                // Set the Projectile's position relative to the target's center
                Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
                Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;

                // The damage and stun comes from the debuff, not the Projectile
            }
            else
                Projectile.Kill(); // Otherwise, kill the Projectile
        }

        public override void OnKill(int timeLeft)
        {
            if (StickTimer < stickTime) return;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position); // Play a death sound
            Vector2 usePos = Projectile.position; // Position to use for dusts

            Vector2 rotationVector = (Projectile.rotation - MathHelper.ToRadians(0f)).ToRotationVector2(); // rotation vector to use for dust velocity
            usePos += rotationVector * 16f;

            // Spawn some dusts upon javelin death
            for (int i = 0; i < 20; i++)
            {
                // Create a new dust
                Dust dust = Dust.NewDustDirect(usePos, Projectile.width, Projectile.height, DustID.Electric);
                dust.position = (dust.position + Projectile.Center) / 2f;
                dust.velocity += rotationVector * 2f;
                dust.velocity *= 0.5f;
                dust.noGravity = true;
                usePos -= rotationVector * 8f;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            SappedPlayer sappedPlayer = target.GetModPlayer<SappedPlayer>();
            sappedPlayer.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<Sapped>(), 600);
        }

        private const int maxSappers = 1; // This is the max. amount of sappers being able to attach
        private readonly Point[] activeSappers = new Point[maxSappers]; // The point array holding for sticking sappers

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            SappedNPC npc = target.GetGlobalNPC<SappedNPC>();
            npc.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<Sapped>(), 600);
            StickOnEnemy = true;
            TargetWhoAmI = target.whoAmI; // Set the target whoAmI
            Projectile.netUpdate = true;
            Projectile.damage = 0;
            Projectile.KillOldestJavelin(Projectile.whoAmI, Type, target.whoAmI, activeSappers);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            // If attached to an NPC, draw behind tiles (and the npc) if that NPC is behind tiles, otherwise just behind the NPC.
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
            // Since we aren't attached, add to this list
            behindNPCsAndTiles.Add(index);
        }

        // Change this number if you want to alter how the alpha changes
        private const int AlphaFadeInSpeed = 25;

        private void UpdateAlpha()
        {
            // Slowly remove alpha as it is present
            if (Projectile.alpha > 0)
                Projectile.alpha -= AlphaFadeInSpeed;

            // If alpha gets lower than 0, set it to 0
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
        }
    }
}