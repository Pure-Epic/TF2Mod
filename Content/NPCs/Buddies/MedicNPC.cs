using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Consumables;
using TF2.Content.Items.Weapons.Medic;
using TF2.Content.Projectiles.Medic;

namespace TF2.Content.NPCs.Buddies
{
    public class MedicNPC : MercenaryBuddy
    {
        protected override Asset<Texture2D> Spritesheet => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/MedicNPC");

        protected override Asset<Texture2D> SpritesheetReverse => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/MedicNPC_Reverse");

        public override int BaseHealth => 150;

        protected override float SpeedMuliplier => 1.07f;

        protected override int Weapon => ModContent.ItemType<SyringeGun>();

        protected override int AttackSpeed => TF2.Time(0.105);

        protected override int ClipSize => 40;

        protected override int ReloadSpeed => TF2.Time(1.305);

        protected override string ReloadSound => "TF2/Content/Sounds/SFX/Weapons/syringegun_reload";

        protected override bool MagazineReload => true;

        protected override float Range => 1500f;

        public override float MaxDamageMultiplier => 1.2f;

        public override float DamageFalloffRange => 700f;

        private int regenerationTimer;
        private int healTimer;

        protected override void BuddyStatistics() => SetBuddyStatistics(10, "TF2/Content/Sounds/SFX/Voicelines/medic_painsevere01", "TF2/Content/Sounds/SFX/Voicelines/medic_paincriticaldeath01");

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("TF2.Bestiary.Medic")),
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
                float speed = 25f;
                int type = ModContent.ProjectileType<Syringe>();
                int damage = TF2.Round(NPC.damage / 2 * Player.GetModPlayer<TF2Player>().damageMultiplier);
                IEntitySource projectileSource = NPC.GetSource_FromAI();
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/syringegun_shoot"), NPC.Center);
                BuddyShoot(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, Owner);
                Ammo--;
                AttackTimer = 0;
                weaponAnimation = AttackSpeed;
                NPC.netUpdate = true;
            }
        }

        protected override void BuddyUpdate()
        {
            regenerationTimer++;
            if (regenerationTimer >= TF2.Time(1) && NPC.life < NPC.lifeMax)
            {
                int health = TF2.Round(NPC.lifeMax / 50f);
                if ((health + NPC.life) > NPC.lifeMax)
                    health = NPC.lifeMax - NPC.life;
                NPC.life += health;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.HealEffect(health);
                regenerationTimer = 0;
                NPC.netUpdate = true;
            }
            healTimer++;
            if (healTimer >= TF2.Time(30))
            {
                IEntitySource ammoSource = NPC.GetSource_FromAI();
                int loot = Item.NewItem(ammoSource, NPC.Center, ModContent.ItemType<SmallHealthPoint>());
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncItem, number: loot);
                healTimer = 0;
                NPC.netUpdate = true;
            }
            NPC.netUpdate = true;
        }

        protected override void BuddySendExtraAI(BinaryWriter writer)
        {
            writer.Write(regenerationTimer);
            writer.Write(healTimer);
        }

        protected override void BuddyReceiveExtraAI(BinaryReader binaryReader)
        {
            regenerationTimer = binaryReader.ReadInt32();
            healTimer = binaryReader.ReadInt32();
        }
    }
}