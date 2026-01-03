using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Medic;
using TF2.Content.Projectiles.Medic;
using TF2.Content.Projectiles.NPCs;

namespace TF2.Content.NPCs.Buddies
{
    public class MedicBuddyNPC : Buddy
    {
        protected override Asset<Texture2D> Spritesheet => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/MedicBuddyNPC");

        protected override Asset<Texture2D> SpritesheetReverse => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/MedicBuddyNPC_Reverse");

        protected override int ExtraFrames => 1;

        public override int BaseHealth => 150;

        public override float BaseSpeed => !isHealing ? 1.07f : healingMovementSpeed;

        protected override int Weapon => !canHeal ? ModContent.ItemType<SyringeGun>() : ModContent.ItemType<MediGun>();

        protected override int AttackSpeed => TF2.Time(0.105);

        protected override int ClipSize => 40;

        protected override int ReloadSpeed => TF2.Time(1.305);

        protected override string ReloadSound => "TF2/Content/Sounds/SFX/Weapons/syringegun_reload";

        protected override bool MagazineReload => true;

        protected override float Range => 1500f;

        public override float MaxDamageMultiplier => 1.2f;

        public override float DamageFalloffRange => 700f;

        protected static SoundStyle MediGunHealSound => new SoundStyle("TF2/Content/Sounds/SFX/Weapons/medigun_heal");

        protected SlotId mediGunHealSoundSlot = new SlotId();

        private int regenerationTimer;
        internal int healTimer;
        internal bool canHeal;
        internal bool isHealing;
        internal float healingMovementSpeed = 1f;

        protected override void BuddyStatistics() => SetBuddyStatistics(10, "TF2/Content/Sounds/SFX/Voicelines/medic_painsevere01", "TF2/Content/Sounds/SFX/Voicelines/medic_paincriticaldeath01");

        protected override void BuddyFrame()
        {
            if (!TF2.IsBossAlive())
            {
                if (Falling)
                {
                    NPC.frameCounter = 0;
                    horizontalFrame = (byte)((weaponAnimation <= 0) ? 1 : 7);
                    verticalFrame = 0;
                }
                else if (NPC.position == NPC.oldPosition && weaponAnimation <= 0 && !forceWeaponDraw)
                {
                    NPC.frameCounter = 0;
                    horizontalFrame = 0;
                    verticalFrame = 0;
                }
                else if (NPC.position == NPC.oldPosition)
                {
                    NPC.frameCounter = 0;
                    horizontalFrame = 2;
                    verticalFrame = 0;
                }
                else if (focus)
                {
                    NPC.frameCounter = 0;
                    horizontalFrame = (byte)(weaponAnimation <= 0 ? 0 : 2);
                    verticalFrame = 0;
                }
                else
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 2)
                    {
                        NPC.frameCounter = 0;
                        horizontalFrame++;
                        if (horizontalFrame >= 14)
                            horizontalFrame = 0;
                    }
                    verticalFrame = (byte)(weaponAnimation <= 0 ? 1 : 2);
                }
            }
            else
                base.BuddyFrame();
        }

        protected override void OverrideState()
        {
            if (!TF2.IsBossAlive())
                State = StateFollow;
        }

        protected override void BuddyFollow()
        {
            Timer = 0;
            int direction = Player.position.X >= NPC.position.X ? 1 : -1;
            AdjustMoveSpeed(ref NPC.velocity, direction, walkSpeed * BaseSpeed * speedMultiplier, moveAcceleration, moveDeceleration, moveFriction, onSolidGround);
            NPC.direction = direction;
            if (Math.Abs(Player.position.X - NPC.position.X) <= 50f || (NPC.position.Y - Player.position.Y >= 250f))
            {
                NPC.velocity.X = 0f;
                NPC.netUpdate = true;
            }
        }

        private void MediGun()
        {
            AttackTimer++;
            if (AttackTimer >= TF2.Time(0.01666))
            {
                weaponRotation = NPC.AngleTo(Player.Center);
                NPC.spriteDirection = NPC.direction = (weaponRotation >= -MathHelper.PiOver2 && weaponRotation <= MathHelper.PiOver2) ? 1 : -1;
                if (!isHealing)
                {
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    if (!SoundEngine.TryGetActiveSound(mediGunHealSoundSlot, out var _))
                        mediGunHealSoundSlot = SoundEngine.PlaySound(MediGunHealSound, NPC.Center);
                    BuddyShoot(projectileSource, NPC.Center, Vector2.Zero, ModContent.ProjectileType<BuddyHealingBeam>(), 0, 0f, Owner);
                }
                weaponAnimation = TF2.Time(0.01666);
            }
        }

        protected override void BuddyAttack(NPC target)
        {
            if (Reloading || Falling) return;
            AttackTimer++;
            if (AttackTimer >= AttackSpeed && Ammo > 0)
            {
                NPC.velocity.X = 0f;
                Vector2 shootVel = NPC.DirectionTo(target.Center);
                weaponRotation = NPC.AngleTo(target.Center);
                NPC.spriteDirection = NPC.direction = (weaponRotation >= -MathHelper.PiOver2 && weaponRotation <= MathHelper.PiOver2) ? 1 : -1;
                float speed = 25f;
                int type = ModContent.ProjectileType<Syringe>();
                int damage = TF2.Round(NPC.damage / 2 * Player.GetModPlayer<TF2Player>().damageMultiplier);
                IEntitySource projectileSource = NPC.GetSource_FromAI();
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/syringegun_shoot"), NPC.Center);
                BuddyShoot(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, Owner);
                Ammo--;
                AttackTimer = 0;
                weaponAnimation = AttackSpeed;
                NPC.netUpdate = true;
            }
        }

        protected override void BuddyUpdate()
        {
            canHeal = !TF2.IsBossAlive() && Player.active && !Player.dead && NPC.Distance(Player.Center) <= 200f;
            if (canHeal)
                MediGun();
            else
                isHealing = false;
            if (healTimer > 0)
                healTimer--;
            TF2.Minimum(ref healTimer, 0);
            regenerationTimer++;
            if (regenerationTimer >= TF2.Time(1) && NPC.life < NPC.lifeMax)
            {
                int health = TF2.Round(NPC.lifeMax / 50f);
                if ((health + NPC.life) > NPC.lifeMax)
                    health = NPC.lifeMax - NPC.life;
                NPC.life += health;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.HealEffect(health);
                regenerationTimer = 0;
                NPC.netUpdate = true;
            }
            NPC.netUpdate = true;
        }

        protected override void BuddyDie()
        {
            if (SoundEngine.TryGetActiveSound(mediGunHealSoundSlot, out var healSound))
                healSound.Stop();
        }

        protected override void BuddySendExtraAI(BinaryWriter writer)
        {
            writer.Write(regenerationTimer);
            writer.Write(healTimer);
            writer.Write(canHeal);
            writer.Write(isHealing);
            writer.Write(healingMovementSpeed);
        }

        protected override void BuddyReceiveExtraAI(BinaryReader binaryReader)
        {
            regenerationTimer = binaryReader.ReadInt32();
            healTimer = binaryReader.ReadInt32();
            canHeal = binaryReader.ReadBoolean();
            isHealing = binaryReader.ReadBoolean();
            healingMovementSpeed = binaryReader.ReadSingle();
        }
    }
}