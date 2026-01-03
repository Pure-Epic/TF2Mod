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
using TF2.Content.Projectiles.Pyro;

namespace TF2.Content.NPCs.Buddies
{
    public class PyroBuddyNPC : Buddy
    {
        protected override Asset<Texture2D> Spritesheet => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/PyroBuddyNPC");

        protected override Asset<Texture2D> SpritesheetReverse => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/PyroBuddyNPC_Reverse");

        public override int BaseHealth => 175;

        protected override int Weapon => ModContent.ItemType<FlameThrower>();

        protected override int AttackSpeed => TF2.Time(0.105);

        protected override int ClipSize => 200;

        protected override int ReloadSpeed => TF2.Time(2.5);

        protected override bool MagazineReload => true;

        protected override float Range => 200f;

        protected static SoundStyle FlameThrowerAttackSound => new SoundStyle("TF2/Content/Sounds/SFX/Weapons/flame_thrower_loop");

        protected SlotId flameThrowerAttackSoundSlot = new SlotId();

        protected override void BuddyStatistics()
        {
            SetBuddyStatistics(18, "TF2/Content/Sounds/SFX/Voicelines/pyro_painsevere01", "TF2/Content/Sounds/SFX/Voicelines/pyro_paincriticaldeath01");
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                if (TF2BuffBase.fireBuff[i])
                    NPC.buffImmune[i] = true;
            }
        }

        protected override void BuddyAttack(NPC target)
        {
            if (Reloading || Falling || NPC.wet) return;
            AttackTimer++;
            if (AttackTimer >= AttackSpeed && Ammo > 0)
            {
                NPC.velocity.X = 0f;
                Vector2 shootVel = NPC.DirectionTo(target.Center);
                weaponRotation = NPC.AngleTo(target.Center);
                NPC.spriteDirection = NPC.direction = (weaponRotation >= -MathHelper.PiOver2 && weaponRotation <= MathHelper.PiOver2) ? 1 : -1;
                float speed = 10f;
                int type = ModContent.ProjectileType<Fire>();
                int damage = TF2.Round(NPC.damage / 2 * Player.GetModPlayer<TF2Player>().damageMultiplier);
                IEntitySource projectileSource = NPC.GetSource_FromAI();
                if (!SoundEngine.TryGetActiveSound(flameThrowerAttackSoundSlot, out var _))
                    flameThrowerAttackSoundSlot = SoundEngine.PlaySound(FlameThrowerAttackSound, NPC.Center);
                BuddyShoot(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, Owner);
                Ammo--;
                AttackTimer = 0;
                weaponAnimation = AttackSpeed;
                NPC.netUpdate = true;
            }
        }

        protected override void BuddyUpdate()
        {
            if (SoundEngine.TryGetActiveSound(flameThrowerAttackSoundSlot, out var attackSound))
            {
                attackSound.Position = NPC.Center;
                if (!NPC.active || (State != StateAttack) || Falling)
                    attackSound.Stop();
            }
        }

        protected override void BuddyDie()
        {
            if (SoundEngine.TryGetActiveSound(flameThrowerAttackSoundSlot, out var attackSound))
                attackSound.Stop();
        }
    }
}