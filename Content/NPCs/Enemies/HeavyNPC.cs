using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Heavy;
using TF2.Content.Projectiles.NPCs;

namespace TF2.Content.NPCs.Enemies
{
    public class EnemyHeavyNPC : BLUMercenary
    {
        protected override Asset<Texture2D> Spritesheet => ModContent.Request<Texture2D>("TF2/Content/NPCs/Enemies/HeavyNPC");

        protected override Asset<Texture2D> SpritesheetReverse => ModContent.Request<Texture2D>("TF2/Content/NPCs/Enemies/HeavyNPC_Reverse");

        public override int BaseHealth => 300;

        protected override float SpeedMuliplier => 0.77f;

        protected override int Weapon => ModContent.ItemType<Minigun>();

        protected override double Damage => 9;

        protected override int AttackSpeed => TF2.Time(0.1);

        protected override int ClipSize => 200;

        protected override int ReloadSpeed => TF2.Time(2);

        protected override bool MagazineReload => true;

        protected override float Range => 1000f;

        protected static SoundStyle MinigunSpinSound => new SoundStyle("TF2/Content/Sounds/SFX/Weapons/minigun_spin")
        {
            SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
        };

        protected static SoundStyle MinigunAttackSound => new SoundStyle("TF2/Content/Sounds/SFX/Weapons/minigun_shoot")
        {
            SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
        };

        protected static SoundStyle MinigunSpinUpSound => new SoundStyle("TF2/Content/Sounds/SFX/Weapons/minigun_wind_up");

        protected static SoundStyle MinigunSpinDownSound => new SoundStyle("TF2/Content/Sounds/SFX/Weapons/minigun_wind_down");

        private int spinTimer;
        private bool endSpinUpSound;
        private bool endSpinDownSound;
        public SlotId minigunSpinUpSoundSlot;
        public SlotId minigunSpinDownSoundSlot;
        public SlotId minigunSpinSoundSlot;
        public SlotId minigunAttackSoundSlot;

