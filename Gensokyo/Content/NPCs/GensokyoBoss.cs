using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TF2.Gensokyo.Content.NPCs
{
    // This class is the rewritten version of the GensokyoBoss class by Rhoenicx.
    // This class is identical to the one in the Gensokyo mod with a few additions.
    // Don't worry, I have exclusive permission to use and share this code.
    [ExtendsFromMod("Gensokyo")]
    public abstract class GensokyoBoss : ModNPC
    {
        #region States
        //**************************************************************************************************************
        //********** States ********************************************************************************************
        //**************************************************************************************************************

        private const int State_Decision = 0;
        private const int State_MoveWindup = 1;
        private const int State_MovePrepare = 2;
        protected const int State_Move = 3;
        protected const int State_AttackPrepare = 4;
        protected const int State_SlowdownPrepare = 5;
        protected const int State_Slowdown = 6;
        protected const int State_AttackWindup = 7;
        public const int State_Attack = 8;
        protected const int State_Winddown = 9;
        private const int State_End = 10;
        protected const int State_Despawn = 99;
        #endregion

        #region PhaseIDs

        //**************************************************************************************************************
        //********** Phases ********************************************************************************************
        //**************************************************************************************************************

        protected const int Phase_Empty = -1;
        protected const int Phase_SpawnMove = 0;
        protected const int Phase_NewStageSlowdown = 1;
        protected const int Phase_DefaultStageMove = 2;
        protected const int Phase_DefaultStageAttack = 3;

        #endregion PhaseIDs

        #region MovementTypes

        //**************************************************************************************************************
        //********** Movement ******************************************************************************************
        //**************************************************************************************************************

        protected const int MoveType_None = -1;
        protected const int MoveType_Straight = 0;
        protected const int MoveType_Follow = 1;
        protected const int MoveType_Teleport = 2;
        protected const int MoveType_Orbit = 3;
        protected const int MoveType_Fixed = 4;
        protected const int MoveType_BossSpecific = 5;

        #endregion MovementTypes

        #region AI_Fields
        //**************************************************************************************************************
        //********** Synchronized Values *******************************************************************************
        //**************************************************************************************************************
        protected int State
        {
            get => (int)NPC.localAI[0];
            set => NPC.localAI[0] = value;
        }

        protected int Stage
        {
            get => (int)NPC.localAI[1];
            private set => NPC.localAI[1] = value;
        }

        protected int Phase
        {
            get => (int)NPC.localAI[2];
            set => NPC.localAI[2] = value;
        }

        protected int Timer
        {
            get => (int)NPC.localAI[3];
            set => NPC.localAI[3] = value;
        }

        protected float TargetSpeed;
        #endregion

        #region Variables
        //**************************************************************************************************************
        //********** Variables *****************************************************************************************
        //**************************************************************************************************************

        // ----- Phase Queue -----
        protected List<int> PhaseQueue = new();

        // ----- Target(s) ----- 
        protected Vector2 TargetCenter =>
            !IgnoreDecoys && Main.player[NPC.target].tankPet >= 0
                ? Main.projectile[Main.player[NPC.target].tankPet].Center
                : Main.player[NPC.target].Center;

        protected Vector2 TargetBottom =>
        !IgnoreDecoys && Main.player[NPC.target].tankPet >= 0
            ? Main.projectile[Main.player[NPC.target].tankPet].Bottom
            : Main.player[NPC.target].Bottom;

        protected Vector2 OldTargetBottom = Vector2.Zero;

        protected Vector2 TargetDirection;
        protected readonly IList<int> Targets = new List<int>();

        protected Vector2 MoveDirection;

        // ----- Boss settings -----
        public int NumStages;
        protected int Tier;
        protected bool IgnoreDecoys;

        protected int[] MovePhaseDuration;
        protected int[] AttackPhaseDuration;
        protected float MovespeedMax;

        // ----- Decision variables -----
        protected bool JustSpawned = true;
        protected bool NewStage = true;
        protected int MoveRetryCounter = 0;
        protected int PunishStrikeCounter = 0;

        // Destination of movements
        protected Vector2 MoveDestination = Vector2.Zero;
        protected Vector2 MoveOffset = Vector2.Zero;
        protected Vector2 MovePrediction = Vector2.Zero;

        // Spellcard announcement
        private bool _spellCardAnnounced;

        // Direction of slowdown state,
        // 1=Accelerate or -1=Decelerate
        private int _slowdownDirection = 0;

        // Additional Movement variables
        protected int ElapsedMoveTime = 0;
        protected int TotalMoveTime = 0;
        protected int SlowdownTime = 0;
        protected float SlowdownDistance = 0f;

        protected bool enableRevengeanceDamageResistance;
        protected float damageScale = 1f;

        #endregion Variables

        #region Music

        protected static int LoadMusic(string musicName, int musicID, int minVersion = 2) // TODO Remove once no longer needed
        {
            ModLoader.TryGetMod("GensokyoMusic", out Mod gensokyoMusic);
            if (gensokyoMusic == null)
            {
                return musicID;
            }

            if (gensokyoMusic.Version.Minor < minVersion)
            {
                return musicID;
            }

            int bossMusic = MusicLoader.GetMusicSlot(gensokyoMusic, "Sounds/Music/" + musicName);
            return bossMusic == 0 ? musicID : bossMusic;
        }

        #endregion Music

        #region Helpers

        protected bool GetTarget()
        {
            // Target a player; if none available, return false
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    return false;
                }
            }
            return true;
        }

        public static Vector2 TargetPlayerOrDecoyPosition(int playerID, bool ignoreDecoys = false)
        {
            Player target = Main.player[playerID];
            return target.tankPet >= 0 && !ignoreDecoys
                ? Main.projectile[target.tankPet].Center
                : target.Center;
        }

        public static Vector2 TargetPlayerOrDecoyVelocity(int playerID, bool ignoreDecoys = false)
        {
            Player target = Main.player[playerID];
            return target.tankPet >= 0 && !ignoreDecoys
                ? Main.projectile[target.tankPet].velocity
                : target.velocity;
        }

        protected Vector2 TargetPlayerOrDecoyPosition(int playerID) => TargetPlayerOrDecoyPosition(playerID, IgnoreDecoys);

        protected Vector2 TargetPlayerOrDecoyVelocity(int playerID) => TargetPlayerOrDecoyVelocity(playerID, IgnoreDecoys);

        protected Vector2 GetTargetDirection() => NPC.DirectionTo(TargetCenter);

        protected float GetTargetDistance() => NPC.Distance(TargetCenter);

        private void StageCheck()
        {
            int currentStage = Stage;

            float healthPerStage = NPC.lifeMax / (float)NumStages;
            int healthLost = NPC.lifeMax - NPC.life;

            Stage = (int)(healthLost / healthPerStage);

            if (currentStage != Stage)
            {
                NewStage = true;
                if (currentStage < Stage)
                    _spellCardAnnounced = false;
            }
        }

        #endregion Helpers

        #region Overrides
        //**************************************************************************************************************
        //********** Set or overriden by boss NPCs *********************************************************************
        //**************************************************************************************************************

        #region Despawn
        /// <summary>
        /// Determines the direction the boss will move when it should despawn.
        /// Returns (0f, -12f), i.e. moving upwards, by default
        /// </summary>
        protected virtual Vector2 DespawnMoveDirection() => new Vector2(0f, -12f);

        /// <summary>
        /// The range between boss and target when despawning is
        /// possible.
        /// </summary>
        protected virtual float DespawnRange() => 2880f;

        /// <summary>
        /// Used to despawn the boss if a certain requirement
        /// has been triggered. Return true to despawn the boss. 
        /// Returns false by default
        /// </summary>
        protected virtual bool AdditionalDespawnCondition() => false;
        #endregion

        #region Decision
        /// <summary>
        /// Can be used to implement restrictions on whether the current target is considered valid.
        /// Returns true by default.
        /// </summary>
        protected virtual bool TargetIsValid_BossSpecific() => true;

        /// <summary>
        /// This is the 'brain' of the boss, here the phases (attacks/moves)
        /// are chosen. Add the phases to the PhaseQueue List. Here you 
        /// can create a complex moveset for the boss depending on 
        /// conditions. By default the stage move and attack phases
        /// will be assigned to make this AI compatible with existing
        /// Gensokyo mod bosses.
        /// </summary>
        protected virtual void Decision()
        {
            // When the boss is spawned in for the first time,
            // execute the spawn movement.
            if (JustSpawned)
            {
                PhaseQueue.Add(Phase_SpawnMove);
                PhaseQueue.Add(Phase_DefaultStageAttack);
            }

            // When the boss' current stage has been changed
            // Clear the queue and assign the new stage phases
            if (NewStage && !JustSpawned)
            {
                // Clear everyting from the current stage
                MoveRetryCounter = 0;
                PhaseQueue.Clear();
            }

            // Insert the phases into the queue whenever
            // it has executed everything.
            if (PhaseQueue.Count == 0)
            {
                PhaseQueue.Add(Phase_DefaultStageMove);
                PhaseQueue.Add(Phase_DefaultStageAttack);
            }

            // If the boss still has velocity from the previous
            // stage, slow the boss down.
            if (NewStage && NPC.velocity.Length() > 0f)
            {
                PhaseQueue.Insert(0, Phase_NewStageSlowdown);
            }
        }

        protected virtual bool CanChangeStage() => true;
        #endregion

        #region MoveWindup
        /// <summary>
        /// Indicates if this movement has an MoveWindup,
        /// Used for setting up things before the move, like 
        /// making sure animations have been played.
        /// Also requires you to use MoveWindupCompleted()
        /// Returns false by default.
        /// </summary>
        protected virtual bool HasMoveWindup() => false;

        /// <summary>
        /// Used for executing the MoveWindUp, once the windup
        /// is completed return true.
        /// Returns true by default.
        /// </summary>
        protected virtual bool MoveWindUpComplete() => true;
        #endregion

        #region Move
        /// <summary>
        /// Determines if the current phase contains a Move,
        /// if it has no movement, MovePrepare and Move states
        /// are not executed.
        /// </summary>
        protected virtual bool HasMove() => MoveType() != MoveType_None;

        /// <summary>
        /// Determines which Movement Type this phase should use
        /// Returns MoveType_Straight by default.
        /// </summary>
        protected virtual int MoveType() => Phase switch
        {
            Phase_SpawnMove or Phase_DefaultStageMove => MoveType_Follow,
            _ => MoveType_None,
        };

        /// <summary>
        /// Determines if the MoveDestination needs to be updated
        /// outside MovePrepare. Used for Follow and Orbit movetype
        /// </summary>
        /// <returns></returns>
        protected virtual bool MoveUpdateDestination() => (State == State_Move && MoveType() is MoveType_Follow or MoveType_Orbit)
                || (State == State_Slowdown && SlowdownType() is MoveType_Follow or MoveType_Orbit);

        /// <summary>
        /// Get the move destination. By default this returns the
        /// TargetCenter. Can be used to set a fixed MoveDestination.
        /// </summary>
        protected virtual Vector2 GetMoveDestination() => TargetCenter;

        /// <summary>
        /// Determines if the current movement uses an additional
        /// offset from the Destination. Returns true if the MoveType
        /// is not MoveType_Orbit.
        /// </summary>
        /// <returns></returns>
        protected virtual bool MoveUseOffset() => MoveType() != MoveType_Orbit;

        /// <summary>
        /// Get an offset around the destination position chosen in 
        /// GetMoveDestination(). The 'random' offset is calculated 
        /// based on the decimal part of the boss' position. This 
        /// is sudo-random and also in sync between all parties.
        /// Only gets called once during MovePrepare, after that
        /// the offset is stored in the MoveOffset field.
        /// </summary>
        protected virtual Vector2 GetMoveOffset()
        {
            Vector2 offset = new Vector2(0, 192)
                + new Vector2(0, GetRandomOffsetDistance(120f, 200f))
                .RotatedBy(MathHelper.ToRadians(GetRandomOffsetAngle(-60f, 60f)));

            return TargetCenter.Y >= 2000 ? -offset : offset;
        }

        /// <summary>
        /// Determines if the movement should predict the target 
        /// position and direction based on the targets velocity.
        /// Returns true if the MoveType is set to MoveType_Follow.
        /// </summary>
        protected virtual bool MoveUsePrediction() => MoveType() == MoveType_Follow;

        /// <summary>
        /// Determines if the movement can be ended early.
        /// For example if the boss is in range of the player
        /// and there is still time left on the Timer,
        /// you might want to slow the boss down earlier and
        /// execute attack. Returns false by default.
        /// </summary>
        /// <returns></returns>
        protected virtual bool MoveCanEndEarly() => false;

        /// <summary>
        /// Place your end-early conditions
        /// here. Used in combination with MoveCanEndEarly(),
        /// when true is returned in this hook the movement
        /// will be cancelled. Returns false by default.
        /// </summary>
        protected virtual bool MoveEndEarlyComplete() => false;

        /// <summary>
        /// The Minimum move time of this phase.
        /// Returns MovePhaseDuration[Stage] by default.
        /// </summary>
        protected virtual int MoveMinimumTime() => MovePhaseDuration[Stage];

        /// <summary>
        /// The Maximum move time of this phase.
        /// Returns MovePhaseDuration[Stage] by default if the 
        /// current phase is not Phase_SpawnMove.
        /// </summary>
        protected virtual int MoveMaximumTime()
        {
            if (Phase == Phase_SpawnMove
                || MoveCalculationAddDecelerationTime())
            {
                return int.MaxValue;
            }

            return MovePhaseDuration[Stage];
        }


        /// <summary>
        /// The Maximum move speed of this phase.
        /// Returns 10f by default;
        /// </summary>
        protected virtual float MoveMaximumSpeed() => MovespeedMax;

        /// <summary>
        /// The Acceleration rate used for this phase.
        /// Returns 1.075f by default.
        /// </summary>
        protected virtual float MoveAccelerationRate() => 1.075f;

        /// <summary>
        /// The Deceleration rate used for this phase.
        /// Returns 0.93f by default.
        /// </summary>
        protected virtual float MoveDecelerationRate() => 0.93f;

        /// <summary>
        /// The turnrate used for this phase, if the
        /// MovementType() is set to follow or orbit.
        /// Returns 0.05f by default.
        /// </summary>
        protected virtual float MoveTurnRate() => 0.05f;

        /// <summary>
        /// Determines if CalculateMoveTime() needs hit 
        /// the MoveMinimumTime, in some special cases
        /// this time cannot be reached without a longer
        /// move distance. This setting is used to prefer
        /// hitting the MoveMinimumTime over the Distance.
        /// Returns true by default.
        /// </summary>
        protected virtual bool MoveEnforceMinimumTime() => true;

        /// <summary>
        /// Determines if the deceleration time will be
        /// added to the Timer when the MoveTime calculation
        /// has been completed. By default only when MoveType()
        /// is set to MoveType_Orbit.
        /// </summary>
        protected virtual bool MoveCalculationAddDecelerationTime() => MoveType() == MoveType_Orbit;

        /// <summary>
        /// Executes slowdown during State_Move. Used for phases
        /// where you also need to have the time to slowdown in the 
        /// MidMoveAttack.
        /// </summary>
        protected virtual bool MoveSlowdownDuringMove() => MoveCalculationAddDecelerationTime() ||
                MoveType() == MoveType_Follow;

        /// <summary>
        /// Determines if the time needed to slowdown will be taken
        /// into consideration for the MoveTime calculation.
        /// By default returns the value set in HasSlowdown().
        /// </summary>
        protected virtual bool MoveCalculationHasSlowdown() => HasSlowdown();

        /// <summary>
        /// Determines if a Straight movement should be re-directed
        /// towards the target, useful for when a dash attack is 
        /// executed after a follow movement. Returns true by default.
        /// </summary>
        protected virtual bool MoveStraightShouldRedirect() => true;

        /// <summary>
        /// The position in degrees around the MoveDestination the boss
        /// should stop in the Orbit movetype. This position can get flipped
        /// depending on the current position of the boss.
        /// Boss left => end position right and viceversa
        /// Returns 0f by default (Above target)
        /// </summary>
        protected virtual float MoveOrbitTargetRotation() => 90f;

        /// <summary>
        /// The offset of the boss from the target during the Orbit
        /// movetype. This distance also gets used in the
        /// circumference calculations.
        /// Returns 0f by default.
        /// </summary>
        protected virtual float MoveOrbitDistance() => 400f;

        /// <summary>
        /// The direction the boss will move around the player
        /// 1=clockwise, -1=counter-clockwise. Gets flipped 
        /// depending on the current position of the boss.
        /// Returns 1 clockwise by default (boss moves to top)
        /// </summary>
        protected virtual int MoveOrbitDirection() => 1;

        /// <summary>
        /// Amount of additional revolutions around the destination
        /// before the orbit stops. Can be used to have the boss 
        /// rotate around the destination longer.
        /// Returns 0 (no extra revolutions) by default;
        /// </summary>
        protected virtual int MoveOrbitRevolutions() => 0;

        /// <summary>
        /// Determines if this move phase should check for the
        /// distance between boss and target. If enabled use
        /// MoveRangeCheckDistance() to se the distance.
        /// Returns true by default.
        /// </summary>
        protected virtual bool HasMoveRangeCheck() => true;


        /// <summary>
        /// The distance between boss and target when it is 
        /// considered out of range. Returns 1750f by default.
        /// </summary>
        protected virtual float MoveRangeCheckDistance() => 1750f;

        /// <summary>
        /// The maximum amount of retries of the movement when
        /// the target is not in range. Returns -1 by default;
        /// negative values equals infinite retries!
        /// </summary>
        protected virtual int MoveRetryAmount() => -1;

        /// <summary>
        /// Used to manually run MovePrepare for BossSpecific
        /// movements, runs when MoveType() is set to MoveType_BossSpecific
        /// Everything need to be handled manually!
        /// </summary>
        protected virtual void MovePrepare_BossSpecific()
        {

        }

        /// <summary>
        /// Used to manually run Move for BossSpecific movements,
        /// runs when MoveType() is set to MoveType_BossSpecific
        /// Everything need to be handled manually!
        /// </summary>
        protected virtual void Move_BossSpecific()
        {

        }
        #endregion

        #region MidMoveAttack
        /// <summary>
        /// Used to execute the MidMoveAttack contained in
        /// this phase. Keep in mind that the Timer counts
        /// down!
        /// </summary>
        protected virtual void MidMoveAttack()
        {

        }
        #endregion

        #region Attackprepare
        /// <summary>
        /// Determines if the current phase has an spellcard
        /// announcement message
        /// </summary>
        protected virtual bool HasSpellCardAnnouncement() => Phase == Phase_DefaultStageAttack;
        /// <summary>
        /// Plays the Spellcard message in chat. Place
        /// logic for playing the message in this hook.
        /// </summary>
        protected virtual void AnnounceSpellCard()
        {

        }

        /// <summary>
        /// Determines if this phase uses ExtraAttackPreparations(). Set
        /// to true to use addition preperations. Returns false by default.
        /// </summary>
        protected virtual bool HasExtraAttackPreparations() => false;

        /// <summary>
        /// Hook for additional attack preparations, such as updating a target list
        /// </summary>
        protected virtual void ExtraAttackPreparations()
        {

        }
        #endregion

        #region Slowdown

        /// <summary>
        /// Determines if this phase contains a slowdown.
        /// Can be used in attack-only phases to make sure
        /// the boss has been slowed down. By default returns
        /// true if SlowdownType() has been set to anything
        /// other than MoveType_None
        /// </summary>
        protected virtual bool HasSlowdown() => SlowdownType() != MoveType_None;

        /// <summary>
        /// The slowdown type of this phase.
        /// </summary>
        protected virtual int SlowdownType()
        {
            switch (Phase)
            {
                case Phase_SpawnMove:
                case Phase_DefaultStageMove:
                case Phase_NewStageSlowdown:
                    return MoveType_Straight;

                default:
                    return MoveType_None;
            }
        }

        /// <summary>
        /// The target speed the boss needs to get after 
        /// the slowdown state has been completed.
        /// Returns 0f by default.
        /// </summary>
        protected virtual float SlowdownSpeed() => 0f;

        /// <summary>
        /// Used to manually run Move for BossSpecific movements,
        /// runs when MoveType() is set to MoveType_BossSpecific
        /// Everything need to be handled manually!
        /// </summary>
        protected virtual void Slowdown_BossSpecific()
        {

        }
        #endregion

        #region AttackWindup
        /// <summary>
        /// Indicates that this phase contains an AttackWindup
        /// Returns false by default
        /// </summary>
        protected virtual bool HasAttackWindup() => false;

        /// <summary>
        /// Executes the Attack windup, used for playing animations
        /// just before the attack state for example.
        /// When the windup is completed, return true.
        /// Returns True by default.
        /// </summary>
        protected virtual bool AttackWindupComplete() => true;
        #endregion

        #region Attack
        /// <summary>
        /// Indicates that this phase contains an Attack
        /// Returns false by default
        /// </summary>
        protected virtual bool HasAttack()
        {
            switch (Phase)
            {
                case Phase_DefaultStageAttack:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Executes the Attack contained in this phase
        /// Also override AttackComplete() condition and
        /// AttackPhaseDuration() timer
        /// </summary>
        protected virtual void Attack()
        {

        }

        /// <summary>
        /// Override to use a custom attack completed condition,
        /// by default it uses Timer together with AttackPhaseDuration()
        /// </summary>
        protected virtual bool AttackComplete() => Timer >= GetAttackPhaseDuration();

        /// <summary>
        /// Sets the maximum amount of time the Attack takes.
        /// Returns AttackPhaseDuration[Stage] by default.
        /// </summary>
        protected virtual int GetAttackPhaseDuration() => AttackPhaseDuration[Stage];

        /// <summary>
        /// Used to clean up and reset variables used in the attack.
        /// </summary>
        protected virtual void PostAttack()
        {

        }
        #endregion

        #region Ending
        /// <summary>
        /// Indicates that this phase uses an winddown.
        /// Used for playing animation, putting away a sword etc.
        /// Returns false by default
        /// </summary>
        protected virtual bool HasWinddown() => false;

        /// <summary>
        /// Used for executing the winddown conditions, when 
        /// done return true. Returns true by default
        /// </summary>
        protected virtual bool WinddownComplete() => true;

        /// <summary>
        /// Used to clean up and reset variables for this phase
        /// </summary>
        protected virtual void End()
        {

        }
        #endregion

        #endregion

        #region AI
        //**************************************************************************************************************
        //********** BOSSAI ********************************************************************************************
        //**************************************************************************************************************
        public override void AI()
        {
            // Clientsided check for when the boss is spawned in.
            // Reset NewStage and JustSpawned
            if (JustSpawned && Main.netMode == NetmodeID.MultiplayerClient)
            {
                NewStage = false;
                JustSpawned = false;
            }

            // Makes sure the boss does not automatically despawn,
            // only when it is outside our defined range
            if (NPC.timeLeft < 10)
            {
                if (NPC.Distance(TargetCenter) <= DespawnRange())
                    NPC.timeLeft = NPC.activeTime;
            }

            // Execute additional despawn conditions
            if (AdditionalDespawnCondition())
                Despawn();

            // State machine
            switch (State)
            {
                //-----------------------------------------------------------------------------
                case State_Decision:
                    {
                        // Determine the current Stage of the boss fight
                        StageCheck();

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NewStage = false;
                            return;
                        }

                        NPC.TargetClosest(false);

                        if (!TargetIsValid() || !TargetIsValid_BossSpecific())
                        {
                            Despawn();
                            return;
                        }

                        // Run decision for this boss, here the phases
                        // need to be added to the PhaseQueue
                        Decision();

                        // NewStage and JustSpawned set to false
                        NewStage = false;
                        JustSpawned = false;

                        // Assign the phase in slot 0 of the PhaseQueue List
                        Phase = PhaseQueue.Count > 0 ? PhaseQueue[0] : Phase_Empty;

                        // Reset timers, and set netUpdate
                        Timer = 0;
                        NPC.netUpdate = true;

                        // Phase has no movement, directly go to AttackPrepare
                        if (!HasMove())
                        {
                            State = State_AttackPrepare;
                            return;
                        }

                        // Phase has movement with windup, go to MoveWindup
                        if (HasMoveWindup())
                        {
                            State = State_MoveWindup;
                            return;
                        }

                        // go to MovePrepare
                        State = State_MovePrepare;
                    }
                    return;


                //-----------------------------------------------------------------------------
                case State_MoveWindup:
                    {
                        if (MoveWindUpComplete() && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Timer = 0;
                            State = State_MovePrepare;
                            NPC.netUpdate = true;
                        }
                    }
                    return;


                //-----------------------------------------------------------------------------
                case State_MovePrepare:
                    {
                        // Check the current stage of the boss
                        StageCheck();

                        // Stage has been changed, go to the End state
                        if (NewStage)
                        {
                            if (Main.netMode == NetmodeID.MultiplayerClient) return;

                            State = HasWinddown() ? State_Winddown : State_End;
                            NPC.netUpdate = true;
                            return;
                        }

                        // Movement type bossSpecific
                        if (MoveType() == MoveType_BossSpecific)
                        {
                            MovePrepare_BossSpecific();
                            return;
                        }

                        // Grab the destination of this movement
                        MoveDestination = GetMoveDestination();

                        if (MoveOffset == Vector2.Zero)
                            MoveOffset = GetMoveOffset();

                        if (MoveUseOffset())
                            MoveDestination += MoveOffset;

                        if (MoveUsePrediction())
                        {
                            MovePrediction = Vector2.Lerp(MovePrediction, Main.player[NPC.target].velocity * Timer, 0.01f);
                            MoveDestination += MovePrediction;
                        }

                        // Save the Target bottom position
                        OldTargetBottom = TargetBottom;

                        // Reset variables
                        ElapsedMoveTime = 0;
                        TotalMoveTime = 0;
                        SlowdownTime = 0;
                        SlowdownDistance = 0f;

                        if (Main.netMode == NetmodeID.MultiplayerClient) return;

                        // Calculate move times and speed
                        switch (MoveType())
                        {
                            //------------------------------------
                            case MoveType_Straight:
                            case MoveType_Follow:
                                {
                                    // Calculate the movement MoveTime and TargetSpeed   
                                    CalculateMovement(Vector2.Distance(MoveDestination, NPC.Center));

                                    if (NPC.velocity.Length() < 0.5f || (MoveType() == MoveType_Straight && MoveStraightShouldRedirect()))
                                    {
                                        Vector2 direction = NPC.DirectionTo(MoveDestination);

                                        if (direction.HasNaNs())
                                        {
                                            direction = NPC.DirectionTo(TargetCenter);
                                        }

                                        NPC.velocity = direction * Math.Max(NPC.velocity.Length(), 0.5f);
                                    }
                                }
                                break;

                            //------------------------------------
                            case MoveType_Teleport:
                                {
                                    Timer = 0;
                                    NPC.Center = MoveDestination;
                                }
                                break;

                            //------------------------------------
                            case MoveType_Orbit:
                                {
                                    // Get the direction between destination and boss,
                                    // then rotate 90 degrees to make -Y axis zero.
                                    Vector2 Direction = MoveDestination.DirectionTo(NPC.Center).RotatedBy(MathHelper.PiOver2);
                                    float OrbitCurrentRotation = MathHelper.ToDegrees((float)Math.Atan2(Direction.Y, Direction.X));

                                    // Set the target rotation and orbit direction.
                                    float OrbitTargetRotation = MoveOrbitTargetRotation() * (OrbitCurrentRotation > 0f ? -1 : 1);
                                    int OrbitDirection = MoveOrbitDirection() * (OrbitCurrentRotation > 0f ? -1 : 1);

                                    // Calculate the amount of degrees between current and target,
                                    // while keeping into account clockwise and counter-clockwise preset.
                                    float OrbitRotationAmount = 0f;
                                    if ((OrbitDirection == -1 && OrbitTargetRotation <= OrbitCurrentRotation)
                                        || (OrbitDirection == 1 && OrbitTargetRotation >= OrbitCurrentRotation))
                                    {
                                        if (OrbitTargetRotation < 0f)
                                            OrbitRotationAmount = (float)Math.Abs(OrbitTargetRotation) + (float)Math.Abs(OrbitCurrentRotation);
                                        else
                                            OrbitRotationAmount = (float)Math.Abs(OrbitCurrentRotation - OrbitTargetRotation);
                                    }
                                    else
                                        OrbitRotationAmount = 180f - (float)Math.Abs(OrbitTargetRotation) + (180f - (float)Math.Abs(OrbitCurrentRotation));

                                    // Add the amount of extra revolutions
                                    OrbitRotationAmount += MoveOrbitRevolutions() * 360f;

                                    // Convert to Radians
                                    OrbitRotationAmount = MathHelper.ToRadians(OrbitRotationAmount);

                                    // Calculate the distance of the circular movement,
                                    // and add the remaining distance between target
                                    // and edge of the orbit's cicle
                                    float distance = MoveOrbitDistance() * OrbitRotationAmount;
                                    distance += Math.Abs(Vector2.Distance(MoveDestination, NPC.Center) - MoveOrbitDistance());

                                    // Now calculate the time and targetspeed for this orbit
                                    CalculateMovement(distance);

                                    // To synchronize the clockwise or couner clockwise movement, 
                                    // We multiply the targetspeed by the direction => use Math.Sign().
                                    TargetSpeed *= OrbitDirection;

                                    // 1. Calculate the rotation between destination and boss
                                    Vector2 currentRotation = MoveDestination.DirectionTo(NPC.Center);

                                    // 2. Calculate the circumference of the orbit's circle
                                    float circumference = MoveOrbitDistance() * 2 * (float)Math.PI;

                                    // 3. Get the speed of the boss
                                    float speed = Math.Max(NPC.velocity.Length(), 0.5f);

                                    // 4. Calculate the amount of radians that will be travelled
                                    //    along the circle orbit with the given speed
                                    float addedRotation = speed / circumference * 2 * (float)Math.PI;

                                    // 5. Create the new rotation by rotating the current rotation
                                    Vector2 newRotation = currentRotation.RotatedBy(addedRotation * OrbitDirection);

                                    // 6. Calculate the new position
                                    Vector2 newPosition = newRotation * MoveOrbitDistance();

                                    // 7. Adjust the velocity towards the new position
                                    //    ONLY when the speed is not set yet.
                                    if (NPC.velocity.Length() < 0.5f)
                                        NPC.velocity = NPC.Center.DirectionTo(newPosition) * 0.5f;
                                }
                                break;

                            //------------------------------------
                            case MoveType_Fixed:
                                {
                                    // This movement type keeps the boss on its
                                    // current destination using TargetBottom
                                    Timer = MoveMinimumTime();
                                }
                                break;

                            //------------------------------------
                            default:
                                {
                                    // Terminate the movement
                                    Timer = 0;
                                    State = State_AttackPrepare;
                                    NPC.netUpdate = true;
                                }
                                return;
                        }

                        // Change direction of the boss
                        NPC.direction = NPC.velocity.X >= 0f ? 1 : -1;

                        // Go to the Move State
                        State = State_Move;
                        NPC.netUpdate = true;
                    }
                    return;


                //-----------------------------------------------------------------------------
                case State_Move:
                    {
                        // Boss Specific movement
                        if (MoveType() == MoveType_BossSpecific)
                        {
                            Move_BossSpecific();
                            return;
                        }

                        if (MoveUpdateDestination())
                        {
                            MoveDestination = GetMoveDestination();

                            if (MoveUseOffset())
                                MoveDestination += MoveOffset;

                            if (MoveUsePrediction())
                            {
                                MovePrediction = Vector2.Lerp(MovePrediction, Main.player[NPC.target].velocity * Timer, 0.01f);
                                MoveDestination += MovePrediction;
                            }
                        }

                        // Calculate distance between Destination and NPC
                        float distance = Vector2.Distance(MoveDestination, NPC.Center);

                        // Calculate the distance and time needed to slowdown
                        CalculateSlowdown();

                        // New velocity for this movement
                        Vector2 newVelocity = NPC.velocity;

                        switch (MoveType())
                        {
                            //------------------------------------
                            case MoveType_Straight:
                                {
                                    if (NPC.velocity.Length() > TargetSpeed)
                                        newVelocity *= MoveDecelerationRate();
                                    else if (NPC.velocity.Length() < TargetSpeed)
                                        newVelocity *= MoveAccelerationRate();

                                    if ((NPC.velocity.Length() < TargetSpeed && newVelocity.Length() > TargetSpeed)
                                        || (NPC.velocity.Length() >= TargetSpeed && newVelocity.Length() < TargetSpeed))
                                        newVelocity = Vector2.Normalize(NPC.velocity) * TargetSpeed;
                                }
                                break;

                            //------------------------------------
                            case MoveType_Follow:
                                {
                                    Vector2 newDirection = NPC.Center.DirectionTo(MoveDestination);

                                    // Calculate a new TargetSpeed for the current distance
                                    CalculateMovement(distance, false, Timer);

                                    // If the boss needs to move away from its destination
                                    // accelerate the boss
                                    if (distance > 0.5f && newVelocity.Length() < 0.5f)
                                        newVelocity = newDirection * 0.5f;

                                    // Create a sample for the new direction and speed of the boss
                                    Vector2 sample = Vector2.Lerp(newVelocity, newDirection * TargetSpeed, 0.001f);

                                    // Adjust speed
                                    if (newVelocity.Length() > sample.Length())
                                        newVelocity *= MoveDecelerationRate();
                                    else if (newVelocity.Length() < sample.Length())
                                        newVelocity *= MoveAccelerationRate();

                                    // Adjust rotation
                                    Vector2 currentDirection = Vector2.Normalize(newVelocity);
                                    float turnRate = MoveTurnRate();
                                    if (newVelocity.Length() > 0f && MoveMaximumSpeed() > 0f)
                                        turnRate = MoveMaximumSpeed() / (newVelocity.Length() + 1f) * turnRate;

                                    // Set the new direction
                                    newDirection = Vector2.Normalize(Vector2.Lerp(currentDirection, newDirection, turnRate));

                                    // Additional deceleration when in range
                                    if (distance <= SlowdownDistance)
                                        newVelocity *= MoveDecelerationRate();

                                    // Assign the new capped speed and rotation
                                    newVelocity = newDirection * newVelocity.Length();

                                    // If the boss is really close to its destination,
                                    // set velocity to zero to prevent glitchy movement.
                                    if (distance <= 0.5f)
                                        newVelocity = Vector2.Zero;
                                }
                                break;

                            //------------------------------------
                            case MoveType_Orbit:
                                {
                                    // Calculate the rotation between destination and boss
                                    Vector2 currentRotation = MoveDestination.DirectionTo(NPC.Center);

                                    // Calculate the circumference of the orbit's circle
                                    float circumference = MoveOrbitDistance() * 2 * (float)Math.PI;

                                    // Get the speed of the boss
                                    float speed = Math.Max(newVelocity.Length(), 0.5f);

                                    // Calculate the amount of radians that will be travelled
                                    // along the circle orbit with the given speed
                                    float addedRotation = 0f;

                                    addedRotation = speed / circumference * 2 * (float)Math.PI;

                                    // Create the new rotation by rotating the current rotation
                                    Vector2 newRotation = currentRotation.RotatedBy(addedRotation * Math.Sign(TargetSpeed));

                                    // Calculate the new position
                                    MoveDestination += newRotation * MoveOrbitDistance();

                                    // Direction towards the new position
                                    Vector2 newDirection = NPC.Center.DirectionTo(MoveDestination);

                                    // Create a sample for the new direction and speed of the boss
                                    Vector2 sample = Vector2.Lerp(newVelocity, newDirection * Math.Abs(TargetSpeed), 0.001f);

                                    // Calculate the remaining distance of the orbit
                                    distance = SlowdownDistance;
                                    for (int i = Timer - SlowdownTime; i > 0; i--)
                                        distance += Math.Abs(TargetSpeed);

                                    // Adjust speed
                                    if (newVelocity.Length() < sample.Length())
                                    {
                                        // Only if the distance is greater than the distance
                                        // needed for the slowdown
                                        if (distance > SlowdownDistance)
                                            newVelocity *= MoveAccelerationRate();
                                    }
                                    else
                                        newVelocity *= MoveDecelerationRate();

                                    // Additional deceleration when in range
                                    if (distance <= SlowdownDistance)
                                        newVelocity *= MoveDecelerationRate();

                                    // Assign the new capped speed and rotation
                                    newVelocity = newDirection * newVelocity.Length();

                                    // If the boss is really close to its destination,
                                    // set velocity to zero to prevent glitchy movement.
                                    if (distance <= 0.5f)
                                        newVelocity = Vector2.Zero;

                                    if (Timer <= 0)
                                        newVelocity += TargetBottom - OldTargetBottom;
                                    else
                                    {
                                        // Snap the boss on the player's position
                                        NPC.Center += TargetBottom - OldTargetBottom;
                                        OldTargetBottom = TargetBottom;
                                    }
                                }
                                break;

                            //------------------------------------
                            case MoveType_Fixed:
                                {
                                    newVelocity = TargetBottom - OldTargetBottom;
                                    OldTargetBottom = TargetBottom;
                                }
                                break;

                            //------------------------------------
                            default:
                                {
                                    // Do nothing, Timer will run out
                                }
                                break;
                        }

                        // Set the new velocity
                        if (!newVelocity.HasNaNs())
                            NPC.velocity = newVelocity;

                        // Change direction of the boss
                        if (NPC.velocity != Vector2.Zero)
                            NPC.direction = NPC.velocity.X >= 0f ? 1 : -1;

                        // Set the move direction
                        MoveDirection = Vector2.Normalize(NPC.velocity);

                        // Execute MidMoveAttack
                        MidMoveAttack();

                        // Reduce Timer
                        Timer--;
                        ElapsedMoveTime++;

                        if (Main.netMode == NetmodeID.MultiplayerClient) return;

                        // Continue the movement if the Timer has not run out yet,
                        // and if the move has not been cancelled
                        if (Timer >= 0 && !(MoveCanEndEarly() && MoveEndEarlyComplete()))
                            return;

                        // Reset Timers and set netUpdate
                        Timer = 0;
                        NPC.netUpdate = true;

                        // Check the range between boss and target
                        if (HasMoveRangeCheck() && Vector2.Distance(MoveDestination, NPC.Center) > MoveRangeCheckDistance()
                            && (MoveRetryAmount() < 0 || MoveRetryCounter < MoveRetryAmount()))
                        {
                            MoveRetryCounter++;
                            State = State_Decision;
                            return;
                        }

                        // Too much retries, do a punish/catchup
                        if (MoveRetryCounter >= MoveRetryAmount())
                            PunishStrikeCounter++;

                        // Go to the AttackPrepare state
                        if (HasAttack() || HasSlowdown())
                        {
                            State = State_AttackPrepare;
                            return;
                        }

                        // Otherwise end this phase
                        State = HasWinddown() ? State_Winddown : State_End;
                    }
                    return;


                //-----------------------------------------------------------------------------
                case State_AttackPrepare:
                    {
                        // Check the current stage of the boss
                        StageCheck();

                        // Stage has been changed, go to the End state
                        if (NewStage)
                        {
                            if (Main.netMode == NetmodeID.MultiplayerClient) return;

                            State = HasWinddown() ? State_Winddown : State_End;
                            NPC.netUpdate = true;
                            return;
                        }

                        // Announce the spellcard in chat
                        if (Main.netMode != NetmodeID.Server
                            //&& Gensokyo.GensokyoConfigClient.DisplaySpellcardNames
                            && !_spellCardAnnounced
                            && HasSpellCardAnnouncement())
                        {
                            AnnounceSpellCard();
                            _spellCardAnnounced = true;
                        }

                        // return when this is a client
                        if (Main.netMode == NetmodeID.MultiplayerClient) return;

                        // Hook for additional attack preparations, such as updating a target list
                        if (HasExtraAttackPreparations())
                            ExtraAttackPreparations();

                        // Determine the base target direction for the attacks
                        TargetDirection = GetTargetDirection();

                        // Reset Timer and set netUpdate
                        Timer = 0;
                        NPC.netUpdate = true;

                        // Phase has Slowdown go to slowdown state
                        if (HasSlowdown())
                        {
                            State = State_SlowdownPrepare;
                            return;
                        }

                        // Phase has an attack
                        if (HasAttack())
                        {
                            // Target the closest player and change the npc direction
                            NPC.TargetClosest();

                            State = HasAttackWindup()
                                ? State = State_AttackWindup
                                : State_Attack;
                            return;
                        }

                        // Determine ending state
                        State = HasWinddown()
                            ? State_Winddown
                            : State_End;
                    }
                    return;

                //-----------------------------------------------------------------------------
                case State_SlowdownPrepare:
                case State_Slowdown:
                    {
                        // Boss specific slowdown
                        if (SlowdownType() == MoveType_BossSpecific)
                        {
                            Slowdown_BossSpecific();
                            return;
                        }

                        // Run SlowdownPrepare state only the first tick.
                        if (State == State_SlowdownPrepare)
                        {
                            // Calculate slowdown distance and time
                            CalculateSlowdown();
                            Timer = SlowdownTime;

                            // Determine the direction of the slowdown in the first tick.
                            _slowdownDirection = NPC.velocity.Length() < SlowdownSpeed() ? 1 : -1;

                            // if the boss' current speed is slower than 0.5f and it needs to 
                            // accelerate to a speed higher than 0.5f, give it some velocity
                            if (NPC.velocity.Length() < 0.5f && SlowdownSpeed() >= 0.5f)
                                NPC.velocity = Vector2.Normalize(NPC.velocity) / MoveAccelerationRate() * 0.5f;

                            // Move offset is empty, caused by this phase not having a movement.
                            if (MoveOffset == Vector2.Zero)
                                MoveOffset = GetMoveOffset();

                            // Transition to State_Slowdown, this happens on all parties!
                            State = State_Slowdown;
                        }

                        // Update the destination of the boss
                        if (MoveUpdateDestination())
                        {
                            MoveDestination = GetMoveDestination();

                            if (MoveUseOffset())
                                MoveDestination += MoveOffset;

                            if (MoveUsePrediction())
                            {
                                MovePrediction = Vector2.Lerp(MovePrediction, Main.player[NPC.target].velocity * Timer, 0.01f);
                                MoveDestination += MovePrediction;
                            }
                        }

                        // Create new velocity variable with current velocity.
                        Vector2 newVelocity = NPC.velocity;

                        // Accelerate the boss
                        if (_slowdownDirection == 1)
                            newVelocity *= MoveAccelerationRate();

                        // Decelerate the boss
                        if (_slowdownDirection == -1)
                            newVelocity *= MoveDecelerationRate();

                        // break when movetype is not follow
                        if (SlowdownType() == MoveType_Follow)
                        {
                            // New direction towards the destination point
                            Vector2 newDirection = NPC.Center.DirectionTo(MoveDestination);

                            // Adjust rotation
                            Vector2 currentDirection = Vector2.Normalize(newVelocity);
                            float turnRate = MoveTurnRate();
                            if (newVelocity.Length() > 0f && MoveMaximumSpeed() > 0f)
                            {
                                turnRate = MoveMaximumSpeed() / (newVelocity.Length() + 1f) * turnRate;
                            }

                            // Set the new velocity
                            newVelocity = Vector2.Normalize(Vector2.Lerp(currentDirection, newDirection, turnRate)) * newVelocity.Length();
                        }

                        // Assign the new velocity
                        if (!newVelocity.HasNaNs())
                            NPC.velocity = newVelocity;

                        // Change direction of the boss
                        if (NPC.velocity != Vector2.Zero)
                            NPC.direction = NPC.velocity.X >= 0f ? 1 : -1;

                        // Set the move direction
                        MoveDirection = Vector2.Normalize(NPC.velocity);

                        // Run ModMoveAttack()
                        MidMoveAttack();

                        // Reduce Timer
                        Timer--;
                        ElapsedMoveTime++;

                        // Evaluate if the slowdown has been finished
                        if (Timer > 0 && Vector2.Distance(MoveDestination, NPC.Center) > Math.Max(SlowdownSpeed(), 0.5f))
                        {
                            // If not, return here
                            return;
                        }

                        // If both the current velocity and desired velocity is below 0.5f,
                        // stop the boss instead. otherwise assign slowdownSpeed
                        if (NPC.velocity.Length() <= 0.5f && SlowdownSpeed() <= 0.5f)
                            NPC.velocity = Vector2.Zero;
                        else
                            NPC.velocity = Vector2.Normalize(NPC.velocity) * SlowdownSpeed();

                        if (Main.netMode == NetmodeID.MultiplayerClient) return;

                        // Reset Timer and set netUpdate
                        Timer = 0;
                        NPC.netUpdate = true;

                        // Phase has an attack
                        if (HasAttack())
                        {
                            // Determine the attack state
                            State = HasAttackWindup()
                                ? State_AttackWindup
                                : State_Attack;
                            return;
                        }

                        // Determine the end state
                        State = HasWinddown()
                            ? State_Winddown
                            : State_End;
                    }
                    return;

                //-----------------------------------------------------------------------------
                case State_AttackWindup:
                    {
                        if (AttackWindupComplete() && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Timer = 0;
                            State = State_Attack;
                            NPC.netUpdate = true;
                        }
                    }
                    return;


                //-----------------------------------------------------------------------------
                case State_Attack:
                    {
                        // Execute the attack
                        Attack();

                        // Check if the attack has been completed
                        if (AttackComplete())
                        {
                            PostAttack();

                            if (Main.netMode == NetmodeID.MultiplayerClient) return;

                            // Reset Timers and set netUpdate
                            Timer = 0;
                            NPC.netUpdate = true;

                            // Determine next state
                            State = HasWinddown()
                                ? State_Winddown
                                : State_End;
                            return;
                        }

                        // Increase Timer for use in the attack
                        Timer++;
                    }
                    return;


                //-----------------------------------------------------------------------------
                case State_Winddown:
                    {
                        if (WinddownComplete() && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Timer = 0;
                            State = State_End;
                            NPC.netUpdate = true;
                        }
                    }
                    return;


                //-----------------------------------------------------------------------------
                case State_End:
                    {
                        End();

                        // Reset variables
                        MoveOffset = Vector2.Zero;
                        MoveDestination = Vector2.Zero;
                        MovePrediction = Vector2.Zero;
                        MoveRetryCounter = 0;
                        ElapsedMoveTime = 0;
                        TotalMoveTime = 0;
                        Timer = 0;
                        SlowdownTime = 0;
                        SlowdownDistance = 0f;
                        _slowdownDirection = 0;

                        if (Main.netMode == NetmodeID.MultiplayerClient) return;

                        // Remove this phase from the queue
                        if (PhaseQueue.Count > 0 && PhaseQueue[0] == Phase)
                            PhaseQueue.RemoveAt(0);

                        State = State_Decision;
                        NPC.netUpdate = true;
                    }
                    return;


                //-----------------------------------------------------------------------------
                case State_Despawn:
                    {
                        // *Nothing*
                    }
                    return;


                //-----------------------------------------------------------------------------
                default:
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            State = State_Decision;
                            NPC.netUpdate = true;
                        }
                    }
                    return;
            }

        }
        #endregion

        #region Movement_Helpers
        protected float GetRandomOffsetAngle(float minRange, float maxRange) => minRange * (1f - (NPC.position.Y % 1f)) + maxRange * (NPC.position.Y % 1f);

        protected float GetRandomOffsetDistance(float minRange, float maxRange) => minRange + NPC.position.X % 1f * (maxRange - minRange);

        protected void CalculateSlowdown()
        {
            SlowdownDistance = 0f;
            SlowdownTime = 0;
            _slowdownDirection = Math.Max(NPC.velocity.Length(), 0.5f) < Math.Max(SlowdownSpeed(), 0.5f) ? 1 : -1;

            if (_slowdownDirection == 1)
            {
                for (float f = Math.Max(NPC.velocity.Length(), 0.5f); f < Math.Max(SlowdownSpeed(), 0.5f); f *= MoveAccelerationRate())
                {
                    SlowdownDistance += f;
                    SlowdownTime++;
                }
            }
            else
            {
                for (float f = Math.Max(NPC.velocity.Length(), 0.5f); f > Math.Max(SlowdownSpeed(), 0.5f); f *= MoveDecelerationRate())
                {
                    SlowdownDistance += f;
                    SlowdownTime++;
                }
            }
        }

        protected void CalculateMovement(float distance, bool setTimer = true, int minMoveTime = -1)
        {
            // ---------------------------------------------------------- \\
            // --------------------- PREPARE INPUTS --------------------- \\
            // ---------------------------------------------------------- \\

            // Times
            minMoveTime = minMoveTime <= -1 ? Math.Max(MoveMinimumTime(), 0) : minMoveTime;
            int maxMoveTime = Math.Max(MoveMaximumTime(), MoveMinimumTime());

            // Speed
            float maxSpeed = Math.Max(MoveMaximumSpeed(), 0.01f); // Maximum move speed
            float startSpeed = Math.Max(NPC.velocity.Length(), 0.5f); // Start speed of this movement
            float stopSpeed = Math.Max(SlowdownSpeed(), 0.5f); // Stop speed of this movement

            // ---------------------------------------------------------- \\
            // ----------------------- Variables ------------------------ \\
            // ---------------------------------------------------------- \\

            // Default needed vars
            int MoveTime = 0;
            TargetSpeed = maxSpeed;
            bool distanceReached = false;

            // Binary Search
            bool binarySearchTimeActive = false;
            bool binarySearchDistanceActive = false;
            int accuracy = 100;
            float upperTargetSpeed = 0f;
            float lowerTargetSpeed = 0f;

            // ---------------------------------------------------------- \\
            // ------------------------- Loop --------------------------- \\
            // ---------------------------------------------------------- \\

            // Main loop, protected by a for loop with maximum execution amount
            for (int i = 0; i < 100; i++)
            {
                float calculatedDistance = distance;
                bool accelerationDirection = startSpeed < Math.Max(TargetSpeed, 0.5f);
                bool decelerationDirection = stopSpeed < Math.Max(TargetSpeed, 0.5f);

                int accelerationTime = 0;
                int decelerationTime = 0;
                float accelerationDistance = 0f;
                float decelerationDistance = 0f;

                // ---------------------------------------------------------------
                // Calculate the acceleration time and distance with the current TargetSpeed
                if (accelerationDirection)
                {
                    for (float f = startSpeed; f < TargetSpeed; f *= MoveAccelerationRate())
                    {
                        f = f > TargetSpeed ? TargetSpeed : f;
                        accelerationDistance += f;
                        accelerationTime++;
                    }
                }
                else
                {
                    for (float f = startSpeed; f > TargetSpeed; f *= MoveDecelerationRate())
                    {
                        f = f < TargetSpeed ? TargetSpeed : f;
                        accelerationDistance += f;
                        accelerationTime++;
                    }
                }

                // Calculate the deceleration time and distance with the current TargetSpeed
                // Only if slowdown has been enabled and the slowdowntype is a straight movement
                if (HasSlowdown() && MoveCalculationHasSlowdown())
                {
                    if (decelerationDirection)
                    {
                        for (float f = TargetSpeed; f > stopSpeed; f *= MoveDecelerationRate())
                        {
                            f = f < stopSpeed ? stopSpeed : f;
                            decelerationDistance += f;
                            decelerationTime++;
                        }
                    }
                    else
                    {
                        for (float f = TargetSpeed; f < stopSpeed; f *= MoveAccelerationRate())
                        {
                            f = f > stopSpeed ? stopSpeed : f;
                            decelerationDistance += f;
                            decelerationTime++;
                        }
                    }
                }

                // Check if the Acceleration + DecelerationDistance exceed the given Distance
                if (accelerationDistance + decelerationDistance > calculatedDistance)
                {
                    // Assign the movetime and TotalDistance
                    MoveTime = accelerationTime + (MoveCalculationAddDecelerationTime() ? decelerationTime : 0);

                    // Break when the Distance has been reached while stepping down
                    if (distanceReached)
                        break;

                    // The StartSpeed and StopSpeed are above the TargetSpeed AND the
                    // distance has been reached. This means we cannot solve this 
                    // movement with the given parameters. Start stepping down from
                    // StartSpeed until the Distance expires again.
                    if (!distanceReached && !accelerationDirection && !decelerationDirection)
                    {
                        if (!MoveEnforceMinimumTime())
                        {
                            distanceReached = true;
                            TargetSpeed = startSpeed;
                            continue;
                        }
                        else
                            calculatedDistance = accelerationDistance + decelerationDistance;
                    }

                    // Distance to fully accelerate or decelerate exceeds distance
                    // entirely. Which means there is no solution for this movement.
                    if ((!accelerationDirection && decelerationDirection)
                        || (accelerationDirection && !decelerationDirection))
                    {
                        if (!MoveEnforceMinimumTime())
                            break;
                        else
                            calculatedDistance = accelerationDistance + decelerationDistance;
                    }
                }

                // While the Distance has been reached keep lowering the TargetSpeed
                // Until we trigger the distance check above to get the proper TargetSpeed.
                if (distanceReached)
                {
                    TargetSpeed *= Math.Clamp(MoveDecelerationRate(), 0f, 1f);
                    continue;
                }

                // Calculate the Distances and Times of the current movement
                float remainingDistance = calculatedDistance - (accelerationDistance + decelerationDistance);
                int constantTime = (int)Math.Max(remainingDistance / TargetSpeed, 0);
                float totalDistance = accelerationDistance + decelerationDistance + (constantTime * TargetSpeed);

                MoveTime = accelerationTime + constantTime + (MoveCalculationAddDecelerationTime() ? decelerationTime : 0);
                TotalMoveTime = MoveTime + decelerationTime;

                // Check if we need to run the Binary search
                if (remainingDistance >= 0f && MoveTime >= minMoveTime && !binarySearchTimeActive && !binarySearchDistanceActive)
                    break;

                // ----------------------------------------------
                // Binary Search algorithm
                // ----------------------------------------------

                if (binarySearchTimeActive || binarySearchDistanceActive)
                {
                    // if both searches are activated prefer the time
                    if (binarySearchTimeActive && binarySearchDistanceActive)
                        binarySearchDistanceActive = false;

                    // Keep running until Accuracy runs out
                    if (accuracy-- < 0)
                        break;

                    // Found the curresponding TargetSpeed to produce this MinMoveTime
                    // and/or distance
                    if ((binarySearchTimeActive && MoveTime == minMoveTime)
                        || (binarySearchDistanceActive
                        && totalDistance <= calculatedDistance + TargetSpeed / 2
                        && totalDistance >= calculatedDistance - TargetSpeed / 2))
                        break;
                    // Change the lower bound
                    else if ((binarySearchTimeActive && MoveTime > minMoveTime)
                        || (binarySearchDistanceActive && totalDistance < calculatedDistance))
                    {
                        lowerTargetSpeed = TargetSpeed;
                        TargetSpeed = (upperTargetSpeed + lowerTargetSpeed) * 0.5f;
                    }
                    // Change the upper bound
                    else if ((binarySearchTimeActive && MoveTime < minMoveTime)
                        || (binarySearchDistanceActive && totalDistance > calculatedDistance))
                    {
                        upperTargetSpeed = TargetSpeed;
                        TargetSpeed = (lowerTargetSpeed + upperTargetSpeed) * 0.5f;
                    }
                }

                // If the calculated MoveTime is lower than the minimum move time:
                // start the time binary search and set the boundries.
                if (!binarySearchTimeActive && MoveTime < minMoveTime)
                {
                    upperTargetSpeed = TargetSpeed;
                    lowerTargetSpeed = 0f;
                    TargetSpeed *= 0.5f;
                    binarySearchTimeActive = true;
                }

                // If the time search is not active yet, and the acceleration + deceleration
                // distance is greater than the given distance, start distance search
                if (!binarySearchTimeActive && !binarySearchDistanceActive && remainingDistance < 0f)
                {
                    upperTargetSpeed = TargetSpeed;
                    lowerTargetSpeed = 0f;
                    TargetSpeed *= 0.5f;
                    binarySearchDistanceActive = true;
                }
            }

            // Apply maximum move time
            if (setTimer)
                Timer = Math.Min(MoveTime, maxMoveTime);
        }
        #endregion

        #region Other
        private bool TargetIsValid()
        {
            Player player = Main.player[NPC.target];
            return player.active && !player.dead;
        }

        protected void Despawn()
        {
            // Despawn has been called, cleanup
            // and change state to despawn
            if (State != State_Despawn)
            {
                PostAttack();

                if (Main.netMode == NetmodeID.MultiplayerClient) return;

                NPC.velocity = DespawnMoveDirection();
                State = State_Despawn;
                NPC.netUpdate = true;
            }
        }

        #endregion

        #region ModifyHit

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            float dmgReduction = 0f;

            // Homing projectile resistance. Yes, focus shot will be less effective.
            if (ProjectileID.Sets.CultistIsResistantTo[projectile.type])
                dmgReduction += 0.25f;

            if (LastStageBonusResistance())
                dmgReduction += 0.2f;

            modifiers.SourceDamage *= 1 - dmgReduction;
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            float dmgReduction = 0f;

            if (LastStageBonusResistance())
                dmgReduction += 0.2f;

            modifiers.SourceDamage *= 1 - dmgReduction;
        }

        private bool LastStageBonusResistance() => Tier >= 6 && Stage == NumStages - 1;

        public void RevengeanceDamageResistance()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
                bool revengeance = (bool)calamity.Call("GetDifficultyActive", "revengeance");
                NPC.dontTakeDamage = revengeance && Overkill();
            }
        }

        public bool Overkill()
        {
            int currentSpellCard = Stage;
            float healthVsSpellCard = NPC.lifeMax / NumStages;
            float healthLost = NPC.lifeMax - NPC.life;
            return currentSpellCard != (int)(healthLost / healthVsSpellCard) && currentSpellCard < (int)(healthLost / healthVsSpellCard);
        }

        #endregion ModifyHit

        #region Shop
        protected static void InformAboutNewShopItems()
        {
            string info = Language.GetTextValue("Mods.Gensokyo.Miscellaneous.NewShopInventory");
            Color textColor = Color.DodgerBlue;
            if (Main.dedServ)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(info), textColor);
            else
                Main.NewText(info, textColor);
        }
        #endregion

        #region Sound
        /*
        protected void PlayCustomSound(string name, Vector2 position, float volume = 1f, float pitchVariance = 0f)
        {
            if (Main.dedServ)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)Gensokyo.GensokyoMessageType.PlayCustomSound);
                packet.Write(name);
                packet.Write(volume);
                packet.Write(pitchVariance);
                packet.Write((int)position.X);
                packet.Write((int)position.Y);

                packet.Send();
                return;
            }

            if ((int)position.X == -1 || (int)position.Y == -1)
            {
                SoundEngine.PlaySound(
                    new SoundStyle("Gensokyo/Sounds/Custom/" + name) with { Volume = volume, PitchVariance = pitchVariance },
                    Main.LocalPlayer.Center
                );
            }
            else
            {
                SoundEngine.PlaySound(
                    new SoundStyle("Gensokyo/Sounds/Custom/" + name) with { Volume = volume, PitchVariance = pitchVariance },
                    position
                );
            }
        }
        */
        #endregion

        #region Network
        protected enum BossMessageType : byte
        {
            TargetList,
            SeijaVectorReversal,
            SeijaGravityReversal
        }
        /*
        protected ModPacket GetBossPacket(BossMessageType type)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)Gensokyo.GensokyoMessageType.Boss);
            packet.Write(NPC.whoAmI);
            packet.Write((byte)type);
            return packet;
        }
        */

        /*
        public void HandlePacket(BinaryReader reader)
        {
            // All boss message types are only server => client
            BossMessageType type = (BossMessageType)reader.ReadByte();
            switch (type)
            {
                case BossMessageType.TargetList:
                    int numTargets = reader.ReadByte();
                    Targets.Clear();
                    for (int k = 0; k < numTargets; k++)
                    {
                        Targets.Add(reader.ReadByte());
                    }
                    break;

                case BossMessageType.SeijaVectorReversal:
                case BossMessageType.SeijaGravityReversal:
                    if (this is SeijaKijin.SeijaKijin seija)
                    {
                        seija.HandlePacket((int)type, reader);
                    }
                    break;

                default:
                    Mod.Logger.Warn("Gensokyo: Unknown BossMessageType: " + type);
                    break;
            }
        }
        */
        /*
        public void RefreshTargetList()
        {
            // Clear and then refill the target list
            Targets.Clear();
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead && Vector2.Distance(player.Center, NPC.Center) <= 2500)
                {
                    Targets.Add(player.whoAmI);
                }
            }

            // If the boss AI is exectuted by the server, also send the target list to all players
            if (Main.dedServ)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)Gensokyo.GensokyoMessageType.Boss);
                packet.Write(NPC.whoAmI);
                packet.Write((byte)BossMessageType.TargetList);
                packet.Write((byte)Targets.Count);
                foreach (int target in Targets)
                {
                    packet.Write((byte)target);
                }
                packet.Send();
            }
        }
        */
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(State);
            writer.Write(Stage);
            writer.Write(Phase);
            writer.Write(Timer);
            writer.Write(TargetSpeed);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            State = reader.ReadInt32();
            Stage = reader.ReadInt32();
            Phase = reader.ReadInt32();
            Timer = reader.ReadInt32();
            TargetSpeed = reader.ReadSingle();
            base.ReceiveExtraAI(reader);
        }
        #endregion
    }
}