using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Projectiles;
using static TF2.Content.NPCs.Buddies.Buddy;
using static TF2.Content.Tiles.TF2Tile;

namespace TF2.Content.NPCs.Enemies
{
    public abstract class BLUMercenary : ModNPC
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

        protected short StuckTime
        {
            get => (short)NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        protected short RetreatCountdown
        {
            get => (short)NPC.ai[3];
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

        protected bool CanCrit => false;

        protected bool CanMiniCrit => false;

        public Player EnemyPlayer => Main.player[NPC.target];

        protected bool Standing => NPC.velocity.X == 0f;

        protected bool Moving => NPC.velocity.X != 0f;

        protected bool Falling => !onSolidGround && NPC.velocity.Y > 0.1f;

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

        protected virtual double Damage => 0;

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

        public override string Texture => $"TF2/{Spritesheet.Name}";

        protected float walkSpeed = 1f;
        protected float moveAcceleration = 0.05f;
        protected float moveDeceleration = 0.10f;
        protected float moveFriction = 0.98f;
        protected float jumpHeight = 6 * 16f;
        protected float jumpSpeed = 3f;
        protected short jumpCooldown = 15;
        internal int finalBaseHealth;
        protected byte maxTurnTime = (byte)TF2.Time(2);
        protected Vector2 targetCenter;
        private Vector2 oldJumpPosition;
        protected short turnTime;
        public float speedMultiplier = 1f;
        protected bool stunned;
        protected bool onSolidGround;
        protected bool fallingThroughPlatforms;
        protected bool steppedUp;
        protected bool steppedDown;
        protected bool atDoor;
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
        protected const int StateMove = 1;
        protected const int StateAttack = 2;
        protected const int StateReload = 3;

        protected virtual void EnemyStatistics()
        { }

        protected virtual void EnemySpawn()
        { }

        protected virtual void EnemyFrame()
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
            if (State == StateMove)
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

        protected virtual void EnemyAttack(Player target)
        { }

        protected virtual void EnemyAttack(NPC target)
        { }

        protected virtual void EnemyDie()
        { }

        protected virtual void EnemyMovement()
        {
            if (RetreatCountdown <= 0)
                UpdateMoveDirections();
            AdjustMoveSpeed(ref NPC.velocity, NPC.direction, walkSpeed * BaseSpeed * speedMultiplier, moveAcceleration * speedMultiplier, moveDeceleration * speedMultiplier, moveFriction * speedMultiplier, onSolidGround);
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
            else if (CanJump() && EnemyPlayer.Center.Y <= NPC.Bottom.Y && JumpGaps(NPC.position, NPC.direction, NPC.width, NPC.height))
            {
                jumping = true;
                jumped = true;
                jumpType = 2;
                NPC.velocity.Y = GetJumpSpeed(jumpHeight * JumpHeightMuliplier, jumpSpeed * BaseSpeed, 1);
            }
            else if (CanJump() && EnemyPlayer.Center.Y < NPC.Top.Y && JumpUpSolidTop(NPC.position, NPC.width, NPC.height, jumpHeight * JumpHeightMuliplier, out float height2))
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
                    2 => EnemyPlayer.Center.Y <= NPC.Bottom.Y && JumpGaps(NPC.position, NPC.direction, NPC.width, NPC.height),
                    3 => EnemyPlayer.Center.Y < NPC.Top.Y && JumpUpSolidTop(NPC.position, NPC.width, NPC.height, jumpHeight * JumpHeightMuliplier, out float _),
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
            StuckCheck();
        }

        protected virtual void StuckCheck()
        {
            if (NPC.position.X == NPC.oldPosition.X && NPC.velocity.X != 0f && !jumping && !atDoor)
                StuckTime++;
            else if (StuckTime > 0 && !jumping && !atDoor)
                StuckTime--;
            if (jumped)
            {
                if (oldJumpPosition == NPC.position)
                    StuckTime += (short)TF2.Time(0.25);
                else
                    StuckTime = 0;
                oldJumpPosition = NPC.position;
            }
            if (StuckTime < 0 || steppedUp || steppedDown || NPC.direction != NPC.oldDirection || NPC.justHit || NPC.Distance(targetCenter) < 16f)
                StuckTime = 0;
            if (!stunned && !atDoor && StuckTime > TF2.Time(2.5))
            {
                NPC.direction *= -1;
                StuckTime = 0;
                if (RetreatCountdown <= 0)
                    RetreatCountdown = (short)Main.rand.Next(TF2.Time(5), TF2.Time(10));
            }
            if (NPC.direction != NPC.oldDirection && State == StateIdle)
            {
                byte turnTimer = (byte)(turnTime & 0x00FF);
                byte turnCounter = (byte)((turnTime & 0xFF00) >> 8);
                if (turnTimer > 0)
                {
                    turnCounter++;
                }
                turnTimer = maxTurnTime;
                byte turnAmount = 5;
                if (turnCounter > turnAmount)
                {
                    turnTimer = 0;
                    turnCounter = 0;
                    if (RetreatCountdown <= 0)
                        RetreatCountdown = (short)Main.rand.Next(TF2.Time(5), TF2.Time(10));
                    NPC.netUpdate = true;
                }
                turnTime = (short)((turnCounter << 8) + turnTimer);
            }
        }

        protected virtual void EnemyUpdateAmmo()
        {
            if (Reloading)
            {
                if (ReloadCooldownTimer == 0 && onSolidGround)
                    NPC.velocity.X = 0f;
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
            NPC.netUpdate = true;
            if (Ammo == ClipSize)
            {
                AttackTimer = AttackSpeed;
                ReloadCooldownTimer = 0;
                ReloadTimer = 0;
                State = StateIdle;
                NPC.netUpdate = true;
            }
        }

        protected virtual void EnemyUpdate()
        { }

        protected virtual void EnemyUpdateWithTarget(Player target)
        { }

        protected virtual void EnemySendExtraAI(BinaryWriter writer)
        { }

        protected virtual void EnemyReceiveExtraAI(BinaryReader binaryReader)
        { }

        protected void SetEnemyStatistics(string hurtSound, string deathSound)
        {
            NPC.HitSound = new SoundStyle(hurtSound);
            NPC.DeathSound = new SoundStyle(deathSound);
        }

        protected void EnemyShoot(IEntitySource spawnSource, Vector2 position, Vector2 velocity, int type, int damage, float knockBack, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            TF2Projectile projectile = TF2.NPCCreateProjectile(spawnSource, position, velocity, type, damage, knockBack, owner, ai0, ai1, ai2, CanCrit, CanMiniCrit);
            if (this is EnemySniperNPC)
                projectile.crit = true;
        }

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
            NPC.netAlways = true;
            EnemyStatistics();
        }

        public override void FindFrame(int frameHeight) => EnemyFrame();

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
                spriteBatch.Draw(itemTexture, new Vector2(weaponOffset.X + weaponCenter.X - Main.screenPosition.X, weaponOffset.Y + weaponCenter.Y - Main.screenPosition.Y), (Rectangle?)weaponHitbox, Color.White, rotation, origin, NPC.scale * scale, (SpriteEffects)(NPC.direction == -1 ? 1 : 0), 0f);
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
            finalBaseHealth = NPC.lifeMax = TF2.HealthRound(BaseHealth * TF2.GlobalHealthMultiplier);
            NPC.life = NPC.lifeMax;
            NPC.direction = -1;
            horizontalFrame = 0;
            verticalFrame = 0;
            AttackTimer = AttackSpeed - 1;
            if (NPC.ModNPC is EnemyHeavyNPC)
                AttackTimer++;
            Ammo = ClipSize;
            EnemySpawn();
            NPC.netUpdate = true;
        }

        public sealed override void AI()
        {
            NPC.lifeMax = finalBaseHealth;
            NPC.TargetClosest(false);
            bool withinRange = WithinAttackRange(Range);
            targetCenter = EnemyPlayer.Center;
            NPC.spriteDirection = NPC.direction;
            steppedUp = false;
            steppedDown = false;
            atDoor = false;
            jumped = false;
            fallingThroughPlatforms = false;
            onSolidGround = IsOnSolidGround(NPC.Bottom, NPC.velocity, NPC.width);
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
            if (RetreatCountdown > 0)
            {
                RetreatCountdown--;
                if (RetreatCountdown <= 0)
                    RetreatCountdown = 0;
            }
            if ((byte)(turnTime & 0x00FF) > 0)
            {
                turnTime--;
                if ((byte)(turnTime & 0x00FF) == 0)
                    turnTime = 0;
            }
            if (withinRange && State != StateReload && onSolidGround)
            {
                State = StateAttack;
                NPC.netUpdate = true;
            }
            switch (State)
            {
                case StateIdle:
                    if (forceWeaponDraw) break;
                    State = StateMove;
                    NPC.netUpdate = true;
                    break;
                case StateMove:
                    if (forceWeaponDraw) break;
                    EnemyMovement();
                    break;
                case StateAttack:
                    if (onSolidGround)
                        NPC.velocity.X = 0f;
                    if (!EnemyPlayer.active || EnemyPlayer.dead || !withinRange)
                    {
                        State = StateIdle;
                        NPC.netUpdate = true;
                        break;
                    }
                    EnemyAttack(EnemyPlayer);
                    break;
                case StateReload:
                    EnemyUpdateAmmo();
                    break;
            }
            NPC.oldPosition = NPC.position;
            if (weaponAnimation > 0)
                weaponAnimation--;
            if (UsesAmmo && Ammo <= 0 && weaponAnimation <= 0)
            {
                AttackTimer = 0;
                State = StateReload;
                NPC.netUpdate = true;
            }
            if (!withinRange && State != StateAttack && State != StateReload)
            {
                State = StateMove;
                NPC.netUpdate = true;
            }
            EnemyUpdate();
            EnemyUpdateWithTarget(EnemyPlayer);
            speedMultiplier = 1f;
        }

        public sealed override void OnKill() => EnemyDie();

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AttackTimer);
            writer.Write(Ammo);
            writer.Write(ReloadTimer);
            writer.Write(ReloadCooldownTimer);
            writer.Write(finalBaseHealth);
            writer.Write(speedMultiplier);
            writer.Write(targetCenter.X);
            writer.Write(targetCenter.Y);
            writer.Write(oldJumpPosition.X);
            writer.Write(oldJumpPosition.Y);
            writer.Write(turnTime);
            writer.Write(stunned);
            writer.Write(onSolidGround);
            writer.Write(fallingThroughPlatforms);
            writer.Write(steppedUp);
            writer.Write(steppedDown);
            writer.Write(atDoor);
            writer.Write(jumping);
            writer.Write(jumped);
            writer.Write(jumpsLeft);
            writer.Write(jumpType);
            writer.Write(oldJumpHeight);
            writer.Write(horizontalFrame);
            writer.Write(verticalFrame);
            writer.Write(itemRotation);
            writer.Write(weaponAnimation);
            writer.Write(forceWeaponDraw);
            EnemySendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AttackTimer = reader.ReadInt32();
            Ammo = reader.ReadInt32();
            ReloadTimer = reader.ReadInt32();
            ReloadCooldownTimer = reader.ReadInt32();
            finalBaseHealth = reader.ReadInt32();
            speedMultiplier = reader.ReadSingle();
            targetCenter = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            oldJumpPosition = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            turnTime = reader.ReadInt16();
            stunned = reader.ReadBoolean();
            onSolidGround = reader.ReadBoolean();
            fallingThroughPlatforms = reader.ReadBoolean();
            steppedUp = reader.ReadBoolean();
            steppedDown = reader.ReadBoolean();
            atDoor = reader.ReadBoolean();
            jumping = reader.ReadBoolean();
            jumped = reader.ReadBoolean();
            jumpsLeft = reader.ReadInt32();
            jumpType = reader.ReadByte();
            oldJumpHeight = reader.ReadSingle();
            horizontalFrame = reader.ReadByte();
            verticalFrame = reader.ReadByte();
            itemRotation = reader.ReadSingle();
            weaponAnimation = reader.ReadInt32();
            forceWeaponDraw = reader.ReadBoolean();
            EnemyReceiveExtraAI(reader);
        }

        #region NPC Library

        protected void UpdateMoveDirections()
        {
            NPC.direction = targetCenter.X >= NPC.Center.X ? 1 : -1;
            NPC.directionY = targetCenter.Y >= NPC.Center.Y ? 1 : -1;
        }

        protected bool CanStepUp() => !Falling && Moving;

        protected bool CanStepDown() => !jumping && !Falling && Moving;

        protected bool CanJump() => onSolidGround && !jumping && !atDoor && Moving && jumpCooldown <= 0;

        protected bool CanFallThroughSolidTops() => Moving && !jumping && !Falling && !jumped && !steppedDown && !steppedUp && !atDoor && !(jumpCooldown > 0 && NPC.position.X == NPC.oldPosition.X) && (NPC.collideX || EnemyPlayer.Center.Y > NPC.Bottom.Y);

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
            if (Main.netMode != NetmodeID.MultiplayerClient)
                TryOpenDoor(NPC.doorX, NPC.doorY, NPC.direction);
            return true;
        }

        public static void SetEnemySpeed(BLUMercenary enemy, double percentage)
        {
            ref float speed = ref enemy.speedMultiplier;
            speed *= (float)(percentage / 100f);
        }

        protected bool WithinAttackRange(float range)
        {
            Player player = EnemyPlayer;
            float distance = Vector2.Distance(player.Center, NPC.Center);
            bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
            bool closeThroughWall = distance < (range / 10f);
            return player.active && !player.dead && distance <= range && (lineOfSight || closeThroughWall);
        }

        protected int GetDamage() => TF2.Round(Damage * TF2.GlobalDamageMultiplier);

        protected Player GetTargetPlayer(float range)
        {
            Player target = null;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            foreach (Player targetPlayer in Main.ActivePlayers)
            {
                if (!targetPlayer.invis && !targetPlayer.dead)
                {
                    float distance = Vector2.Distance(targetPlayer.Center, NPC.Center);
                    bool closest = Vector2.Distance(NPC.Center, targetCenter) > distance;
                    bool inRange = distance < range;
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
            return target;
        }

        public void Heal(int amount)
        {
            if (NPC.life > NPC.lifeMax) return;
            int healingAmount = (NPC.life + amount > NPC.lifeMax) ? (NPC.lifeMax - NPC.life) : amount;
            NPC.lifeMax += healingAmount;
            NPC.HealEffect(healingAmount);
            NPC.netUpdate = true;
        }
        #endregion
    }
}