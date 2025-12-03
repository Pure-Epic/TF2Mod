using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.Projectiles;

namespace TF2.Content.NPCs.Buddies
{
    public class SniperNPC : Buddy
    {
        protected override Asset<Texture2D> Spritesheet => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/SniperNPC");

        protected override Asset<Texture2D> SpritesheetReverse => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/SniperNPC_Reverse");

        public override int BaseHealth => 125;

        protected override int Weapon => ModContent.ItemType<SniperRifle>();

        protected override int AttackSpeed => TF2.Time(1.5);

        protected override int ClipSize => 1;

        protected override float Range => 2500f;

        public override bool NoDamageModifier => true;

        private int zoomDelay;
        private int chargeTimer;

        protected override void BuddyStatistics() => SetBuddyStatistics(450, "TF2/Content/Sounds/SFX/Voicelines/sniper_painsevere01", "TF2/Content/Sounds/SFX/Voicelines/sniper_paincriticaldeath01");

        protected override void BuddySpawn() => zoomDelay = TF2.Time(1.3);

        protected override void BuddyFrame()
        {
            if (State == StateAttack && weaponAnimation <= 0 && (zoomDelay < TF2.Time(1.3) || chargeTimer < TF2.Time(2)))
            {
                NPC.frameCounter = 0;
                horizontalFrame = 0;
                verticalFrame = 0;
                NPC.netUpdate = true;
                return;
            }
            base.BuddyFrame();
        }

        protected override void BuddyAttack(NPC target)
        {
            if (weaponAnimation <= 0 && (zoomDelay < TF2.Time(1.3) || chargeTimer < TF2.Time(2)))
            {
                NPC.direction = target.Center.X >= NPC.Center.X ? 1 : -1;
                NPC.directionY = target.Center.Y >= NPC.Center.Y ? 1 : -1;
                return;
            }
            if (Reloading || Falling || zoomDelay < TF2.Time(1.3) || chargeTimer < TF2.Time(2)) return;
            AttackTimer++;
            if (AttackTimer >= AttackSpeed && Ammo > 0)
            {
                NPC.velocity.X = 0f;
                Vector2 shootVel = NPC.DirectionTo(target.Center);
                itemRotation = NPC.AngleTo(target.Center);
                NPC.spriteDirection = NPC.direction = (itemRotation >= -MathHelper.PiOver2 && itemRotation <= MathHelper.PiOver2) ? 1 : -1;
                float speed = 10f;
                int type = ModContent.ProjectileType<Bullet>();
                int damage = TF2.Round(NPC.damage / 2 * Player.GetModPlayer<TF2Player>().damageMultiplier);
                IEntitySource projectileSource = NPC.GetSource_FromAI();
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/sniper_shoot"), NPC.Center);
                BuddyShoot(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, Owner);
                Ammo--;
                AttackTimer = 0;
                weaponAnimation = AttackSpeed;
                zoomDelay = 0;
                chargeTimer = 0;
                NPC.netUpdate = true;
            }
        }

        protected override void BuddyUpdate()
        {
            if (State == StateAttack)
            {
                zoomDelay++;
                if (zoomDelay > TF2.Time(1.3))
                    zoomDelay = TF2.Time(1.3);
                if (zoomDelay >= TF2.Time(1.3))
                {
                    chargeTimer++;
                    if (chargeTimer > TF2.Time(2))
                        chargeTimer = TF2.Time(2);
                }
            }
            NPC.netUpdate = true;
        }

        protected override void BuddySendExtraAI(BinaryWriter writer)
        {
            writer.Write(zoomDelay);
            writer.Write(chargeTimer);
        }

        protected override void BuddyReceiveExtraAI(BinaryReader binaryReader)
        {
            zoomDelay = binaryReader.ReadInt32();
            chargeTimer = binaryReader.ReadInt32();
        }
    }
}