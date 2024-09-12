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
using TF2.Content.Items.Weapons.Soldier;
using TF2.Content.Projectiles.NPCs;

namespace TF2.Content.NPCs.Buddies
{
    public class SoldierNPC : MercenaryBuddy
    {
        protected override Asset<Texture2D> Spritesheet => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/SoldierNPC");

        protected override Asset<Texture2D> SpritesheetReverse => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/SoldierNPC_Reverse");

        public override int BaseHealth => 200;

        protected override float SpeedMuliplier => 0.8f;

        protected override int Weapon => ModContent.ItemType<RocketLauncher>();

        protected override int AttackSpeed => TF2.Time(0.8);

        protected override int ClipSize => 4;

        protected override int InitialReloadSpeed => TF2.Time(0.92);

        protected override int ReloadSpeed => TF2.Time(0.8);

        protected override string ReloadSound => "TF2/Content/Sounds/SFX/Weapons/rocket_reload";

        protected override float Range => 2000f;

        protected override void BuddyStatistics() => SetBuddyStatistics(90, "TF2/Content/Sounds/SFX/Voicelines/soldier_painsevere01", "TF2/Content/Sounds/SFX/Voicelines/soldier_paincriticaldeath01");

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("TF2.Bestiary.Soldier")),
            ]);

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
                float speed = 10f;
                int type = ModContent.ProjectileType<RocketNPC>();
                int damage = TF2.Round(NPC.damage / 2 * Player.GetModPlayer<TF2Player>().damageMultiplier);
                IEntitySource projectileSource = NPC.GetSource_FromAI();
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/rocket_shoot"), NPC.Center);
                BuddyShoot(projectileSource, NPC.Center, shootVel * speed, type, damage, 5f, Owner);
                Ammo--;
                AttackTimer = 0;
                weaponAnimation = AttackSpeed;
                NPC.netUpdate = true;
            }          
        }
    }
}