        protected override void EnemyStatistics() => SetEnemyStatistics("TF2/Content/Sounds/SFX/Voicelines/heavy_painsevere01", "TF2/Content/Sounds/SFX/Voicelines/heavy_paincriticaldeath01");

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.GetModPlayer<TF2Player>().ClassSelected && Main.hardMode ? 0.025f : 0f;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("TF2.Bestiary.Heavy")),
            ]);

        protected override void EnemyMovement()
        {
            if (spinTimer > 0) return;
            base.EnemyMovement();
        }

        protected override void EnemyAttack(Player target)
        {
            if (Reloading || Falling) return;
            if (spinTimer <= 0)
            {
                itemRotation = NPC.AngleTo(target.Center);
                NPC.spriteDirection = NPC.direction = (itemRotation >= -MathHelper.PiOver2 && itemRotation <= MathHelper.PiOver2) ? 1 : -1;
            }
            endSpinDownSound = false;
            if (!endSpinUpSound)
            {
                if (SoundEngine.TryGetActiveSound(minigunSpinDownSoundSlot, out var spinDown))
                    spinDown.Stop();
                if (!SoundEngine.TryGetActiveSound(minigunSpinUpSoundSlot, out var _))
                    minigunSpinUpSoundSlot = SoundEngine.PlaySound(MinigunSpinUpSound, NPC.Center);
                endSpinUpSound = true;
            }
            spinTimer++;
            forceWeaponDraw = spinTimer > 0;
            if (endSpinUpSound && !SoundEngine.TryGetActiveSound(minigunSpinUpSoundSlot, out var _) && !SoundEngine.TryGetActiveSound(minigunSpinSoundSlot, out var _) && !SoundEngine.TryGetActiveSound(minigunSpinDownSoundSlot, out var _) && !SoundEngine.TryGetActiveSound(minigunAttackSoundSlot, out var _))
                minigunSpinSoundSlot = SoundEngine.PlaySound(MinigunSpinSound, NPC.position);
            if (spinTimer >= TF2.Time(0.87))
            {
                spinTimer = TF2.Time(0.87);
                AttackTimer++;
                if (AttackTimer >= AttackSpeed && Ammo > 0)
                {
                    NPC.velocity.X = 0f;
                    Vector2 shootVel = NPC.DirectionTo(target.Center);
                    itemRotation = NPC.AngleTo(target.Center);
                    NPC.spriteDirection = NPC.direction = (itemRotation >= -MathHelper.PiOver2 && itemRotation <= MathHelper.PiOver2) ? 1 : -1;
                    float speed = 10f;
                    Vector2 newVelocity = shootVel.RotatedByRandom(MathHelper.ToRadians(10f));
                    int type = ModContent.ProjectileType<EnemyBulletNPC>();
                    int damage = GetDamage();
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    if (SoundEngine.TryGetActiveSound(minigunSpinSoundSlot, out var spinSound))
                        spinSound.Stop();
                    if (!SoundEngine.TryGetActiveSound(minigunAttackSoundSlot, out var _))
                        minigunAttackSoundSlot = SoundEngine.PlaySound(MinigunAttackSound, NPC.Center);
                    EnemyShoot(projectileSource, NPC.Center, newVelocity * speed, type, damage, 0f);
                    Ammo--;
                    AttackTimer = 0;
                    weaponAnimation = AttackSpeed;
                    NPC.netUpdate = true;
                }
            }
        }

        protected override void EnemyUpdate()
        {
            if (SoundEngine.TryGetActiveSound(minigunSpinSoundSlot, out var spinSound))
                spinSound.Position = NPC.Center;
            if (SoundEngine.TryGetActiveSound(minigunAttackSoundSlot, out var attackSound))
                attackSound.Position = NPC.Center;
            if (Reloading || Falling)
            {
                if (SoundEngine.TryGetActiveSound(minigunSpinUpSoundSlot, out var spinUp))
                    spinUp.Stop();
                spinSound?.Stop();
                attackSound?.Stop();
            }
        }

        protected override void EnemyUpdateWithTarget(Player target)
        {
            if (target == null)
            {
                if (SoundEngine.TryGetActiveSound(minigunSpinSoundSlot, out var spinSound))
                    spinSound.Stop();
                if (SoundEngine.TryGetActiveSound(minigunAttackSoundSlot, out var attackSound))
                    attackSound.Stop();
            }
            if (target == null || State == StateReload)
            {
                endSpinUpSound = false;
                if (!endSpinDownSound && spinTimer > 0)
                {
                    if (!SoundEngine.TryGetActiveSound(minigunSpinDownSoundSlot, out var spinDown))
                        minigunSpinDownSoundSlot = SoundEngine.PlaySound(MinigunSpinDownSound, NPC.Center);
                    endSpinDownSound = true;
                    NPC.netUpdate = true;
                }
                forceWeaponDraw = spinTimer > 0;
                spinTimer--;
                if (spinTimer < 0)
                    spinTimer = 0;
                NPC.netUpdate = true;
            }
        }

        protected override void EnemyDie()
        {
            if (SoundEngine.TryGetActiveSound(minigunSpinUpSoundSlot, out var spinUp))
                spinUp.Stop();
            if (SoundEngine.TryGetActiveSound(minigunSpinDownSoundSlot, out var spinDown))
                spinDown.Stop();
            if (SoundEngine.TryGetActiveSound(minigunSpinSoundSlot, out var spinSound))
                spinSound.Stop();
            if (SoundEngine.TryGetActiveSound(minigunAttackSoundSlot, out var attackSound))
                attackSound.Stop();
        }

        protected override void EnemySendExtraAI(BinaryWriter writer)
        {
            writer.Write(spinTimer);
            writer.Write(endSpinUpSound);
            writer.Write(endSpinDownSound);
        }

        protected override void EnemyReceiveExtraAI(BinaryReader binaryReader)
        {
            spinTimer = binaryReader.ReadInt32();
            endSpinUpSound = binaryReader.ReadBoolean();
            endSpinDownSound = binaryReader.ReadBoolean();
        }
    }
}