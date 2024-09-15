using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.Projectiles.NPCs;

namespace TF2.Content.NPCs.Enemies
{
    public class EnemySniperNPC : BLUMercenary
    {
        protected override Asset<Texture2D> Spritesheet => ModContent.Request<Texture2D>("TF2/Content/NPCs/Enemies/SniperNPC");

        protected override Asset<Texture2D> SpritesheetReverse => ModContent.Request<Texture2D>("TF2/Content/NPCs/Enemies/SniperNPC_Reverse");

        public override int BaseHealth => 125;

        protected override int Weapon => ModContent.ItemType<SniperRifle>();

        protected override double Damage => 150;

        protected override int AttackSpeed => TF2.Time(1.5);

        protected override int ClipSize => 1;

        protected override float Range => 2500f;

        public override bool NoDamageModifier => true;

        private int zoomDelay;
        private int chargeTimer;

        protected override void EnemyStatistics() => SetEnemyStatistics("TF2/Content/Sounds/SFX/Voicelines/sniper_painsevere01", "TF2/Content/Sounds/SFX/Voicelines/sniper_paincriticaldeath01");

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.GetModPlayer<TF2Player>().ClassSelected && NPC.downedPlantBoss ? 0.025f : 0f;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("TF2.Bestiary.Sniper")),
            ]);

        protected override void EnemySpawn() => zoomDelay = TF2.Time(1.3);

        protected override void EnemyFrame()
        {
            if (State == StateAttack && weaponAnimation <= 0 && (zoomDelay < TF2.Time(1.3) || chargeTimer < TF2.Time(2)))
            {
                UpdateMoveDirections();
                NPC.frameCounter = 0;
                horizontalFrame = 0;
                verticalFrame = 0;
                NPC.netUpdate = true;
                return;
            }
            base.EnemyFrame();
        }

        protected override void EnemyAttack(Player target)
        {
            if (Reloading || Falling || zoomDelay < TF2.Time(1.3) || chargeTimer < TF2.Time(2)) return;
            AttackTimer++;
            if (AttackTimer >= AttackSpeed && Ammo > 0)
            {
                NPC.velocity.X = 0f;
                Vector2 shootVel = NPC.DirectionTo(target.Center);
                itemRotation = NPC.AngleTo(target.Center);
                NPC.spriteDirection = NPC.direction = (itemRotation >= -MathHelper.PiOver2 && itemRotation <= MathHelper.PiOver2) ? 1 : -1;
                float speed = 10f;
                int type = ModContent.ProjectileType<EnemyBulletNPC>();
                int damage = GetDamage();
                IEntitySource projectileSource = NPC.GetSource_FromAI();
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/sniper_shoot"), NPC.Center);
                EnemyShoot(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f);
                Ammo--;
                AttackTimer = 0;
                weaponAnimation = AttackSpeed;
                zoomDelay = 0;
                chargeTimer = 0;
                NPC.netUpdate = true;
            }
        }

        protected override void EnemyUpdate()
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

        protected override void EnemySendExtraAI(BinaryWriter writer)
        {
            writer.Write(zoomDelay);
            writer.Write(chargeTimer);
        }

        protected override void EnemyReceiveExtraAI(BinaryReader binaryReader)
        {
            zoomDelay = binaryReader.ReadInt32();
            chargeTimer = binaryReader.ReadInt32();
        }
    }
}