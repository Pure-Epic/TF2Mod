using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Content.Items;

namespace TF2.Content.Projectiles.Demoman
{
    public class Stickybomb : ModProjectile
    {
        public bool Stick
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }

        public int Timer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public int TargetWhoAmI
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        public bool StickOnEnemy
        {
            get => Projectile.localAI[0] == 1f;
            set => Projectile.localAI[0] = value ? 1f : 0f;
        }

        public const int maxPower = 5;
        public Vector2 velocity;
        public TF2Weapon owner;

        public override void SetDefaults()
        {
            Projectile.width = 25;                  // The width of projectile hitbox
            Projectile.height = 25;                 // The height of projectile hitbox
            Projectile.aiStyle = 0;                 // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = false;            // Can the projectile deal damage to enemies?
            Projectile.hostile = false;             // Can the projectile deal damage to the player?
            Projectile.penetrate = -1;              // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 3600;             // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0;                   // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0f;                  // How much light emit around the projectile
            Projectile.ignoreWater = true;          // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true;          // Can the projectile collide with tiles?
            Projectile.extraUpdates = 1;            // Set to above 0 if you want the projectile to update multiple times in a frame
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Stick = false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            velocity = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            Stick = true;
            return false;
        }

        public override bool PreDraw(ref Color lightColor) => TF2.DrawProjectile(Projectile, ref lightColor);

        public override bool PreAI()
        {
            if (!Stick)
                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            return true;
        }

        public override void AI()
        {
            Timer++;
            if (Projectile.timeLeft == 0)
            {
                Projectile.friendly = true;
                Projectile.tileCollide = false;
                Projectile.alpha = 255;
                Projectile.position = Projectile.Center;
                Projectile.width = 250;
                Projectile.height = 250;
                Projectile.Center = Projectile.position;
                StickyJump(velocity);
            }
            if (!Stick && Projectile.timeLeft != 0)
                StartingAI();
            else
                GroundAI();
            foreach (NPC npc in Main.npc)
            {
                if (Projectile.Hitbox.Intersects(npc.Hitbox) && npc.boss && !npc.friendly && npc.active && !npc.dontTakeDamage)
                {
                    TargetWhoAmI = npc.whoAmI;
                    Stick = true;
                    StickOnEnemy = true;
                    Projectile.netUpdate = true;
                }
            }
        }

        public void StartingAI()
        {
            if (!Stick)
                Projectile.timeLeft = 3600;
            if (Projectile.timeLeft == 0)
            {
                Stick = true;
                Projectile.velocity = Vector2.Zero;
            }
            Projectile.rotation = Projectile.position.ToRotation() + MathHelper.ToRadians(90f);
        }

        public void GroundAI()
        {
            if (Projectile.timeLeft > 0)
                Projectile.timeLeft = 3600;
            if (StickOnEnemy)
            {
                Projectile.tileCollide = false;

                int npcTarget = TargetWhoAmI;
                if (npcTarget < 0 || npcTarget >= 200)
                {
                    Projectile.tileCollide = true;
                    Stick = false;
                    StickOnEnemy = false;
                    Projectile.netUpdate = true;
                }
                else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage)
                {
                    Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
                }
                else
                {
                    Projectile.tileCollide = true;
                    Stick = false;
                    StickOnEnemy = false;
                    Projectile.netUpdate = true;
                }
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

        public override void OnKill(int timeLeft) => TF2.Explode(Projectile, new SoundStyle("TF2/Content/Sounds/SFX/explode1"), 5);

        public virtual void StickyJump(Vector2 velocity)
        {
            if (TF2.FindPlayer(Projectile, 50f))
            {
                Player player = Main.player[Projectile.owner];
                velocity *= 2.5f;
                velocity.X = Utils.Clamp(velocity.X, -25f, 25f);
                velocity.Y = Utils.Clamp(velocity.Y, -25f, 25f);
                Main.player[Projectile.owner].velocity -= velocity;
                if (player.immuneNoBlink) return;
                int selfDamage = Convert.ToInt32(Math.Floor(player.statLifeMax2 * 0.25f));
                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " blew themself to smithereens."), selfDamage, 0);
            }
        }
    }
}