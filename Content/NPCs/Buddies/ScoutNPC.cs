using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Scout;
using TF2.Content.Projectiles;

namespace TF2.Content.NPCs.Buddies
{
    public class ScoutNPC : Buddy
    {
        protected override Asset<Texture2D> Spritesheet => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/ScoutNPC");

        protected override Asset<Texture2D> SpritesheetReverse => ModContent.Request<Texture2D>("TF2/Content/NPCs/Buddies/ScoutNPC_Reverse");

        public override int BaseHealth => 125;

        public override float BaseSpeed => 1.33f;

        protected override int ExtraJumps => 1;

        protected override int Weapon => ModContent.ItemType<Scattergun>();

        protected override int AttackSpeed => TF2.Time(0.6);

        protected override int ClipSize => 6;

        protected override int InitialReloadSpeed => TF2.Time(0.6);

        protected override int ReloadSpeed => TF2.Time(0.6);

        protected override string ReloadSound => "TF2/Content/Sounds/SFX/Weapons/scatter_gun_reload";

        protected override float Range => 250f;

        protected override void BuddyStatistics() => SetBuddyStatistics(6, "TF2/Content/Sounds/SFX/Voicelines/scout_painsevere01", "TF2/Content/Sounds/SFX/Voicelines/scout_paincriticaldeath01");

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
                int type = ModContent.ProjectileType<Bullet>();
                int damage = TF2.Round(NPC.damage / 2 * Player.GetModPlayer<TF2Player>().damageMultiplier);
                IEntitySource projectileSource = NPC.GetSource_FromAI();
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/scatter_gun_shoot"), NPC.Center);
                for (int i = 0; i < 10; i++)
                {
                    Vector2 newVelocity = shootVel.RotatedByRandom(MathHelper.ToRadians(12f));
                    BuddyShoot(projectileSource, NPC.Center, newVelocity * speed, type, damage, 0f, Owner);
                }
                Ammo--;
                AttackTimer = 0;
                weaponAnimation = AttackSpeed;
                NPC.netUpdate = true;
            }
        }
    }
}