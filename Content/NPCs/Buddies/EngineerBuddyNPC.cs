using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.Items.Consumables;
using TF2.Content.Items.Weapons.MultiClass;
using TF2.Content.NPCs.Buildings;
using TF2.Content.Projectiles;

namespace TF2.Content.NPCs.Buddies
{
    public class EngineerBuddyNPC : Buddy
    {
        protected override Asset<Texture2D> Spritesheet => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/EngineerBuddyNPC");

        protected override Asset<Texture2D> SpritesheetReverse => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/EngineerBuddyNPC_Reverse");

        public override int BaseHealth => 125;

        protected override int Weapon => ModContent.ItemType<Pistol>();

        protected override int AttackSpeed => TF2.Time(0.15);

        protected override int ClipSize => 12;

        protected override int ReloadSpeed => TF2.Time(1.035);

        protected override string ReloadSound => "TF2/Content/Sounds/SFX/Weapons/pistol_reload";

        protected override bool MagazineReload => true;

        protected override float Range => 1500f;

        private int spreadRecovery;
        protected int metalTimer;

        protected override void BuddyStatistics() => SetBuddyStatistics(12, "TF2/Content/Sounds/SFX/Voicelines/engineer_painsevere01", "TF2/Content/Sounds/SFX/Voicelines/engineer_paincriticaldeath01");

        protected override void BuddyAttack(NPC target)
        {
            if (Reloading || Falling) return;
            AttackTimer++;
            if (AttackTimer >= AttackSpeed && Ammo > 0)
            {
                NPC.velocity.X = 0f;
                Vector2 shootVel = NPC.DirectionTo(target.Center);
                weaponRotation = NPC.AngleTo(target.Center);
                NPC.spriteDirection = NPC.direction = (weaponRotation >= -MathHelper.PiOver2 && weaponRotation <= MathHelper.PiOver2) ? 1 : -1;
                float speed = 10f;
                int type = ModContent.ProjectileType<Bullet>();
                int damage = TF2.Round(NPC.damage / 2 * Player.GetModPlayer<TF2Player>().damageMultiplier);
                IEntitySource projectileSource = NPC.GetSource_FromAI();
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/pistol_shoot"), NPC.Center);
                Vector2 newVelocity = spreadRecovery >= TF2.Time(1.25) ? shootVel : shootVel.RotatedByRandom(MathHelper.ToRadians(2.5f));
                BuddyShoot(projectileSource, NPC.Center, newVelocity * speed, type, damage, 0f, Owner);
                spreadRecovery = 0;
                Ammo--;
                AttackTimer = 0;
                weaponAnimation = AttackSpeed;
                NPC.netUpdate = true;
            }
        }

        protected override void BuddyUpdate()
        {
            spreadRecovery++;
            metalTimer++;
            if (metalTimer >= TF2.Time(30))
            {
                if (!FindSentry())
                {
                    ConstructBuilding();
                    metalTimer = 0;
                    NPC.netUpdate = true;
                }
                else if (Player.GetModPlayer<TF2Player>().currentClass == TF2Item.Engineer)
                {
                    IEntitySource ammoSource = NPC.GetSource_FromAI();
                    int loot = Item.NewItem(ammoSource, NPC.Center, ModContent.ItemType<MediumAmmoPoint>());
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.SyncItem, number: loot);
                    metalTimer = 0;
                    NPC.netUpdate = true;
                }
            }
            NPC.netUpdate = true;
        }

        private void ConstructBuilding()
        {
            NPC npc = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<EngineerBuddySentry>(), 0, 0, 0, 0, Owner);
            TF2.EngineerBuddyConstructSentryGunMultiplayer(this, npc);
            npc.Bottom = NPC.Bottom;
            npc.direction = NPC.direction;
            Building building = npc.ModNPC as Building;
            if (!Main.dedServ)
                building.air = focus;
            else
                building.ConstuctBuildingMultiplayer(focus);
        }

        private bool FindSentry()
        {
            bool found = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].ModNPC is EngineerBuddySentry npc && npc.Owner == Owner)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        protected override void BuddySendExtraAI(BinaryWriter writer)
        {
            writer.Write(spreadRecovery);
            writer.Write(metalTimer);
        }

        protected override void BuddyReceiveExtraAI(BinaryReader binaryReader)
        {
            spreadRecovery = binaryReader.ReadInt32();
            metalTimer = binaryReader.ReadInt32();
        }
    }
}