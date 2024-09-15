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
using TF2.Content.Items.Weapons.Demoman;
using TF2.Content.Projectiles.NPCs;

namespace TF2.Content.NPCs.Enemies
{
    public class EnemyDemomanNPC : BLUMercenary
    {
        protected override Asset<Texture2D> Spritesheet => ModContent.Request<Texture2D>("TF2/Content/NPCs/Enemies/DemomanNPC");

        protected override Asset<Texture2D> SpritesheetReverse => ModContent.Request<Texture2D>("TF2/Content/NPCs/Enemies/DemomanNPC_Reverse");

        public override int BaseHealth => 175;

        protected override float SpeedMuliplier => 0.93f;

        protected override int Weapon => ModContent.ItemType<GrenadeLauncher>();

        protected override double Damage => 100;

        protected override int AttackSpeed => TF2.Time(0.6);

        protected override int ClipSize => 4;

        protected override int InitialReloadSpeed => TF2.Time(1.24);

        protected override int ReloadSpeed => TF2.Time(0.6);

        protected override string ReloadSound => "TF2/Content/Sounds/SFX/Weapons/grenade_launcher_reload";

        protected override float Range => 500f;

        public override bool NoDamageModifier => true;

        protected override void EnemyStatistics() => SetEnemyStatistics("TF2/Content/Sounds/SFX/Voicelines/demoman_painsevere01", "TF2/Content/Sounds/SFX/Voicelines/demoman_paincriticaldeath01");

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.GetModPlayer<TF2Player>().ClassSelected && Main.hardMode ? 0.025f : 0f;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("TF2.Bestiary.Demoman")),
            ]);

        protected override void EnemyUpdateAmmo()
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
                float speed = 12.5f;
                int type = ModContent.ProjectileType<EnemyGrenadeNPC>();
                int damage = GetDamage();
                IEntitySource projectileSource = NPC.GetSource_FromAI();
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/grenade_launcher_shoot"), NPC.Center);
                EnemyShoot(projectileSource, NPC.Center, shootVel * speed, type, damage, 5f);
                Ammo--;
                AttackTimer = 0;
                weaponAnimation = AttackSpeed;
                NPC.netUpdate = true;
            }
        }
    }
}