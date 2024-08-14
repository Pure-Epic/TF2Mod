using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Projectiles;
using TF2.Gensokyo.Common;
using TF2.Gensokyo.Content.NPCs.Byakuren_Hijiri;

namespace TF2.Gensokyo.Content.NPCs
{
    // Old code. Bosses now use the same AI as the Gensokyo Mod bosses.
    // Code is for bosses inspired from the Gensokyo Mod. It is modified the original source code from that mod.
    // Bosses implemented with this class are immune to the Cursed Decoy Doll and are capable of custom dialogue.
    // Bosses implemented with this class will not run without the Gensokyo Mod as they will not load.
    // Variables are renamed and some code have been tweaked to my liking.
    // If the code is too similar to the source code, please contact me and I will take this code down (and maybe salvage bosses for the actual Gensokyo mod).
    // I got approval by Rhoenicx
    [ExtendsFromMod("Gensokyo")]
    public abstract class GensokyoBossLegacy : ModNPC
    {
        public int BossAI
        {
            get => (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public int SpellCard
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        public int MoveTimer
        {
            get => (int)NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        public int AttackTimer
        {
            get => (int)NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        public int tier;
        public float damageScale = 1f;
        public int spellCardAmount = 1;
        public int[] movementDuration;
        public int[] attackDuration;
        public int[] spellCardDuration;
        public float speed;
        private bool initializeBoss;
        public Player targetPlayer;
        private float targetPlayerSpeed;
        private bool spellCardAnnounced;
        public bool dialogueActive;
        private bool finishedDialogue;
        public int attackType;
        public bool finishedSwitchingAttacks;
        public bool usedNonSpellCardDanmaku;
        public bool enableRevengeanceDamageResistance;

        public override bool CheckActive() => false;

        private bool TargetPlayerFound()
        {
            Player player = Main.player[NPC.target];
            targetPlayer = player;
            return !player.dead;
        }

        private float GetTargetPlayerDistance() => NPC.Distance(targetPlayer.Center);

        private void GetSpellCard()
        {
            int currentSpellCard = SpellCard;
            float healthVsSpellCard = NPC.lifeMax / spellCardAmount;
            float healthLost = NPC.lifeMax - NPC.life;
            SpellCard = (int)(healthLost / healthVsSpellCard);
            if (currentSpellCard != SpellCard && currentSpellCard < SpellCard)
            {
                spellCardAnnounced = false;
                usedNonSpellCardDanmaku = false;
            }
        }

        public int FacePlayer() => targetPlayer.Center.X >= NPC.Center.X ? 1 : -1;

        public virtual void MoveTowardsPlayer(float maxSpeed)
        {
            Vector2 vector = new Vector2(0f, 192f) + Utils.RotatedBy(new Vector2(0f, 160f), MathHelper.ToRadians(Main.rand.Next(-60, 61)), default);
            Vector2 destination;
            if (targetPlayer.Center.Y < 2000f)
                destination = targetPlayer.Center + vector;
            else
                destination = targetPlayer.Center - vector;
            float distance = Vector2.Distance(destination, NPC.Center);
            int movementTimer = movementDuration[SpellCard];
            if (!initializeBoss)
            {
                movementTimer += (int)(distance / maxSpeed);
                MoveTimer += (int)(distance / maxSpeed);
                initializeBoss = true;
            }
            targetPlayerSpeed = (distance / movementTimer > maxSpeed) ? maxSpeed : (distance / movementTimer);
            NPC.velocity = NPC.DirectionTo(destination);
        }

        public override void AI()
        {
            if (enableRevengeanceDamageResistance)
                RevengeanceDamageResistance();

            // Custom AI here (Recycles the timers)

            // Bool here for during custom movement and attack styles disabling normal AI
            switch (BossAI)
            {
                case 0: // Despawn code
                    if (!TargetPlayerFound() && !UniqueDespawn())
                    {
                        NPC.velocity.Y -= 0.04f;
                        NPC.EncourageDespawn(10); // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
                        return;
                    }
                    MoveTimer = 0;
                    AttackTimer = 0;
                    BossAI = 1;
                    NPC.netUpdate = true;
                    finishedSwitchingAttacks = false;
                    finishedDialogue = false;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    return;

                case 1: // Prepare to move towards the player
                    PrepareMoving();
                    return;

                case 2: // Move towards the player
                    Move();
                    return;

                case 3: // Pre-spell card behavior
                    GetAttack();
                    return;

                case 4: // Text spam
                    Slowdown();
                    return;

                case 5: // Call spell cards after pre-spell card behavior
                    AttackWindup();
                    return;

                case 6: // The attack (finally!)
                    if (Main.netMode != NetmodeID.MultiplayerClient && !finishedSwitchingAttacks)
                    {
                        SwitchAttack();
                        finishedSwitchingAttacks = true;
                    }
                    switch (attackType)
                    {
                        case 0:
                            if (AttackTimer < attackDuration[SpellCard])
                            {
                                BasicAttack();
                                AttackTimer++;
                                return;
                            }
                            break;

                        case 1:
                            if (AttackTimer < spellCardDuration[SpellCard])
                            {
                                SpellCardAttack();
                                AttackTimer++;
                                return;
                            }
                            break;

                        case 2:
                            // This was originally for post spell card attacks
                            break;

                        default:
                            break;
                    }
                    PostSpellCardAttack();
                    if (attackType == 0)
                        usedNonSpellCardDanmaku = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        BossAI = 0;
                        NPC.netUpdate = true;
                    }
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    return;

                default:
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        BossAI = 0;
                        NPC.netUpdate = true;
                    }
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    return;
            }
        }

        public virtual void PrepareMoving()
        {
            NPC.direction = FacePlayer();
            GetSpellCard();
            if (!usedNonSpellCardDanmaku)
                attackType = 0;
            if (Main.netMode == NetmodeID.MultiplayerClient || UniqueBossMovement())
                return;
            MoveTowardsPlayer(speed);
            BossAI = 2;
            NPC.netUpdate = true;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
        }

        public virtual void Move()
        {
            MoveTimer++;
            // Check for player distance
            // If the right distance, set state to 3
            // Else run if statement the code below
            if (MoveTimer < movementDuration[SpellCard])
            {
                if (NPC.velocity.Length() < targetPlayerSpeed)
                    NPC.velocity *= 1.075f;
                return;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                BossAI = 3;
                NPC.netUpdate = true;
            }
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
        }

        public virtual void GetAttack()
        {
            GetSpellCard();
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            NPC.TargetClosest(true);
            BossAI = (GetTargetPlayerDistance() >= 1750f) ? 0 : 4;
            NPC.netUpdate = true;
        }

        public virtual void Slowdown()
        {
            if (Main.netMode != NetmodeID.Server && !finishedDialogue)
            {
                Dialogue();
                if (dialogueActive)
                    return;
            }
            if (Main.netMode != NetmodeID.Server && !spellCardAnnounced && attackType == 1)
            {
                GetSpellCardName();
                spellCardAnnounced = true;
            }
            NPC.velocity *= 0.93f;
            if (NPC.velocity.Length() <= 0.5f)
            {
                NPC.velocity = Vector2.Zero;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    BossAI = SpellCardAttackDelay() ? 5 : 6;
                    NPC.netUpdate = true;
                }
            }
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
        }

        public virtual void AttackWindup()
        {
            if (ChargeUpCompleted() && Main.netMode != NetmodeID.MultiplayerClient)
            {
                BossAI = 6;
                NPC.netUpdate = true;
            }
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
        }

        public void RevengeanceDamageResistance()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
                bool revengeance = (bool)calamity.Call("GetDifficultyActive", "revengeance");
                NPC.dontTakeDamage = revengeance && (attackType == 0 || Overkill());
            }
        }

