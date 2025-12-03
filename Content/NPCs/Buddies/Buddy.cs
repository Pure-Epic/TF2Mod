using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Modules;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Medic;
using static TF2.Content.Tiles.TF2Tile;

namespace TF2.Content.NPCs.Buddies
{
    // Rhoenicx created the movement and tile detection code, while I created the attacking and reloading code
    public abstract class Buddy : ModNPC
    {
        internal int Timer
        {
            get => (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        protected byte State
        {
            get => (byte)NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        public int Owner
        {
            get => (int)NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        protected int AttackTimer
        {
            get => (int)NPC.localAI[0];
            set => NPC.localAI[0] = value;
        }

        protected int Ammo
        {
            get => (int)NPC.localAI[1];
            set => NPC.localAI[1] = value;
        }

        protected int ReloadTimer
        {
            get => (int)NPC.localAI[2];
            set => NPC.localAI[2] = value;
        }

        protected int ReloadCooldownTimer
        {
            get => (int)NPC.localAI[3];
            set => NPC.localAI[3] = value;
        }

        protected bool CanCrit => NPC.HasBuff<KritzkriegUberCharge>();

        protected bool CanMiniCrit => NPC.HasBuff<BuffBannerBuff>();

        public Player Player
        {
            get
            {
                Player player = Main.player[Owner];
                if (!player.active || player.dead)
                    TF2.KillNPC(NPC);
                return Main.player[Owner];
            }
        }

        protected bool Standing => NPC.velocity.X == 0f;

        protected bool Moving => NPC.velocity.X != 0f;

        protected bool Falling => !onSolidGround && NPC.velocity.Y > 0.1f && !focus;

        protected bool Reloading => State == StateReload;

        protected virtual Asset<Texture2D> Spritesheet => null;

        protected virtual Asset<Texture2D> SpritesheetReverse => null;

        protected virtual Vector2 TrueWidthAndHeight => new Vector2(32, 44);

        public virtual int BaseHealth => 100;

        public virtual float BaseSpeed => 1f;

        protected virtual float JumpHeightMuliplier => 1f;

        protected virtual int ExtraJumps => 0;

        protected virtual int Weapon => ItemID.None;

        protected virtual Vector2 WeaponOffset => new Vector2(0f, 0f);

        protected virtual int AttackSpeed => TF2.Time(1);

        protected virtual int ClipSize => 1;

        protected virtual int InitialReloadSpeed => TF2.Time(0);

        protected virtual int ReloadSpeed => TF2.Time(0);

        protected virtual string ReloadSound => "";

        protected virtual float Range => 1000f;

        public virtual float MaxDamageMultiplier => 1.5f;

        public virtual float DamageFalloffRange => 1000f;

        public virtual bool NoDamageModifier => false;

        protected virtual bool UsesAmmo => true;

        protected virtual bool MagazineReload => false;

        public bool HealPenalty => healPenalty > 0;

        protected float walkSpeed = 1f;
        protected float moveAcceleration = 0.05f;
        protected float moveDeceleration = 0.10f;
        protected float moveFriction = 0.98f;
        protected float jumpHeight = 6 * 16f;
        protected float jumpSpeed = 3f;
        protected short jumpCooldown = 15;
        internal int finalBaseHealth;
        internal bool temporaryBuddy;
        internal float healthMultiplier;
        internal int overheal;
        protected int overhealDecayTimer;
        private int healPenalty;
        public float speedMultiplier = 1f;
        public bool focus;
        protected bool onSolidGround;
        protected bool fallingThroughPlatforms;
        protected bool steppedUp;
        protected bool steppedDown;
        protected bool atDoor;
        protected HashSet<Point> doorsOpened = new HashSet<Point>();
        protected HashSet<Point> doorOpenedCache = new HashSet<Point>();
        protected bool jumping;
        protected bool jumped;
        protected int jumpsLeft;
        private float oldJumpHeight;
        private byte jumpType;
        protected byte horizontalFrame;
        protected byte verticalFrame;
        protected float itemRotation;
        protected int weaponAnimation;
        protected bool forceWeaponDraw;
        protected const int StateIdle = 0;
        protected const int StateWalk = 1;
        protected const int StateFollow = 2;
        protected const int StateAttack = 3;
        protected const int StateReload = 4;

        protected virtual void BuddyStatistics()
        { }

        protected virtual void BuddySpawn()
        { }

        protected virtual void BuddyFrame()
        {
            NPC npc = NPC;
            if (Falling)
            {
                NPC.frameCounter = 0;
                horizontalFrame = 1;
                verticalFrame = 0;
                return;
            }
            else if ((State == StateIdle || State == StateReload || (npc.position == npc.oldPosition && State != StateAttack)) && weaponAnimation <= 0 && !forceWeaponDraw)
            {
                NPC.frameCounter = 0;
                horizontalFrame = 0;
                verticalFrame = 0;
                return;
            }
            else if (weaponAnimation > 0)
            {
                NPC.frameCounter = 0;
                horizontalFrame = 2;
                verticalFrame = 0;
                return;
            }
            if (State == StateWalk || State == StateFollow)
            {
                npc.frameCounter++;
                if (npc.frameCounter >= 2)
                {
                    NPC.frameCounter = 0;
                    horizontalFrame++;
                    if (horizontalFrame >= 14)
                        horizontalFrame = 0;
                }
                verticalFrame = 1;
            }
        }

        protected virtual void BuddyAttack(Player target)
        { }

        protected virtual void BuddyAttack(NPC target)
        { }

        protected virtual void BuddyDie()
        { }

        protected virtual void BuddyMovement()
        {
            Timer++;
            AdjustMoveSpeed(ref NPC.velocity, NPC.direction, walkSpeed * BaseSpeed * speedMultiplier, moveAcceleration, moveDeceleration, moveFriction, onSolidGround);
            if (OnLedge(NPC.position, NPC.direction, NPC.width, NPC.height))
                NPC.velocity.X = 0f;
            if (Timer == TF2.Time(2))
            {
                Timer = 0;
                NPC.velocity.X = 0f;
                State = StateIdle;
                NPC.netUpdate = true;
            }
        }

        protected virtual void BuddyFollow()
        {
            Timer = 0;
            int direction = Player.position.X >= NPC.position.X ? 1 : -1;
            AdjustMoveSpeed(ref NPC.velocity, direction, walkSpeed * BaseSpeed * speedMultiplier, moveAcceleration, moveDeceleration, moveFriction, onSolidGround);
            NPC.direction = direction;
            if (Math.Abs(Player.position.X - NPC.position.X) <= 50f || (NPC.position.Y - Player.position.Y >= 250f))
            {
                NPC.velocity.X = 0f;
                State = StateIdle;
                NPC.netUpdate = true;
            }
        }

        protected virtual void BuddyUpdateAmmo()
        {
            if (Reloading)
            {
                ReloadCooldownTimer++;
                ReloadTimer++;
                if (ReloadCooldownTimer == (InitialReloadSpeed - ReloadSpeed) && !MagazineReload && ReloadSound != "")
                    SoundEngine.PlaySound(new SoundStyle(ReloadSound), NPC.Center);
                if (ReloadCooldownTimer >= InitialReloadSpeed)
                {
                    if (MagazineReload && ReloadTimer == 0 && ReloadSound != "")
                        SoundEngine.PlaySound(new SoundStyle(ReloadSound), NPC.Center);
                    if (ReloadTimer >= ReloadSpeed)
                    {
                        if (MagazineReload)
                            Ammo = ClipSize;
                        else
                            Ammo++;
                        if (ReloadSound != "" && !MagazineReload)
                            SoundEngine.PlaySound(new SoundStyle(ReloadSound), NPC.Center);
                        ReloadTimer = 0;
                    }
                }
            }
            else
                ReloadTimer = 0;
            if (Ammo == ClipSize)
            {
                AttackTimer = AttackSpeed;
                ReloadCooldownTimer = 0;
                ReloadTimer = 0;
                State = StateIdle;
                NPC.netUpdate = true;
            }
        }

        protected virtual void BuddyUpdate()
        { }

        protected virtual void BuddyUpdateWithTarget(NPC target)
        { }

        protected virtual void BuddyDamage(ref NPC.HitModifiers modifiers)
        { }

        protected virtual void BuddySendExtraAI(BinaryWriter writer)
        { }

        protected virtual void BuddyReceiveExtraAI(BinaryReader binaryReader)
        { }

        protected virtual bool EnableBasicMovement() => !focus;

        protected void SetBuddyStatistics(int damage, string hurtSound, string deathSound)
        {
            NPC.damage = damage;
            NPC.HitSound = new SoundStyle(hurtSound);
            NPC.DeathSound = new SoundStyle(deathSound);
        }

        protected void BuddyShoot(IEntitySource spawnSource, Vector2 position, Vector2 velocity, int type, int damage, float knockBack, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            TF2Projectile projectile = TF2.NPCCreateProjectile(spawnSource, position, velocity, type, damage, knockBack, owner, ai0, ai1, ai2, CanCrit, CanMiniCrit);
            if (this is SniperNPC)
                projectile.crit = true;
        }

        public override bool CheckActive() => false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            });
        }

        public sealed override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 42;
            NPC.aiStyle = -1;
            NPC.defense = 0;
            NPC.lifeMax = BaseHealth;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            BuddyStatistics();
        }

