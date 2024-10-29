using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.Items.Weapons;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.NPCs.Buddies;
using TF2.Content.NPCs.Enemies;

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
        public int npcOwner;
        public TF2Weapon weapon;
        public bool noDistanceModifier;
        protected Vector2 velocity;
        public bool miniCrit;
        public bool crit;
        public bool healingProjectile;
        public bool sniperMiniCrit;
        public bool sniperCrit;
        public bool backStab;
        public bool ammoShot;
        public bool healthShot;
        public bool reserveShooterProjectile;
        public bool bazaarBargainProjectile;
        public bool lEtrangerProjectile;
        public bool homing;
        public float shootSpeed = 10f;
        private float distance;
        private byte target;

        protected virtual void ProjectileStatistics()
        { }

        protected virtual void ProjectileInitialize(IEntitySource source)
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

        protected static void KnockbackPlayer(Player player, ref Player.HurtModifiers modifiers, float knockbackPower)
        {
            if (modifiers.HitDirection < 0 && player.velocity.X > -knockbackPower)
            {
                if (player.velocity.X > 0f)
                    player.velocity.X -= knockbackPower;
                player.velocity.X -= knockbackPower;
                TF2.Minimum(ref player.velocity.X, -knockbackPower);
            }
            else if (modifiers.HitDirection > 0 && player.velocity.X < knockbackPower)
            {
                if (player.velocity.X < 0f)
                    player.velocity.X += knockbackPower;
                player.velocity.X += knockbackPower;
                TF2.Minimum(ref player.velocity.X, knockbackPower);
            }
            knockbackPower *= -0.5f;
            if (player.velocity.Y > knockbackPower)
            {
                player.velocity.Y += knockbackPower;
                TF2.Minimum(ref player.velocity.Y, knockbackPower);
            }
        }

        protected static void KnockbackPlayer(Player player, int direction, float knockbackPower)
        {
            if (direction < 0 && player.velocity.X > -knockbackPower)
            {
                if (player.velocity.X > 0f)
                    player.velocity.X -= knockbackPower;
                player.velocity.X -= knockbackPower;
                TF2.Minimum(ref player.velocity.X, -knockbackPower);
            }
            else if (direction > 0 && player.velocity.X < knockbackPower)
            {
                if (player.velocity.X < 0f)
                    player.velocity.X += knockbackPower;
                player.velocity.X += knockbackPower;
                TF2.Minimum(ref player.velocity.X, knockbackPower);
            }
            knockbackPower *= -0.5f;
            if (player.velocity.Y > knockbackPower)
            {
                player.velocity.Y += knockbackPower;
                TF2.Minimum(ref player.velocity.Y, knockbackPower);
            }
        }

        protected static void KnockbackNPC(NPC npc, ref NPC.HitModifiers modifiers, float knockbackPower)
        {
            if (modifiers.HitDirection < 0 && npc.velocity.X > -knockbackPower)
            {
                if (npc.velocity.X > 0f)
                    npc.velocity.X -= knockbackPower;
                npc.velocity.X -= knockbackPower;
                TF2.Minimum(ref npc.velocity.X, -knockbackPower);
            }
            else if (modifiers.HitDirection > 0 && npc.velocity.X < knockbackPower)
            {
                if (npc.velocity.X < 0f)
                    npc.velocity.X += knockbackPower;
                npc.velocity.X += knockbackPower;
                TF2.Minimum(ref npc.velocity.X, knockbackPower);
            }
            knockbackPower = npc.noGravity ? (knockbackPower * -0.5f) : (knockbackPower * -0.75f);
            if (npc.velocity.Y > knockbackPower)
            {
                npc.velocity.Y += knockbackPower;
                TF2.Minimum(ref npc.velocity.Y, knockbackPower);
            }
        }

        protected static void KnockbackNPC(NPC npc, int direction, float knockbackPower)
        {
            if (direction < 0 && npc.velocity.X > -knockbackPower)
            {
                if (npc.velocity.X > 0f)
                    npc.velocity.X -= knockbackPower;
                npc.velocity.X -= knockbackPower;
                TF2.Minimum(ref npc.velocity.X, -knockbackPower);
            }
            else if (direction > 0 && npc.velocity.X < knockbackPower)
            {
                if (npc.velocity.X < 0f)
                    npc.velocity.X += knockbackPower;
                npc.velocity.X += knockbackPower;
                TF2.Minimum(ref npc.velocity.X, knockbackPower);
            }
            knockbackPower = npc.noGravity ? (knockbackPower * -0.5f) : (knockbackPower * -0.75f);
            if (npc.velocity.Y > knockbackPower)
            {
                npc.velocity.Y += knockbackPower;
                TF2.Minimum(ref npc.velocity.Y, knockbackPower);
            }
        }

        public sealed override void SetDefaults()
        {
            ProjectileStatistics();
            Projectile.DamageType = ModContent.GetInstance<MercenaryDamage>();
        }

        public override bool PreDraw(ref Color lightColor) => ProjectileDraw(Projectile, ref lightColor);

        public sealed override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC npc)
            {
                spawnedFromNPC = true;
                npcOwner = npc.whoAmI;
                npc.netUpdate = true;
            }
            ProjectileInitialize(source);
        }

        public sealed override bool PreAI() => ProjectilePreAI();

        public sealed override void AI()
        {
            Timer++;
            ProjectileAI();
            if (crit || miniCrit)
                Projectile.penetrate = -1;
            if (homing)
            {
                float projectileDistance = (float)Math.Sqrt(Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y);
                float ai = Timer;
                if (ai == 0f)
                {
                    distance = projectileDistance;
                    ai = projectileDistance;
                }
                if (Projectile.alpha > 0)
                    Projectile.alpha -= 25;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
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
                byte nextAI = 0;
                if (target == 0f)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.npc[i].CanBeChasedBy(this) && (target == 0f || target == i + 1))
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
                                nextAI = (byte)i;
                            }
                        }
                    }
                    if (canSeek)
                        target = (byte)(nextAI + 1);
                    canSeek = false;
                }
                if (target > 0f)
                {
                    int previousAI = (int)(target - 1f);
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
                        target = 0;
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
        }

        public sealed override bool OnTileCollide(Vector2 oldVelocity) => ProjectileTileCollide(oldVelocity);

        public override bool CanHitPlayer(Player target)
        {
            if (Main.npc[npcOwner].ModNPC is BLUMercenary)
                return target.GetModPlayer<TF2Player>().projectileImmunity[Projectile.whoAmI] <= 0;
            else return base.CanHitPlayer(target);
        }

        public sealed override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (modifiers.PvP)
                ProjectileHitPlayer(target, ref modifiers);
            else if (Main.npc[npcOwner].ModNPC is BLUMercenary npc)
            {
                if (Main.expertMode)
                    modifiers.FinalDamage /= 4;
                TF2.NPCDistanceModifier(npc.NPC, Projectile, target, ref modifiers, npc.MaxDamageMultiplier, npc.DamageFalloffRange, npc.NoDamageModifier);
            }
        }

        public sealed override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DamageVariationScale *= 0;
            if (Main.npc[npcOwner].ModNPC is MercenaryBuddy buddy)
                TF2.NPCDistanceModifier(buddy.NPC, Projectile, target, ref modifiers, buddy.MaxDamageMultiplier, buddy.DamageFalloffRange, buddy.NoDamageModifier);
            else if (Main.npc[npcOwner].ModNPC is BLUMercenary npc)
            {
                if (Main.expertMode)
                    modifiers.FinalDamage /= 4;
                TF2.NPCDistanceModifier(npc.NPC, Projectile, target, ref modifiers, npc.MaxDamageMultiplier, npc.DamageFalloffRange, npc.NoDamageModifier);
            }
            if (reserveShooterProjectile && target.noGravity)
                miniCrit = true;
            ProjectileHitNPC(target, ref modifiers);
            Projectile.netUpdate = true;
        }

        public sealed override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            homing = false;
            if (bazaarBargainProjectile && weapon is BazaarBargain)
            {
                ref int heads = ref Main.player[Projectile.owner].GetModPlayer<BazaarBargainPlayer>().heads;
                if (heads < 6)
                    heads++;
            }
            ProjectilePostHitPlayer(target, info);
            TF2Player p = target.GetModPlayer<TF2Player>();
            if (Main.npc[npcOwner].ModNPC is BLUMercenary)
            {
                p.removeImmunityFrames = true;
                p.projectileImmunity[Projectile.whoAmI] = TF2.Time(0.5);
            }
        }

        public sealed override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            homing = false;
            if (bazaarBargainProjectile && weapon is BazaarBargain && target.type != NPCID.TargetDummy)
            {
                ref int heads = ref Main.player[Projectile.owner].GetModPlayer<BazaarBargainPlayer>().heads;
                if (heads < 6)
                    heads++;
            }
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
            writer.Write(npcOwner);
            writer.WriteVector2(velocity);
            writer.Write(crit);
            writer.Write(miniCrit);
            writer.Write(sniperCrit);
            writer.Write(sniperMiniCrit);
            writer.Write(backStab);
            writer.Write(reserveShooterProjectile);
            writer.Write(bazaarBargainProjectile);
            writer.Write(lEtrangerProjectile);
            writer.Write(ammoShot);
            writer.Write(healthShot);
            writer.Write(distance);
            writer.Write(target);
            ProjectileSendExtraAI(writer);
        }

        public sealed override void ReceiveExtraAI(BinaryReader binaryReader)
        {
            Timer = binaryReader.ReadInt32();
            projectileInitialized = binaryReader.ReadBoolean();
            homing = binaryReader.ReadBoolean();
            spawnedFromNPC = binaryReader.ReadBoolean();
            owner = binaryReader.ReadInt32();
            npcOwner = binaryReader.ReadInt32();
            velocity = binaryReader.ReadVector2();
            crit = binaryReader.ReadBoolean();
            miniCrit = binaryReader.ReadBoolean();
            sniperCrit = binaryReader.ReadBoolean();
            sniperMiniCrit = binaryReader.ReadBoolean();
            backStab = binaryReader.ReadBoolean();
            reserveShooterProjectile = binaryReader.ReadBoolean();
            bazaarBargainProjectile = binaryReader.ReadBoolean();
            lEtrangerProjectile = binaryReader.ReadBoolean();
            ammoShot = binaryReader.ReadBoolean();
            healthShot = binaryReader.ReadBoolean();
            distance = binaryReader.ReadSingle();
            target = binaryReader.ReadByte();
            ProjectileReceiveExtraAI(binaryReader);
        }

        public static bool FindOwner(Projectile projectile, float maxDetectDistance)
        {
            bool playerFound = false;
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;
            foreach (Player player in Main.ActivePlayers)
            {
                float sqrDistanceToTarget = Vector2.DistanceSquared(player.Center, projectile.Center);
                if (sqrDistanceToTarget < sqrMaxDetectDistance && player == Main.player[projectile.owner])
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    playerFound = true;
                }
            }
            return playerFound;
        }

        public static Player GetOwner(Projectile projectile, float maxDetectDistance)
        {
            Player playerFound = null;
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;
            foreach (Player player in Main.ActivePlayers)
            {
                float sqrDistanceToTarget = Vector2.DistanceSquared(player.Center, projectile.Center);
                if (sqrDistanceToTarget < sqrMaxDetectDistance && player == Main.player[projectile.owner])
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    playerFound = player;
                }
            }
            return playerFound;
        }

        public static bool NearestNPCFound(Projectile projectile, float maxDetectDistance)
        {
            bool npcFound = false;
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                float sqrDistanceToTarget = Vector2.DistanceSquared(npc.Center, projectile.Center);
                if (sqrDistanceToTarget < sqrMaxDetectDistance && !npc.friendly)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    npcFound = true;
                }
            }
            return npcFound;
        }

        public static NPC GetNearestNPC(Projectile projectile, float maxDetectDistance)
        {
            NPC npcFound = null;
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                float sqrDistanceToTarget = Vector2.DistanceSquared(npc.Center, projectile.Center);
                if (sqrDistanceToTarget < sqrMaxDetectDistance && !npc.friendly)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    npcFound = npc;
                }
            }
            return npcFound;
        }
    }
}