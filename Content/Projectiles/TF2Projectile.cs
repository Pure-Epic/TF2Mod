using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.Items.Weapons;

namespace TF2.Content.Projectiles
{
    public abstract class TF2Projectile : ModProjectile
    {
        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        protected Player Player => Main.player[Projectile.owner];

        protected bool projectileInitialized;
        public bool spawnedFromNPC;
        public int owner;
        public TF2Weapon weapon;
        public bool noDistanceModifier;
        protected Vector2 velocity;
        public bool miniCrit;
        public bool crit;
        public bool healingProjectile;
        public bool sniperMiniCrit; // Exclusive to the Sydney Sleeper
        public bool sniperCrit; // Exclusive to any Sniper Rifle (except for the Sydney Sleeper)
        public bool ammoShot;
        public bool healthShot;
        public bool lEtrangerProjectile;
        public bool homing;
        public float shootSpeed = 10f;

        protected virtual void ProjectileStatistics()
        { }

        protected virtual void ProjectileInitialize()
        { }

        protected virtual bool ProjectileDraw(Projectile projectile, ref Color lightColor)
        {
            Main.instance.LoadProjectile(projectile.type);
            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, projectile.height * 0.5f);
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = projectile.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }

        protected virtual bool ProjectilePreAI() => true;

        protected virtual void ProjectileAI()
        { }

        protected virtual bool ProjectileTileCollide(Vector2 oldVelocity) => true;

        protected virtual void ProjectileHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        { }

        protected virtual void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        { }

        protected virtual void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        { }

        protected virtual void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        { }

        protected virtual void ProjectileDestroy(int timeLeft)
        { }

        protected virtual void ProjectileSendExtraAI(BinaryWriter writer)
        { }

        protected virtual void ProjectileReceiveExtraAI(BinaryReader binaryReader)
        { }

        protected void SetProjectileSize(int width = 25, int height = 25) => Projectile.Size = new Vector2(width, height);     

        protected void SetRotation(float rotation = 0) => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(rotation);

        public sealed override void SetDefaults()
        {
            ProjectileStatistics();
            Projectile.DamageType = ModContent.GetInstance<TF2DamageClass>();
        }

        public override bool PreDraw(ref Color lightColor) => ProjectileDraw(Projectile, ref lightColor);

        public sealed override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC)
                spawnedFromNPC = true;
            ProjectileInitialize();
        }

        public sealed override bool PreAI() => ProjectilePreAI();

        public sealed override void AI()
        {
            Timer++;
            ProjectileAI();
            if (crit || miniCrit)
                Projectile.penetrate = -1;
            /*
            if (crit)
            {
                for (int i = 0; i < 10; i++)
                {
                    float x2 = Projectile.position.X - Projectile.velocity.X / 10f * i;
                    float y2 = Projectile.position.Y - Projectile.velocity.Y / 10f * i;
                    int num179 = Dust.NewDust(new Vector2(x2, y2), 1, 1, DustID.Clentaminator_Red);
                    Main.dust[num179].alpha = Projectile.alpha;
                    Main.dust[num179].position.X = x2;
                    Main.dust[num179].position.Y = y2;
                    Main.dust[num179].velocity *= 0f;
                    Main.dust[num179].noGravity = true;
                }
            }
            */
            if (homing)
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
        }

        public sealed override bool OnTileCollide(Vector2 oldVelocity) => ProjectileTileCollide(oldVelocity);

        public sealed override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (modifiers.PvP)
                ProjectileHitPlayer(target, ref modifiers);
        }

        public sealed override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DamageVariationScale *= 0;
            if ((sniperCrit || crit) && !spawnedFromNPC)
                Player.GetModPlayer<TF2Player>().crit = true;
            if ((sniperMiniCrit || miniCrit) && !spawnedFromNPC)
                Player.GetModPlayer<TF2Player>().miniCrit = true;
            ProjectileHitNPC(target, ref modifiers);
        }

        public sealed override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            homing = false;
            ProjectilePostHitPlayer(target, info);
        }

        public sealed override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            homing = false;
            ProjectilePostHitNPC(target, hit, damageDone);
        }

        public sealed override void OnKill(int timeLeft) => ProjectileDestroy(timeLeft);

        public sealed override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Timer);
            writer.Write(projectileInitialized);
            writer.Write(homing);
            writer.Write(spawnedFromNPC);
            writer.Write(owner);
            writer.WriteVector2(velocity);
            writer.Write(crit);
            writer.Write(miniCrit);
            writer.Write(sniperCrit);
            writer.Write(sniperMiniCrit);
            writer.Write(lEtrangerProjectile);
            writer.Write(ammoShot);
            writer.Write(healthShot);
            ProjectileSendExtraAI(writer);
        }

        public sealed override void ReceiveExtraAI(BinaryReader binaryReader)
        {
            Timer = binaryReader.ReadInt32();
            projectileInitialized = binaryReader.ReadBoolean();
            homing = binaryReader.ReadBoolean();
            spawnedFromNPC = binaryReader.ReadBoolean();
            owner = binaryReader.ReadInt32();
            velocity = binaryReader.ReadVector2();
            crit = binaryReader.ReadBoolean();
            miniCrit = binaryReader.ReadBoolean();
            sniperCrit = binaryReader.ReadBoolean();
            sniperMiniCrit = binaryReader.ReadBoolean();
            lEtrangerProjectile = binaryReader.ReadBoolean();
            ammoShot = binaryReader.ReadBoolean();
            healthShot = binaryReader.ReadBoolean();
            ProjectileReceiveExtraAI(binaryReader);
        }
    }
}