        public override void FindFrame(int frameHeight) => BuddyFrame();

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox) => boundingBox = NPC.Hitbox;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D sprite = NPC.direction == -1 ? Spritesheet.Value : SpritesheetReverse.Value;
            int width = sprite.Width / 14;
            int height = sprite.Height / 2;
            float frameWidth = width - TrueWidthAndHeight.X;
            float frameHeight = height - TrueWidthAndHeight.Y;
            spriteBatch.Draw(sprite, NPC.position - screenPos, new Rectangle?(new Rectangle(horizontalFrame * width, verticalFrame * height, width, height)), drawColor, 0f, new Vector2(frameWidth, frameHeight), NPC.scale, (SpriteEffects)(NPC.direction == 1 ? 1 : 0), 0f);
            if ((weaponAnimation > 0) || forceWeaponDraw)
            {
                float rotation = NPC.direction == -1 ? (itemRotation - MathHelper.Pi) : itemRotation;
                bool firingUp = itemRotation >= (-MathHelper.PiOver4 * 3) && itemRotation <= -MathHelper.PiOver4;
                if (firingUp)
                    spriteBatch.Draw(sprite, NPC.position - screenPos, new Rectangle?(new Rectangle(3 * width, 0, width, height)), drawColor, 0f, new Vector2(frameWidth, frameHeight), NPC.scale, (SpriteEffects)(NPC.direction == 1 ? 1 : 0), 0f);
                float scale = 1f;
                Vector2 offset = new Vector2(2f, 20f);
                Main.GetItemDrawFrame(Weapon, out var itemTexture, out var weaponHitbox);
                Vector2 weaponCenter = new Vector2(itemTexture.Width / 2, itemTexture.Height / 2);
                ItemLoader.HoldoutOffset(1f, Weapon, ref offset);
                Vector2 position = offset;
                int offsetX = (int)position.X;
                weaponCenter.Y = position.Y;
                Vector2 origin = new Vector2(-(float)offsetX, itemTexture.Height / 2);
                if (NPC.direction == -1)
                    origin = new Vector2(itemTexture.Width + offsetX, itemTexture.Height / 2);
                Vector2 weaponOffset = NPC.position + new Vector2(NPC.width * 0.5f - weaponHitbox.Width * 0.5f - NPC.direction * 2, 0f);
                Main.spriteBatch.Draw(itemTexture, new Vector2(weaponOffset.X + weaponCenter.X - Main.screenPosition.X, weaponOffset.Y + weaponCenter.Y - Main.screenPosition.Y), (Rectangle?)weaponHitbox, drawColor, rotation, origin, NPC.scale * scale, (SpriteEffects)(NPC.direction == -1 ? 1 : 0), 0f);
                int arm = 5;
                if (itemRotation >= MathHelper.PiOver4 && itemRotation <= (MathHelper.PiOver4 * 3))
                    arm = 6;
                else if (firingUp)
                    arm = 4;
                spriteBatch.Draw(sprite, NPC.position - screenPos, new Rectangle?(new Rectangle(arm * width, 0, width, height)), drawColor, 0f, new Vector2(frameWidth, frameHeight), NPC.scale, (SpriteEffects)(NPC.direction == 1 ? 1 : 0), 0f);
            }
            return false;
        }

        public sealed override void OnSpawn(IEntitySource source)
        {
            healthMultiplier = Player.GetModPlayer<TF2Player>().healthMultiplier;
            finalBaseHealth = NPC.lifeMax = TF2.HealthRound(BaseHealth * healthMultiplier);
            NPC.life = NPC.lifeMax;
            NPC.direction = -1;
            horizontalFrame = 0;
            verticalFrame = 0;
            AttackTimer = AttackSpeed - 1;
            if (NPC.ModNPC is HeavyNPC)
                AttackTimer++;
            Ammo = ClipSize;
            BuddySpawn();
        }

        public sealed override void AI()
        {
            NPC.spriteDirection = NPC.direction;
            steppedUp = false;
            steppedDown = false;
            atDoor = false;
            jumped = false;
            fallingThroughPlatforms = false;
            onSolidGround = IsOnSolidGround(NPC.Bottom, NPC.velocity, NPC.width);
            NPC.oldPosition = NPC.position;
            focus = Player.GetModPlayer<MannsAntiDanmakuSystemPlayer>().mannsAntiDanmakuSystemActive && !temporaryBuddy;
            NPC.noGravity = focus;
            NPC.lifeMax = finalBaseHealth + overheal;
            TF2.Maximum(ref NPC.life, NPC.lifeMax);
            if (NPC.life < NPC.lifeMax && overheal > 0)
            {
                overheal -= NPC.lifeMax - NPC.life;
                if (overheal < 0)
                    overheal = 0;
            }
            if (jumping && onSolidGround && Math.Abs(NPC.velocity.Y) <= 0.1f)
            {
                jumping = false;
                jumpCooldown = (short)TF2.Time(1);
                jumpsLeft = ExtraJumps;
                oldJumpHeight = 0f;
                jumpType = 0;
            }
            if (jumpCooldown > 0)
                jumpCooldown--;
            if (focus)
            {
                NPC.velocity = Vector2.Zero;
                SetHoverPosition();
                if (State == StateWalk || State == StateFollow)
                {
                    State = StateIdle;
                    NPC.netUpdate = true;
                }
                if ((State == StateIdle || State == StateReload) && weaponAnimation <= 0 && !forceWeaponDraw)
                    NPC.direction = Player.direction;
            }
            if (FindTargetNPC(Range, out NPC target) && target != null && State != StateReload && (onSolidGround || focus))
                State = StateAttack;
            switch (State)
            {
                case StateIdle:
                    if (forceWeaponDraw || focus) break;
                    Timer++;
                    if (Timer >= TF2.Time(2))
                    {
                        State = StateWalk;
                        int direction = Main.rand.NextBool() ? 1 : -1;
                        if (OnLedge(NPC.position, direction, NPC.width, NPC.height))
                            direction *= -1;
                        NPC.direction = direction;
                        Timer = 0;
                        NPC.netUpdate = true;
                    }
                    break;
                case StateWalk:
                    if (forceWeaponDraw || focus) break;
                    BuddyMovement();
                    break;
                case StateFollow:
                    if (forceWeaponDraw || focus) break;
                    BuddyFollow();
                    break;
                case StateAttack:
                    if (target == null || !target.active)
                    {
                        State = StateIdle;
                        NPC.netUpdate = true;
                        break;
                    }
                    BuddyAttack(target);
                    break;
                case StateReload:
                    BuddyUpdateAmmo();
                    break;
            }
            if (weaponAnimation > 0)
                weaponAnimation--;
            if (UsesAmmo && Ammo <= 0 && weaponAnimation <= 0)
            {
                AttackTimer = 0;
                State = StateReload;
                NPC.netUpdate = true;
            }
            if (EnableBasicMovement())
            {
                if (NPC.position.Distance(Player.position) >= 250f && (NPC.position.Y - Player.position.Y <= 250f) && State != StateAttack && State != StateReload)
                {
                    State = StateFollow;
                    NPC.netUpdate = true;
                }
                if (State == StateWalk || State == StateFollow)
                {
                    if (CanStepUp() && StepUp(ref NPC.position, ref NPC.velocity, NPC.direction, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, ref fallingThroughPlatforms))
                        steppedUp = true;
                    else if (CanStepDown() && StepDown(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY))
                        steppedDown = true;
                    else if (Moving && HandleDoors())
                        atDoor = true;
                    else if (CanJump() && JumpWall(NPC.position, NPC.direction, NPC.width, NPC.height, jumpHeight * JumpHeightMuliplier, out float height, !NPC.wet))
                    {
                        jumping = true;
                        jumped = true;
                        jumpType = 1;
                        NPC.velocity.Y = GetJumpSpeed(jumpHeight * JumpHeightMuliplier, jumpSpeed * BaseSpeed, 0, height - 4f);
                    }
                    else if (CanJump() && Player.Center.Y <= NPC.Bottom.Y && JumpGaps(NPC.position, NPC.direction, NPC.width, NPC.height))
                    {
                        jumping = true;
                        jumped = true;
                        jumpType = 2;
                        NPC.velocity.Y = GetJumpSpeed(jumpHeight * JumpHeightMuliplier, jumpSpeed * BaseSpeed, 1);
                    }
                    else if (CanJump() && Player.Center.Y < NPC.Top.Y && JumpUpSolidTop(NPC.position, NPC.width, NPC.height, jumpHeight * JumpHeightMuliplier, out float height2))
                    {
                        jumping = true;
                        jumped = true;
                        jumpType = 3;
                        NPC.velocity.Y = GetJumpSpeed(jumpHeight * JumpHeightMuliplier, jumpSpeed * BaseSpeed, 0, height2 - 4f);
                    }
                    else if (CanFallThroughSolidTops() && OnSolidTops(NPC.Bottom, NPC.width))
                        fallingThroughPlatforms = true;
                    if (jumpType > 0 && Math.Abs(NPC.velocity.Y) <= 0.1f && jumpsLeft >= 1)
                    {
                        bool jump = jumpType switch
                        {
                            1 => JumpWall(NPC.position, NPC.direction, NPC.width, NPC.height, jumpHeight * JumpHeightMuliplier, out float _, !NPC.wet),
                            2 => Player.Center.Y <= NPC.Bottom.Y && JumpGaps(NPC.position, NPC.direction, NPC.width, NPC.height),
                            3 => Player.Center.Y < NPC.Top.Y && JumpUpSolidTop(NPC.position, NPC.width, NPC.height, jumpHeight * JumpHeightMuliplier, out float _),
                            _ => false
                        };
                        if (jump)
                        {
                            NPC.velocity.Y = oldJumpHeight;
                            jumpsLeft--;
                            oldJumpHeight = 0f;
                            jumpType = 0;
                        }
                    }
                }
                doorOpenedCache.Clear();
                foreach (Point door in doorsOpened)
                    CloseDoor(door.X, door.Y);
                foreach (Point door in doorOpenedCache)
                    doorsOpened.Remove(door);
                if ((State == StateIdle || State == StateWalk || State == StateFollow) && NPC.position.Distance(Player.position) >= 1000f)
                    NPC.Bottom = Player.Bottom;
            }
            BuddyUpdate();
            BuddyUpdateWithTarget(target);
            bool healed = false;
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if ((projectile.ModProjectile is HealingBeam healingBeam) && projectile.Hitbox.Intersects(NPC.Hitbox) && NPC.life <= finalBaseHealth * healingBeam.OverhealLimit)
                    healed = true;
            }
            if (overheal > 0 && !healed)
                overhealDecayTimer++;
            if (overhealDecayTimer > TF2.Time(0.5) && !healed)
            {
                overhealDecayTimer = 0;
                overheal--;
            }
            healPenalty--;
            healPenalty = Math.Clamp(healPenalty, 0, TF2.Time(10));
            speedMultiplier = 1f;
        }

        public sealed override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            healPenalty = TF2.Time(10);
            BuddyDamage(ref modifiers);
        }

        public sealed override void OnKill()
        {
            BuddyDie();
            int slot = GetBuddySlot();
            if (slot != -1)
                Player.GetModPlayer<TF2Player>().buddies[slot] = -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AttackTimer);
            writer.Write(Ammo);
            writer.Write(ReloadTimer);
            writer.Write(ReloadCooldownTimer);
            writer.Write(finalBaseHealth);
            writer.Write(healthMultiplier);
            writer.Write(overheal);
            writer.Write(overhealDecayTimer);
            writer.Write(healPenalty);
            writer.Write(temporaryBuddy);
            writer.Write(speedMultiplier);
            writer.Write(focus);
            writer.Write(onSolidGround);
            writer.Write(fallingThroughPlatforms);
            writer.Write(steppedUp);
            writer.Write(steppedDown);
            writer.Write(atDoor);
            writer.Write(jumping);
            writer.Write(jumped);
            writer.Write(jumpsLeft);
            writer.Write(jumpType);
            writer.Write(horizontalFrame);
            writer.Write(verticalFrame);
            writer.Write(itemRotation);
            writer.Write(weaponAnimation);
            writer.Write(forceWeaponDraw);
            BuddySendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AttackTimer = reader.ReadInt32();
            Ammo = reader.ReadInt32();
            ReloadTimer = reader.ReadInt32();
            ReloadCooldownTimer = reader.ReadInt32();
            finalBaseHealth = reader.ReadInt32();
            healthMultiplier = reader.ReadSingle();
            overheal = reader.ReadInt32();
            overhealDecayTimer = reader.ReadInt32();
            healPenalty = reader.ReadInt32();
            temporaryBuddy = reader.ReadBoolean();
            speedMultiplier = reader.ReadSingle();
            focus = reader.ReadBoolean();
            onSolidGround = reader.ReadBoolean();
            fallingThroughPlatforms = reader.ReadBoolean();
            steppedUp = reader.ReadBoolean();
            steppedDown = reader.ReadBoolean();
            atDoor = reader.ReadBoolean();
            jumping = reader.ReadBoolean();
            jumped = reader.ReadBoolean();
            jumpsLeft = reader.ReadInt32();
            jumpType = reader.ReadByte();
            horizontalFrame = reader.ReadByte();
            verticalFrame = reader.ReadByte();
            itemRotation = reader.ReadSingle();
            weaponAnimation = reader.ReadInt32();
            forceWeaponDraw = reader.ReadBoolean();
            BuddyReceiveExtraAI(reader);
        }

        #region NPC Library
        protected bool CanStepUp() => !Falling && Moving;

        protected bool CanStepDown() => !jumping && !Falling && Moving;

        protected bool CanJump() => onSolidGround && !jumping && !atDoor && Moving && jumpCooldown <= 0 && (NPC.position.X != NPC.oldPosition.X || State == StateFollow);

        protected bool CanFallThroughSolidTops() => Moving && !jumping && !Falling && !jumped && !steppedDown && !steppedUp && !atDoor && !(jumpCooldown > 0 && NPC.position.X == NPC.oldPosition.X) && (NPC.collideX || Player.Center.Y > NPC.Bottom.Y);

        public static bool OnLedge(Vector2 position, int direction, int width, int height)
        {
            int tileX = (int)((position.X + width / 2 + 15 * direction) / 16f);
            int tileY = (int)((position.Y + height - 16f) / 16f);
            bool avoidFalling = true;
            int tilePixels = 0;
            bool foundObstacles = false;
            Point point = new Point();
            for (int i = -1; i <= 4; i++)
            {
                Tile tileSafely = Framing.GetTileSafely(tileX, tileY + i);
                if (tileSafely.LiquidType > LiquidID.Water)
                {
                    tilePixels++;
                    if (tileSafely.LiquidType == LiquidID.Lava)
                    {
                        foundObstacles = true;
                        break;
                    }
                }
                if (tileSafely.HasUnactuatedTile && Main.tileSolid[tileSafely.TileType])
                {
                    if (tilePixels > 0)
                    {
                        point.X = tileX;
                        point.Y = tileY + i;
                    }
                    avoidFalling = false;
                    break;
                }
            }
            avoidFalling |= foundObstacles;
            double fullBlockTile = Math.Ceiling(height / 16f);
            if (tilePixels >= fullBlockTile)
                avoidFalling = true;
            if (!avoidFalling && point.X != 0 && point.Y != 0)
            {
                Vector2 vector = point.ToWorldCoordinates(8f, 0f) + new Vector2(-width / 2, -height);
                avoidFalling = Collision.DrownCollision(vector, width, height, 1f);
            }
            return avoidFalling;
        }

        public static bool IsOnSolidGround(Vector2 bottom, Vector2 velocity, float width, bool fallThrough = false)
        {
            if (Math.Abs(velocity.Y) > 0.5f && Framing.GetTileSafely(bottom + new Vector2(0, 2f)).TopSlope)
                return false;
            for (int i = (int)((bottom.X - width / 2f + 2f) / 16f); i <= (int)((bottom.X + width / 2f - 2f) / 16f); i++)
            {
                Tile tile = Framing.GetTileSafely(i, (int)((bottom.Y + 2f) / 16f));
                if (!fallThrough && IsSolidTile(tile) || fallThrough && IsSolidTile(tile, noSolidTop: true))
                    return true;
            }
            return false;
        }

        public static void AdjustMoveSpeed(ref Vector2 velocity, int direction, float speed, float accel, float decel, float friction, bool? onSolidGround = null, float airMultiplier = 0.5f)
        {
            speed = Math.Max(speed, 0f);
            accel = Math.Max(accel, 0f);
            decel = Math.Max(decel, 0f);
            friction = Math.Clamp(friction, 0f, 1f);
            onSolidGround ??= velocity.Y == 0f;
            if (direction == 0)
                direction = 1;
            if (direction == 1)
            {
                if (velocity.X == speed) return;
                else if (velocity.X > speed)
                {
                    velocity.X -= decel * (onSolidGround == true ? 1f : airMultiplier);
                    if (velocity.X < speed)
                        velocity.X = speed;
                }
                else if (velocity.X < speed)
                {
                    if (velocity.Y == 0f && velocity.X < 0f)
                        velocity.X *= friction;
                    velocity.X += accel * (onSolidGround == true ? 1f : airMultiplier);
                    if (velocity.X > speed)
                        velocity.X = speed;
                }
            }
            else if (direction == -1)
            {
                if (velocity.X == -speed) return;
                else if (velocity.X < -speed)
                {
                    velocity.X += decel * (onSolidGround == true ? 1f : airMultiplier);
                    if (velocity.X > -speed)
                        velocity.X = speed;
                }
                else if (velocity.X > -speed)
                {
                    if (velocity.Y == 0f && velocity.X > 0f)
                        velocity.X *= friction;
                    velocity.X -= accel * (onSolidGround == true ? 1f : airMultiplier);
                    if (velocity.X < -speed)
                        velocity.X = -speed;
                }
            }
        }

        public static bool StepUp(ref Vector2 position, ref Vector2 velocity, int direction, int width, int height, ref float stepSpeed, ref float gfxOffY, ref bool fallThrough, bool midAirPossible = false)
        {
            if (!midAirPossible && velocity.Y < 0f || velocity.X == 0f) return false;
            Vector2 predictedPosition = position + new Vector2(velocity.X, 0f);
            int tileX = (int)((predictedPosition.X + width / 2f + (width / 2f + 1f) * direction) / 16f);
            int tileY = (int)((position.Y + height - 1f) / 16f);
            if (tileX * 16 > predictedPosition.X + width || tileX * 16 + 16 < predictedPosition.X) return false;
            Tile stepTile = Framing.GetTileSafely(tileX, tileY);
            Tile aboveStepTile = Framing.GetTileSafely(tileX, tileY - 1);
            if (!IsSolidTile(stepTile) && !IsSolidTile(aboveStepTile)) return false;
            if (IsSolidTile(aboveStepTile, noSolidTop: true) && (IsFullBlock(aboveStepTile) || aboveStepTile.BottomSlope || direction == 1 && aboveStepTile.RightSlope || direction == -1 && aboveStepTile.LeftSlope || TileID.Sets.IgnoredByNpcStepUp[aboveStepTile.TileType])) return false;
            float heightAdjustment;
            float stepToY = position.Y + height;
            int tileCheckUp = 0;
            if (IsSolidTile(aboveStepTile, noSolidTop: true) && (direction == 1 && aboveStepTile.TopSlope && aboveStepTile.LeftSlope || direction == -1 && aboveStepTile.TopSlope && aboveStepTile.RightSlope))
            {
                stepToY = tileY * 16f;
                tileCheckUp = 2;
            }
            else if (IsSolidTile(aboveStepTile, noSolidTop: true) && aboveStepTile.IsHalfBlock)
            {
                stepToY = tileY * 16f - 8f;
                tileCheckUp = 2;
            }
            else if (IsSolidTile(stepTile) && (direction == 1 && stepTile.TopSlope && stepTile.LeftSlope || direction == -1 && stepTile.TopSlope && stepTile.RightSlope))
            {
                if (Main.tileSolidTop[aboveStepTile.TileType] && aboveStepTile.TopSlope)
                {
                    fallThrough = true;
                    return true;
                }
                else if (Main.tileSolidTop[stepTile.TileType] && !Main.tileSolidTop[Framing.GetTileSafely(tileX, tileY + 1).TileType])
                {
                    int clearTilesNeeded = (int)Math.Ceiling(height / 16f) - 1;
                    for (int i = tileY - 1; i > tileY - clearTilesNeeded; i--)
                    {
                        if (IsSolidTile(Framing.GetTileSafely(tileX, i), noSolidTop: true))
                            return false;
                    }
                    if (IsSolidTile(Framing.GetTileSafely(tileX, tileY - clearTilesNeeded - 1), noSolidTop: true))
                    {
                        fallThrough = true;
                        return true;
                    }
                }
                return false;
            }
            else if (Main.tileSolidTop[stepTile.TileType] || TileID.Sets.IgnoredByNpcStepUp[aboveStepTile.TileType] || !IsSolidTile(stepTile)) return false;
            else if (IsFullBlock(stepTile) || stepTile.BottomSlope || direction == 1 && stepTile.RightSlope || direction == -1 && stepTile.LeftSlope)
            {
                stepToY = tileY * 16f;
                tileCheckUp = 1;
            }
            else if (stepTile.IsHalfBlock)
            {
                stepToY = tileY * 16f + 8f;
                tileCheckUp = 1;
            }
            heightAdjustment = stepToY - (position.Y + height);
            if (heightAdjustment <= -16.1f || heightAdjustment >= 0f || tileCheckUp <= 0) return false;
            tileY -= tileCheckUp;
            int heightInTiles = (int)Math.Ceiling(height / 16f);
            for (int i = tileY; i > tileY - heightInTiles; i--)
            {
                if (IsSolidTile(Framing.GetTileSafely(tileX, i), noSolidTop: true))
                    return false;
            }
            tileY -= heightInTiles - 1;
            int widthInTiles = (int)Math.Ceiling((width + velocity.X * direction) / 16f);
            if (direction == 1)
            {
                for (int i = tileX; i > tileX - widthInTiles; i--)
                    if (IsSolidTile(Framing.GetTileSafely(i, tileY), noSolidTop: true)) return false;
            }
            else if (direction == -1)
            {
                for (int i = tileX; i < tileX + widthInTiles; i++)
                    if (IsSolidTile(Framing.GetTileSafely(i, tileY), noSolidTop: true)) return false;
            }
            else return false;
            gfxOffY -= heightAdjustment;
            position += new Vector2(0f, heightAdjustment);
            velocity.Y = 0f;
            stepSpeed = Math.Abs(heightAdjustment) < 9f ? 1f : 2f;
            return true;
        }

        public static bool StepDown(ref Vector2 position, ref Vector2 velocity, int width, int height, ref float stepSpeed, ref float gfxOffY)
        {
            if (velocity.X == 0f || velocity.Y < 0f) return false;
            Vector2 predictedPosition = position + new Vector2(velocity.X, 0f);
            predictedPosition.Y = (float)Math.Floor((predictedPosition.Y + height) / 16f) * 16f - height;
            int tileXLeft = (int)(predictedPosition.X / 16f);
            int tileXRight = (int)((predictedPosition.X + width) / 16f);
            int tileYBottom = (int)((predictedPosition.Y + height + 4f) / 16f);
            int heightInTiles = height / 16 + (height % 16 != 0 ? 1 : 0);
            float stepToY = (tileYBottom + heightInTiles) * 16f;
            for (int tileX = tileXLeft; tileX <= tileXRight; ++tileX)
            {
                for (int tileY = tileYBottom; tileY <= tileYBottom + 1; ++tileY)
                {
                    if (!WorldGen.InWorld(tileX, tileY, 1)) return false;
                    Tile tile = Framing.GetTileSafely(tileX, tileY);
                    if (tileY < (int)(Main.bottomWorld / 16) - 42 && !IsSolidTile(tile)) continue;
                    float stepToY2 = tileY * 16f;
                    if (tile.IsHalfBlock) stepToY2 += 8f;
                    if (Utils.FloatIntersect(tileX * 16f, tileY * 16f - 17f, 16f, 16f, position.X, position.Y, width, height) && stepToY2 < stepToY)
                        stepToY = stepToY2;
                }
            }
            float heightAdjustment = stepToY - (position.Y + height);
            if (heightAdjustment <= 0f || heightAdjustment >= 16.1f) return false;
            gfxOffY -= heightAdjustment;
            position += new Vector2(0f, heightAdjustment);
            velocity.Y = 0f;
            stepSpeed = Math.Abs(heightAdjustment) < 9f ? 1f : 2f;
            return true;
        }

        public static bool AtClosedDoor(Vector2 position, int direction, int width, int height, out int tileX, out int tileY)
        {
            tileX = (int)((position.X + width / 2f + (width / 2f + 1f) * direction) / 16f);
            tileY = (int)((position.Y + height - 1f) / 16f);
            int heightInTiles = (int)Math.Ceiling(height / 16f);
            Tile doorTile = Framing.GetTileSafely(tileX, tileY);
            if (IsSolidTile(doorTile) && !TileLoader.IsClosedDoor(doorTile) && doorTile.TileType != TileID.TallGateClosed)
                tileY -= 1;
            for (int i = tileY; i > tileY - heightInTiles; i--)
            {
                doorTile = Framing.GetTileSafely(tileX, i);
                if (!doorTile.HasUnactuatedTile || !(TileLoader.IsClosedDoor(doorTile) || doorTile.TileType == TileID.TallGateClosed) || WorldGen.IsLockedDoor(doorTile))
                    return false;
            }
            return true;
        }

        public static bool AtAnyDoor(Vector2 position, int direction, int width, int height)
        {
            int tileX = (int)((position.X + width / 2f + (width / 2f + 1f) * direction) / 16f);
            int tileY = (int)((position.Y + height - 1f) / 16f);
            int heightInTiles = (int)Math.Ceiling(height / 16f);
            Tile tile = Framing.GetTileSafely(tileX, tileY);
            if (IsSolidTile(tile) && !TileLoader.IsClosedDoor(tile) && tile.TileType != TileID.TallGateClosed)
                tileY -= 1;
            for (int i = tileY; i > tileY - heightInTiles; i--)
            {
                tile = Framing.GetTileSafely(tileX, i);
                if (!tile.HasUnactuatedTile || !(IsDoor(tile) || tile.TileType == TileID.TallGateClosed || tile.TileType == TileID.TallGateOpen))
                    return false;
            }
            return true;
        }

        public static bool TryOpenDoor(int tileX, int tileY, int direction, bool breakDoor = false)
        {
            Tile doorTile = Framing.GetTileSafely(tileX, tileY);
            if (breakDoor && (TileLoader.IsClosedDoor(doorTile) || doorTile.TileType == TileID.TallGateClosed))
            {
                WorldGen.KillTile(tileX, tileY);
                if (Main.dedServ)
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, tileX, tileY);
                return true;
            }
            if (TileLoader.IsClosedDoor(doorTile))
            {
                if (WorldGen.OpenDoor(tileX, tileY, direction))
                {
                    if (Main.dedServ)
                        NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, tileX, tileY, direction);
                    return true;
                }
                if (WorldGen.OpenDoor(tileX, tileY, -direction))
                {
                    if (Main.dedServ)
                        NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, tileX, tileY, -direction);
                    return true;
                }
                return false;
            }
            if (doorTile.TileType == TileID.TallGateClosed)
            {
                if (WorldGen.ShiftTallGate(tileX, tileY, false))
                {
                    if (Main.dedServ) NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 4, tileX, tileY);
                    return true;
                }
                return false;
            }
            return false;
        }

        public void CloseDoor(int tileX, int tileY)
        {
            if ((NPC.position.X + NPC.width / 2) / 16f > tileX + 2 || (NPC.position.X + NPC.width / 2) / 16f < tileX - 2)
            {
                Tile doorTile = Framing.GetTileSafely(tileX, tileY);
                if (TileLoader.CloseDoorID(doorTile) >= 0)
                {
                    if (WorldGen.CloseDoor(tileX, tileY))
                    {
                        doorOpenedCache.Add(new Point(tileX, tileY));
                        NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 1, tileX, tileY, NPC.direction);
                    }
                }
                else if (doorTile.TileType == TileID.TallGateClosed)
                {
                    if (WorldGen.ShiftTallGate(tileX, tileY, closing: true))
                    {
                        doorOpenedCache.Add(new Point(tileX, tileY));
                        NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 5, tileX, tileY);
                    }
                }
            }
        }

        public static bool JumpWall(Vector2 position, int direction, int width, int height, float maxJumpHeight, out float jumpHeight, bool onlyJumpIfReachable = false)
        {
            jumpHeight = 0f;
            int tileX = (int)((position.X + width / 2f + (width / 2f + 1f) * direction) / 16f);
            int tileY = (int)((position.Y + height - 1f) / 16f);
            int heightInTiles = (int)Math.Ceiling(height / 16f);
            int widthInTiles = (int)Math.Ceiling(width / 16f);
            int tileAmount = 0;
            int correction = Framing.GetTileSafely(tileX, tileY).IsHalfBlock || direction == 1 && Framing.GetTileSafely(tileX, tileY).TopSlope && Framing.GetTileSafely(tileX, tileY).LeftSlope || direction == -1 && Framing.GetTileSafely(tileX, tileY).TopSlope && Framing.GetTileSafely(tileX, tileY).RightSlope ? 0 : -1;
            for (int y = tileY - 1; y > tileY - heightInTiles + 1 + correction; y--)
            {
                if (IsSolidTile(Framing.GetTileSafely(tileX, y), noSolidTop: true))
                    tileAmount++;
            }
            if (tileAmount <= 0) return false;
            int MaxJumpTiles = (int)Math.Floor(Math.Max(maxJumpHeight, 0f) / 16f);
            bool wallJumpPossible = true;
            int wallJumpTileY = tileY - 1;
            int platformAmount = 0;
            List<int> validPlatforms = new();
            int roofTileY = tileY - heightInTiles;
            for (int y = tileY - 1; y > tileY - MaxJumpTiles; y--)
            {
                wallJumpPossible = true;
                platformAmount = 0;
                for (int x = tileX; direction == 1 ? x >= tileX - widthInTiles : x <= tileX + widthInTiles; x += direction == 1 ? -1 : 1)
                {
                    if (x == tileX)
                    {
                        if (IsSolidTile(Framing.GetTileSafely(x, y), noSolidTop: true))
                        {
                            for (int y2 = y - 1; y2 > y - heightInTiles - 1; y2--)
                            {
                                if (IsSolidTile(Framing.GetTileSafely(x, y2), noSolidTop: true))
                                {
                                    wallJumpPossible = false;
                                    break;
                                }
                            }
                            if (wallJumpPossible)
                                wallJumpTileY = y;
                        }
                        else
                            wallJumpPossible = false;
                    }
                    else
                    {
                        if (Main.tileSolidTop[Framing.GetTileSafely(x, y).TileType] && platformAmount >= 0)
                            platformAmount++;
                        for (int y2 = y; y2 > y - heightInTiles - 1; y2--)
                        {
                            if (IsSolidTile(Framing.GetTileSafely(x, y2), noSolidTop: true))
                            {
                                wallJumpPossible = false;
                                platformAmount = -1;
                                roofTileY = y2;
                            }
                        }
                    }
                }
                if (platformAmount < 0 && !wallJumpPossible && validPlatforms.Count == 0) break;
                if (wallJumpPossible) break;
                if (platformAmount > 0 && !validPlatforms.Contains(y))
                    validPlatforms.Add(y);
            }
            int tileYToJumpTo;
            if (platformAmount < 0 && !wallJumpPossible && validPlatforms.Count == 0)
            {
                if (onlyJumpIfReachable) return false;
                tileYToJumpTo = roofTileY + heightInTiles;
            }
            else if (platformAmount == 0 && !wallJumpPossible && validPlatforms.Count == 0)
            {
                if (onlyJumpIfReachable) return false;
                tileYToJumpTo = tileY - MaxJumpTiles + 1;
            }
            else if (wallJumpPossible)
                tileYToJumpTo = wallJumpTileY;
            else if (validPlatforms.Count != 0)
                tileYToJumpTo = validPlatforms.Last();
            else return false;
            float difference = tileYToJumpTo * 16f - (position.Y + height);
            if (difference >= -16.1f) return false;
            jumpHeight = difference;
            return true;
        }

        public static bool CheckJumpClearance(int tileX, int tileY, int direction, int widthInTiles, int heightInTiles, out int jumpTileY)
        {
            jumpTileY = tileY;
            for (int x = tileX; direction == 1 ? x > tileX - widthInTiles : x < tileX + widthInTiles; x += direction == 1 ? -1 : 1)
            {
                for (int y = tileY - 1; y >= tileY - heightInTiles; y--)
                {
                    if (IsSolidTile(Framing.GetTileSafely(x, y), noSolidTop: true))
                    {
                        jumpTileY = y + heightInTiles + 1;
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool JumpGaps(Vector2 position, int direction, int width, int height)
        {
            int tileX = (int)((position.X + width / 2f + (width / 2f + 1f) * direction) / 16f);
            int tileY = (int)((position.Y + height - 1f) / 16f);
            int widthInTiles = (int)Math.Ceiling(width / 16f);
            const int depth = 3;
            for (int x = tileX; direction == 1 ? x < tileX + widthInTiles : x > tileX - widthInTiles; x += direction == 1 ? 1 : -1)
            {
                for (int y = tileY; y < tileY + depth; y++)
                    if (IsSolidTile(Framing.GetTileSafely(x, y))) return false;
            }
            int heightInTiles = (int)Math.Ceiling(height / 16f);
            return CheckJumpClearance(tileX - (direction == 1 ? 1 : -1), tileY, direction, widthInTiles, heightInTiles, out int _);
        }

        public static bool JumpUpSolidTop(Vector2 position, int width, int height, float maxJumpHeight, out float jumpHeight)
        {
            jumpHeight = 0f;
            int tileY = (int)((position.Y + height - 1f) / 16f);
            int heightInTiles = (int)Math.Ceiling(height / 16f);
            int maxJumpTiles = (int)Math.Floor(Math.Max(maxJumpHeight, 0f) / 16f);
            List<int> validJumpTilesY = new();
            for (int y = tileY - 1; y > tileY - maxJumpTiles; y--)
            {
                for (int x = (int)((position.X + 2f) / 16f); x <= (int)((position.X + width - 2f) / 16f); x++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (IsSolidTile(tile, noSolidTop: true)) return false;
                    if (Main.tileSolidTop[tile.TileType])
                    {
                        if (!validJumpTilesY.Contains(y))
                            validJumpTilesY.Add(y);
                    }
                }
            }
            if (validJumpTilesY.Count == 0) return false;
            bool foundValidJump = false;
            bool foundInvalidJump = false;
            int tileYToJumpTo = -1;
            foreach (int y in validJumpTilesY)
            {
                for (int x = (int)((position.X + 2f) / 16f); x <= (int)((position.X + width - 2f) / 16f); x++)
                {
                    for (int y2 = y - 1; y2 > y - heightInTiles - 1; y2--)
                    {
                        if (IsSolidTile(Framing.GetTileSafely(x, y2), noSolidTop: true))
                        {
                            foundInvalidJump = true;
                            break;
                        }
                    }
                    if (foundInvalidJump) break;
                }
                if (foundInvalidJump) break;
                foundValidJump = true;
                tileYToJumpTo = y;
            }
            if (!foundValidJump) return false;
            float difference = tileYToJumpTo * 16f - (position.Y + height);
            if (difference >= -16.1f) return false;
            jumpHeight = difference;
            return true;
        }

        protected float GetJumpSpeed(float maxJumpHeight, float minJumpSpeed, int jumpType = 0, float jumpHeight = 0f)
        {
            float height = jumpType switch
            {
                0 => CalculateJumpSpeed(jumpHeight, NPC.gravity, maxJumpHeight, minJumpSpeed, NPC.wet),
                1 => CalculateJumpSpeed(-maxJumpHeight, NPC.gravity, maxJumpHeight, minJumpSpeed, NPC.wet),
                _ => 0f,
            };
            oldJumpHeight = height;
            return height;
        }

        protected static float CalculateJumpSpeed(float jumpHeight, float gravity, float maxJumpHeight = 7 * 16f, float baseJumpSpeed = -4f, bool wet = false)
        {
            if (gravity <= 0f || jumpHeight > 0f) return 0f;
            float speed = 0f;
            if (wet)
                jumpHeight = -maxJumpHeight;
            while (jumpHeight <= 0f)
            {
                jumpHeight -= speed;
                speed -= gravity;
            }
            return Math.Min(baseJumpSpeed, speed);
        }

        protected static bool OnSolidTops(Vector2 bottom, int width)
        {
            for (int i = (int)((bottom.X - width / 2f + 2f) / 16f); i <= (int)((bottom.X + width / 2f - 2f) / 16f); i++)
            {
                Tile tile = Framing.GetTileSafely(i, (int)((bottom.Y + 2f) / 16f));
                if (IsSolidTile(tile, noSolidTop: true)) return false;
            }
            return true;
        }

        private bool HandleDoors()
        {
            bool door1 = AtAnyDoor(NPC.position + new Vector2(16 * NPC.direction, 0f), NPC.direction, NPC.width, NPC.height);
            bool door2 = AtClosedDoor(NPC.position, NPC.direction, NPC.width, NPC.height, out NPC.doorX, out NPC.doorY);
            if (!door1 && !door2)
                return false;
            if (door1)
                return true;
            if (!door2)
                return false;
            if (Main.netMode != NetmodeID.MultiplayerClient && TryOpenDoor(NPC.doorX, NPC.doorY, NPC.direction))
                doorsOpened.Add(new Point(NPC.doorX, NPC.doorY));
            return true;
        }

        public static void SetBuddySpeed(Buddy buddy, double percentage)
        {
            ref float speed = ref buddy.speedMultiplier;
            speed *= (float)(percentage / 100f);
        }

        protected bool FindTargetPlayer(float range, out Player target)
        {
            target = null;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            foreach (Player targetPlayer in Main.ActivePlayers)
            {
                if (!targetPlayer.invis && !Player.dead)
                {
                    float distance = Vector2.Distance(targetPlayer.Center, NPC.Center);
                    bool closest = Vector2.Distance(NPC.Center, targetCenter) > distance;
                    bool inRange = distance <= range;
                    bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetPlayer.position, targetPlayer.width, targetPlayer.height);
                    bool closeThroughWall = distance < (range / 10f);
                    if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                    {
                        range = distance;
                        targetCenter = targetPlayer.Center;
                        target = targetPlayer;
                        foundTarget = true;
                    }
                }
            }
            return foundTarget;
        }

        protected bool FindTargetNPC(float range, out NPC target)
        {
            target = null;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            float trueRange = range;
            foreach (NPC targetNPC in Main.ActiveNPCs)
            {
                if (targetNPC.CanBeChasedBy())
                {
                    float distance = Vector2.Distance(targetNPC.Center, NPC.Center);
                    bool closest = Vector2.Distance(NPC.Center, targetCenter) > distance;
                    bool inRange = distance <= range;
                    bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNPC.position, targetNPC.width, targetNPC.height);
                    bool closeThroughWall = distance < (range / 10f);
                    if ((trueRange >= distance) && (closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                    {
                        range = distance;
                        targetCenter = targetNPC.Center;
                        target = targetNPC;
                        foundTarget = true;
                    }
                }
            }
            return foundTarget;
        }

        protected int GetBuddySlot()
        {
            int[] buddies = Player.GetModPlayer<TF2Player>().buddies;
            for (int i = 0; i < buddies.Length; i++)
            {
                if (buddies[i] == NPC.whoAmI)
                    return i;
            }
            return -1;
        }

        protected void SetHoverPosition()
        {
            Vector2 offset = Vector2.UnitY * -100f;
            float degree = GetBuddySlot() switch
            {
                0 => 0f,
                1 => 120f,
                2 => 240f,
                _ => 360f
            };
            NPC.Center = Player.Center + offset.RotatedBy(MathHelper.ToRadians(degree));
        }

        public void Heal(int amount)
        {
            if (NPC.life > finalBaseHealth) return;
            int healingAmount = (NPC.life + amount > finalBaseHealth) ? (finalBaseHealth - NPC.life) : amount;
            if (healingAmount > 0)
            {
                NPC.life += healingAmount;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.HealEffect(healingAmount);
                NPC.netUpdate = true;
            }
        }
        #endregion
    }
}