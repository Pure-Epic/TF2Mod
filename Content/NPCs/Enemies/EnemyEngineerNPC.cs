using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.MultiClass;
using TF2.Content.Projectiles.NPCs;

namespace TF2.Content.NPCs.Enemies
{
    public class EnemyEngineerNPC : BLUMercenary
    {
        protected override Asset<Texture2D> Spritesheet => ModContent.Request<Texture2D>("TF2/Content/NPCs/Enemies/EnemyEngineerNPC");

        protected override Asset<Texture2D> SpritesheetReverse => ModContent.Request<Texture2D>("TF2/Content/NPCs/Enemies/EnemyEngineerNPC_Reverse");

        public override int BaseHealth => 125;

        protected override int Weapon => ModContent.ItemType<Pistol>();

        protected override double Damage => 12;

        protected override int AttackSpeed => TF2.Time(0.15);

        protected override int ClipSize => 12;

        protected override int ReloadSpeed => TF2.Time(1.035);

        protected override string ReloadSound => "TF2/Content/Sounds/SFX/Weapons/pistol_reload";

        protected override bool MagazineReload => true;

        protected override float Range => 1500f;

        private int spreadRecovery;

        protected override void EnemyStatistics() => SetEnemyStatistics("TF2/Content/Sounds/SFX/Voicelines/engineer_painsevere01", "TF2/Content/Sounds/SFX/Voicelines/engineer_paincriticaldeath01");

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.GetModPlayer<TF2Player>().ClassSelected && Main.hardMode ? 0.025f : 0f;

        protected override void EnemyAttack(Player target)
        {
            if (Reloading || Falling) return;
            AttackTimer++;
            if (AttackTimer >= AttackSpeed && Ammo > 0)
            {
                NPC.velocity.X = 0f;
                Vector2 shootVel = NPC.DirectionTo(target.Center);
                itemRotation = NPC.AngleTo(target.Center);
                NPC.spriteDirection = NPC.direction = (itemRotation >= -MathHelper.PiOver2 && itemRotation <= MathHelper.PiOver2) ? 1 : -1;
                float speed = 10f;
                int type = ModContent.ProjectileType<EnemyBullet>();
                int damage = GetDamage();
                IEntitySource projectileSource = NPC.GetSource_FromAI();
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/pistol_shoot"), NPC.Center);
                Vector2 newVelocity = spreadRecovery >= TF2.Time(1.25) ? shootVel : shootVel.RotatedByRandom(MathHelper.ToRadians(2.5f));
                EnemyShoot(projectileSource, NPC.Center, newVelocity * speed, type, damage, 0f);
                spreadRecovery = 0;
                Ammo--;
                AttackTimer = 0;
                weaponAnimation = AttackSpeed;
                NPC.netUpdate = true;
            }
        }

        protected override void EnemyUpdate()
        {
            spreadRecovery++;
            NPC.netUpdate = true;
        }

        protected override void EnemySendExtraAI(BinaryWriter writer) => writer.Write(spreadRecovery);

        protected override void EnemyReceiveExtraAI(BinaryReader binaryReader) => spreadRecovery = binaryReader.ReadInt32();
    }
}