using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace TF2.Content.Projectiles.Demoman
{
    public class Stickybomb : TF2Projectile
    {
        public bool Stick
        {
            get => (int)Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value ? 1f : 0f;
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

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(25, 25);
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Stick = false;
        }

        protected override bool ProjectilePreAI()
        {
            if (!Stick)
                Projectile.velocity.Y += 0.2f;
            return true;
        }

        protected override void ProjectileAI()
        {
            if (Timer >= TF2.Time(5))
                noDistanceModifier = true;
            if (Projectile.timeLeft == 0)
            {
                Projectile.position = Projectile.Center;
                Projectile.Size = new Vector2(250, 250);
                Projectile.friendly = true;
                Projectile.hide = true;
                Projectile.tileCollide = false;
                Projectile.tileCollide = false;
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

        protected override bool ProjectileTileCollide(Vector2 oldVelocity)
        {
            velocity = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            Stick = true;
            return false;
        }

        protected override void ProjectileDestroy(int timeLeft) => TF2.Explode(Projectile, new SoundStyle("TF2/Content/Sounds/SFX/explode"));

        public void StartingAI()
        {
            if (!Stick)
                Projectile.timeLeft = 3600;
            if (Projectile.timeLeft == 0)
            {
                Stick = true;
                Projectile.velocity = Vector2.Zero;
            }
            SetRotation();
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

        public virtual void StickyJump(Vector2 velocity)
        {
            if (TF2.FindPlayer(Projectile, 50f))
            {
                velocity *= 2.5f;
                velocity.X = Utils.Clamp(velocity.X, -25f, 25f);
                velocity.Y = Utils.Clamp(velocity.Y, -25f, 25f);
                Player.velocity -= velocity;
                if (Player.immuneNoBlink) return;
                int selfDamage = TF2.GetHealth(Player, 80);
                Player.Hurt(PlayerDeathReason.ByCustomReason(Player.name + " " + TF2.TF2DeathMessagesLocalization[2]), selfDamage, 0);
            }
        }
    }
}