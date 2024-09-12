using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items;
using TF2.Content.Items.Weapons.Scout;

namespace TF2.Content.Projectiles.Scout
{
    public class Baseball : TF2Projectile
    {
        public bool moonShot;
        private bool grounded;

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(20, 20);
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = TF2.Time(3);
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override bool ProjectilePreAI()
        {
            if (projectileInitialized) return true;
            noDistanceModifier = false;
            projectileInitialized = true;
            return true;
        }

        protected override void ProjectileAI()
        {
            if (!grounded)
                Projectile.timeLeft = TF2.Time(3);
            else
                Projectile.velocity.Y += 0.2f;
            if (Projectile.Hitbox.Intersects(Player.Hitbox) && Player.GetModPlayer<TF2Player>().currentClass == TF2Item.Scout && grounded && Timer >= TF2.Time(1))
            {
                for (int i = 0; i < Player.inventory.Length; i++)
                {
                    Item item = Player.inventory[i];
                    if (item.ModItem is Sandman weapon)
                        weapon.timer[0] = TF2.Time(10);
                }
                Projectile.Kill();
            }
            if (grounded)
                homing = false;
            SetRotation();
        }

        public override bool CanHitPlayer(Player target) => !grounded;

        public override bool? CanHitNPC(NPC target) => !grounded;

        protected override bool ProjectileTileCollide(Vector2 oldVelocity)
        {
            if (!grounded)
                Projectile.velocity = -oldVelocity * 0.1f;
            grounded = true;
            return false;
        }

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            SoundEngine.PlaySound(new SoundStyle(!moonShot ? "TF2/Content/Sounds/SFX/Weapons/sandman_stun" : "TF2/Content/Sounds/SFX/Weapons/sandman_stun_moonshot"), target.Center);
            int buffDuration = TF2.Round(Vector2.Distance(Player.Center, target.Center) / 1000f * TF2.Time(7));
            target.AddBuff(ModContent.BuffType<BaseballDebuff>(), !moonShot ? (buffDuration >= TF2.Time(1) ? buffDuration : 0) : TF2.Time(7), false);
            Projectile.velocity *= -0.1f;
            grounded = true;
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(new SoundStyle(!moonShot ? "TF2/Content/Sounds/SFX/Weapons/sandman_stun" : "TF2/Content/Sounds/SFX/Weapons/sandman_stun_moonshot"), target.Center);
            int buffDuration = TF2.Round(Vector2.Distance(Player.Center, target.Center) / 1000f * TF2.Time(7));
            target.AddBuff(ModContent.BuffType<BaseballDebuff>(), !moonShot ? (buffDuration >= TF2.Time(1) ? buffDuration : 0) : TF2.Time(7));
            Projectile.velocity *= -0.1f;
            grounded = true;
        }

        protected override void ProjectileSendExtraAI(BinaryWriter writer) => writer.Write(grounded);

        protected override void ProjectileReceiveExtraAI(BinaryReader binaryReader) => grounded = binaryReader.ReadBoolean();
    }
}