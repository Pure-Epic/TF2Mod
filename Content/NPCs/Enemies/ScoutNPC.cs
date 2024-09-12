using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Scout;
using TF2.Content.Projectiles.NPCs;

namespace TF2.Content.NPCs.Enemies
{
    public class EnemyScoutNPC : BLUMercenary
    {
        protected override Asset<Texture2D> Spritesheet => ModContent.Request<Texture2D>("TF2/Content/NPCs/Enemies/ScoutNPC");

        protected override Asset<Texture2D> SpritesheetReverse => ModContent.Request<Texture2D>("TF2/Content/NPCs/Enemies/ScoutNPC_Reverse");

        public override int BaseHealth => 125;

        protected override float SpeedMuliplier => 1.33f;

        protected override int ExtraJumps => 1;

        protected override int Weapon => ModContent.ItemType<Scattergun>();

        protected override double Damage => 6;

        protected override int AttackSpeed => TF2.Time(0.6);

        protected override int ClipSize => 6;

        protected override int InitialReloadSpeed => TF2.Time(0.6);

        protected override int ReloadSpeed => TF2.Time(0.6);

        protected override string ReloadSound => "TF2/Content/Sounds/SFX/Weapons/scatter_gun_reload";

        protected override float Range => 250f;

        protected override void EnemyStatistics() => SetEnemyStatistics("TF2/Content/Sounds/SFX/Voicelines/scout_painsevere01", "TF2/Content/Sounds/SFX/Voicelines/scout_paincriticaldeath01");

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.GetModPlayer<TF2Player>().ClassSelected ? 0.025f : 0f;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("TF2.Bestiary.Scout")),
            ]);

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
                int type = ModContent.ProjectileType<EnemyBulletNPC>();
                int damage = GetDamage();
                IEntitySource projectileSource = NPC.GetSource_FromAI();
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/scatter_gun_shoot"), NPC.Center);
                for (int i = 0; i < 10; i++)
                {
                    Vector2 newVelocity = shootVel.RotatedByRandom(MathHelper.ToRadians(12f));
                    EnemyShoot(projectileSource, NPC.Center, newVelocity * speed, type, damage, 0f);
                }
                Ammo--;
                AttackTimer = 0;
                weaponAnimation = AttackSpeed;
                NPC.netUpdate = true;
            }
        }
    }
}