        public bool Overkill()
        {
            int currentSpellCard = SpellCard;
            float healthVsSpellCard = NPC.lifeMax / spellCardAmount;
            float healthLost = NPC.lifeMax - NPC.life;
            if (currentSpellCard != (int)(healthLost / healthVsSpellCard) && currentSpellCard < (int)(healthLost / healthVsSpellCard)) return true;
            return false;
        }

        public virtual bool UniqueDespawn() => false;

        public virtual bool UniqueBossMovement() => false;

        public virtual void SwitchAttack()
        { }

        public virtual void BasicAttack()
        { }

        public virtual bool SpellCardAttackDelay() => false;

        public virtual bool ChargeUpCompleted() => false;

        public virtual void Dialogue() => finishedDialogue = true;

        public virtual void GetSpellCardName()
        { }

        public virtual void SpellCardAttack()
        { }

        public virtual void PostSpellCardAttack()
        { }

        public bool CheckFinalSpellCardDamageResistance() => SpellCard == spellCardAmount - 1 && tier >= 6;
    }

    [ExtendsFromMod("Gensokyo")]
    public class CalamityBossDamage : ModPlayer
    {
        // Moved to TF2GlobalNPC
        /*
        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (!(ModLoader.TryGetMod("Gensokyo", out Mod gensokyo)) || !(ModLoader.TryGetMod("CalamityMod", out Mod calamity))) return;
            bool revengeance = (bool)calamity.Call("GetDifficultyActive", "revengeance");
            bool death = (bool)calamity.Call("GetDifficultyActive", "death");
            if ((npc.ModNPC?.Mod == gensokyo || npc.ModNPC?.Mod is TF2) && !npc.friendly)
            {
                if (revengeance && !death)
                    npc.damage = TF2.Round(NPC.damage * 1.25f);
                else if (death)
                    npc.damage = TF2.Round(NPC.damage * 2.5f);
            }
        }
        */

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (GensokyoDLC.Gensokyo == null || !ModLoader.TryGetMod("CalamityMod", out Mod calamity)) return;
            bool revengeance = (bool)calamity.Call("GetDifficultyActive", "revengeance");
            bool death = (bool)calamity.Call("GetDifficultyActive", "death");
            if (proj.ModProjectile?.Mod == GensokyoDLC.Gensokyo || proj.ModProjectile?.Mod is TF2 && (proj.ModProjectile as TF2Projectile).spawnedFromNPC && proj.hostile)
            {
                if (revengeance && !death)
                    modifiers.SourceDamage *= 1.25f;
                else if (death)
                    modifiers.SourceDamage *= 2.5f;
            }
        }

        public override void PostItemCheck()
        {
            bool modActive = GensokyoDLC.gensokyoLoaded;
            if (!modActive) return;

            if (BannedItemDetected(GensokyoDLC.Gensokyo) && NPC.CountNPCS(ModContent.NPCType<ByakurenHijiri>()) >= 1)
            {
                if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
                {
                    bool revengeance = (bool)calamity.Call("GetDifficultyActive", "revengeance");
                    bool death = (bool)calamity.Call("GetDifficultyActive", "death");
                    if (revengeance && !death)
                    {
                        TF2.Dialogue("I can't let your tricks go unnoticed.", Color.DarkMagenta);
                        Player.HeldItem.type = ItemID.None;
                        Player.Hurt(PlayerDeathReason.ByCustomReason(Player.name + " thought that cheesing a boss was a great idea."), 100, 0);
                    }
                    else if (death)
                    {
                        TF2.Dialogue("You know that I don't play such games.", Color.DarkMagenta);
                        Player.KillMe(PlayerDeathReason.ByCustomReason(Player.name + " thought that cheesing a boss was a great idea."), Player.statLife, 0);
                    }
                }
                else if (Main.expertMode)
                {
                    TF2.Dialogue("Sorry sweetie, not allowed!", Color.DarkMagenta);
                    Player.HeldItem.type = ItemID.None;
                }
            }
        }

        private bool BannedItemDetected(Mod gensokyo)
        {
            gensokyo.TryFind("VectorReversal", out ModItem vectorReversal);
            gensokyo.TryFind("CrowTenguCamera", out ModItem crowTenguCamera);
            gensokyo.TryFind("SpiritPhotographyCamera", out ModItem spiritPhotographyCamera);
            gensokyo.TryFind("LunaDial", out ModItem lunaDial);
            gensokyo.TryFind("LunarClock", out ModItem lunarClock);
            return
                Player.HeldItem.type == vectorReversal.Type ||
                Player.HeldItem.type == crowTenguCamera.Type ||
                Player.HeldItem.type == spiritPhotographyCamera.Type ||
                Player.HeldItem.type == lunaDial.Type ||
                Player.HeldItem.type == lunarClock.Type;
        }
    }

    [ExtendsFromMod("Gensokyo")]
    public class BossTesterSupport : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (projectile.Name == "Boss Tester_ Projectile")
            {
                if (target.ModNPC is GensokyoBoss npc)
                {
                    modifiers.FinalDamage.Base = (int)(target.lifeMax / (float)npc.NumStages) + 5 * target.defense;
                    modifiers.DisableCrit();
                }
            }
        }
    }
}