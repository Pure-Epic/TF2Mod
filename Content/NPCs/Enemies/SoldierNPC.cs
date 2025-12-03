using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Soldier;
using TF2.Content.Projectiles.NPCs;

namespace TF2.Content.NPCs.Enemies
{
    public class EnemySoldierNPC : BLUMercenary
    {
        protected override Asset<Texture2D> Spritesheet => ModContent.Request<Texture2D>("TF2/Content/NPCs/Enemies/SoldierNPC");

        protected override Asset<Texture2D> SpritesheetReverse => ModContent.Request<Texture2D>("TF2/Content/NPCs/Enemies/SoldierNPC_Reverse");

        public override int BaseHealth => 200;

        public override float BaseSpeed => 0.8f;

        protected override int Weapon => ModContent.ItemType<RocketLauncher>();

        protected override double Damage => 90;

        protected override int AttackSpeed => TF2.Time(0.8);

        protected override int ClipSize => 4;

        protected override int InitialReloadSpeed => TF2.Time(0.92);

        protected override int ReloadSpeed => TF2.Time(0.8);

        protected override string ReloadSound => "TF2/Content/Sounds/SFX/Weapons/rocket_reload";

        protected override float Range => 2000f;

        public override float MaxDamageMultiplier => 1.25f;

        public override float DamageFalloffRange => 750f;

        protected override void EnemyStatistics() => SetEnemyStatistics("TF2/Content/Sounds/SFX/Voicelines/soldier_painsevere01", "TF2/Content/Sounds/SFX/Voicelines/soldier_paincriticaldeath01");

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.GetModPlayer<TF2Player>().ClassSelected ? 0.025f : 0f;

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
                int type = ModContent.ProjectileType<EnemyRocketNPC>();
                int damage = GetDamage();
                IEntitySource projectileSource = NPC.GetSource_FromAI();
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/rocket_shoot"), NPC.Center);
                EnemyShoot(projectileSource, NPC.Center, shootVel * speed, type, damage, 5f);
                Ammo--;
                AttackTimer = 0;
                weaponAnimation = AttackSpeed;
                NPC.netUpdate = true;
            }
        }
    }
}