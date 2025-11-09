using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Demoman;
using TF2.Content.Projectiles.NPCs;

namespace TF2.Content.NPCs.Buddies
{
    public class DemomanNPC : MercenaryBuddy
    {
        protected override Asset<Texture2D> Spritesheet => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/DemomanNPC");

        protected override Asset<Texture2D> SpritesheetReverse => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/DemomanNPC_Reverse");

        public override int BaseHealth => 175;

        protected override float SpeedMuliplier => 0.93f;

        protected override int Weapon => ModContent.ItemType<GrenadeLauncher>();

        protected override int AttackSpeed => TF2.Time(0.6);

        protected override int ClipSize => 4;

        protected override int InitialReloadSpeed => TF2.Time(1.24);

        protected override int ReloadSpeed => TF2.Time(0.6);

        protected override string ReloadSound => "TF2/Content/Sounds/SFX/Weapons/grenade_launcher_reload";

        protected override float Range => 500f;

        public override bool NoDamageModifier => true;

        protected override void BuddyStatistics() => SetBuddyStatistics(100, "TF2/Content/Sounds/SFX/Voicelines/demoman_painsevere01", "TF2/Content/Sounds/SFX/Voicelines/demoman_paincriticaldeath01");

        protected override void BuddyUpdateAmmo()
        {
            if (Reloading)
            {
                ReloadCooldownTimer++;
                if (ReloadCooldownTimer == 0)
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/grenade_launcher_drum_open"), NPC.Center);
                if (ReloadCooldownTimer >= InitialReloadSpeed && ReloadTimer >= ReloadSpeed)
                {
                    if (MagazineReload)
                        Ammo = ClipSize;
                    else
                        Ammo++;
                    if (ReloadSound != "")
                        SoundEngine.PlaySound(new SoundStyle(ReloadSound), NPC.Center);
                    ReloadTimer = 0;
                    NPC.netUpdate = true;
                }
                ReloadTimer++;
            }
            else
                ReloadTimer = 0;
            NPC.netUpdate = true;
            if (Ammo == ClipSize)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/grenade_launcher_drum_close"), NPC.Center);
                AttackTimer = AttackSpeed;
                ReloadCooldownTimer = 0;
                ReloadTimer = 0;
                State = StateIdle;
                NPC.netUpdate = true;
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
                itemRotation = NPC.AngleTo(target.Center);
                NPC.spriteDirection = NPC.direction = (itemRotation >= -MathHelper.PiOver2 && itemRotation <= MathHelper.PiOver2) ? 1 : -1;
                float speed = 12.5f;
                int type = ModContent.ProjectileType<GrenadeNPC>();
                int damage = TF2.Round(NPC.damage / 2 * Player.GetModPlayer<TF2Player>().damageMultiplier);
                IEntitySource projectileSource = NPC.GetSource_FromAI();
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/grenade_launcher_shoot"), NPC.Center);
                BuddyShoot(projectileSource, NPC.Center, shootVel * speed, type, damage, 5f, Owner);
                Ammo--;
                AttackTimer = 0;
                weaponAnimation = AttackSpeed;
                NPC.netUpdate = true;
            }
        }
    }
}