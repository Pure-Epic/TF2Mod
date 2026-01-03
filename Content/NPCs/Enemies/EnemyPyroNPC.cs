using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Weapons.Pyro;
using TF2.Content.Projectiles.NPCs;

namespace TF2.Content.NPCs.Enemies
{
    public class EnemyPyroNPC : BLUMercenary
    {
        protected override Asset<Texture2D> Spritesheet => ModContent.Request<Texture2D>("TF2/Content/NPCs/Enemies/EnemyPyroNPC");

        protected override Asset<Texture2D> SpritesheetReverse => ModContent.Request<Texture2D>("TF2/Content/NPCs/Enemies/EnemyPyroNPC_Reverse");

        public override int BaseHealth => 175;

        protected override int Weapon => ModContent.ItemType<FlameThrower>();

        protected override double Damage => 18;

        protected override int AttackSpeed => TF2.Time(0.105);

        protected override int ClipSize => 200;

        protected override int ReloadSpeed => TF2.Time(2.5);

        protected override bool MagazineReload => true;

        protected override float Range => 200f;

        protected static SoundStyle FlameThrowerAttackSound => new SoundStyle("TF2/Content/Sounds/SFX/Weapons/flame_thrower_loop");

        protected SlotId flameThrowerAttackSoundSlot = new SlotId();

        protected override void EnemyStatistics()
        {
            SetEnemyStatistics("TF2/Content/Sounds/SFX/Voicelines/pyro_painsevere01", "TF2/Content/Sounds/SFX/Voicelines/pyro_paincriticaldeath01");
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                if (TF2BuffBase.fireBuff[i])
                    NPC.buffImmune[i] = true;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.GetModPlayer<TF2Player>().ClassSelected ? 0.025f : 0f;

        protected override void EnemyAttack(Player target)
        {
            if (Reloading || Falling || NPC.wet) return;
            AttackTimer++;
            if (AttackTimer >= AttackSpeed && Ammo > 0)
            {
                NPC.velocity.X = 0f;
                Vector2 shootVel = NPC.DirectionTo(target.Center);
                itemRotation = NPC.AngleTo(target.Center);
                NPC.spriteDirection = NPC.direction = (itemRotation >= -MathHelper.PiOver2 && itemRotation <= MathHelper.PiOver2) ? 1 : -1;
                float speed = 10f;
                int type = ModContent.ProjectileType<EnemyFire>();
                int damage = GetDamage();
                IEntitySource projectileSource = NPC.GetSource_FromAI();
                if (!SoundEngine.TryGetActiveSound(flameThrowerAttackSoundSlot, out var _))
                    flameThrowerAttackSoundSlot = SoundEngine.PlaySound(FlameThrowerAttackSound, NPC.Center);
                EnemyShoot(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f);
                Ammo--;
                AttackTimer = 0;
                weaponAnimation = AttackSpeed;
                NPC.netUpdate = true;
            }
        }

        protected override void EnemyUpdate()
        {
            if (SoundEngine.TryGetActiveSound(flameThrowerAttackSoundSlot, out var attackSound))
            {
                attackSound.Position = NPC.Center;
                if (!NPC.active || (State != StateAttack) || Falling)
                    attackSound.Stop();
            }
        }

        protected override void EnemyDie()
        {
            if (SoundEngine.TryGetActiveSound(flameThrowerAttackSoundSlot, out var attackSound))
                attackSound.Stop();
        }
    }
}