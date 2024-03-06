using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items;
using TF2.Content.Items.Weapons;
using TF2.Content.Items.Weapons.Demoman;

namespace TF2.Content.Projectiles.Demoman
{
    public class ShieldHitbox : TF2Projectile
    {
        public float CollisionWidth => 10f * Projectile.scale;

        public int totalDuration = 90;

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(10, 10);
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = TF2.Time(12);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override void ProjectileAI()
        {
            if (Player.GetModPlayer<ClaidheamhMorPlayer>().claidheamhMorInInventory)
                totalDuration = TF2.Time(2);
            ShieldPlayer shield = Player.GetModPlayer<TF2Player>().shieldType switch
            {
                1 => Player.GetModPlayer<CharginTargePlayer>(),
                _ => Player.GetModPlayer<CharginTargePlayer>(),
            };
            shield.chargeLeft = totalDuration - Timer;
            shield.chargeDuration = totalDuration;
            TF2Weapon weapon = Player.HeldItem.ModItem as TF2Weapon;
            if (Player.controlUseItem && weapon.IsWeaponType(TF2Item.Melee) && weapon.WeaponCanBeUsed(Player))
            {
                shield.activateGracePeriod = true;
                shield.chargeActive = false;
                shield.chargeProjectileCreated = false;
                Player.velocity = new Vector2(0f, 0f);
                Projectile.Kill();
                return;
            }
            if (Timer >= totalDuration)
            {
                shield.chargeActive = false;
                shield.chargeProjectileCreated = false;
                Player.ClearBuff(ModContent.BuffType<MeleeCrit>());
                Player.velocity = new Vector2(0f, 0f);
                Projectile.Kill();
                return;
            }
            if (Timer >= 30)
                Player.AddBuff(ModContent.BuffType<MeleeCrit>(), TF2.Time(10));
            Projectile.Center = Player.Center;
            Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * 6f;
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
        }

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!info.PvP) return;
            Player.immuneTime += 60;
            Timer = totalDuration;

            ShieldPlayer shield = Player.GetModPlayer<TF2Player>().shieldType switch
            {
                1 => Player.GetModPlayer<CharginTargePlayer>(),
                _ => Player.GetModPlayer<CharginTargePlayer>(),
            };

            shield.activateGracePeriod = false;
            shield.buffDelay = 0;
            shield.chargeActive = false;
            shield.chargeProjectileCreated = false;
            Player.ClearBuff(ModContent.BuffType<MeleeCrit>());
            Player.velocity = new Vector2(0f, 0f);
            Projectile.Kill();
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player.immuneTime += 60;
            Timer = totalDuration;

            ShieldPlayer shield = Player.GetModPlayer<TF2Player>().shieldType switch
            {
                1 => Player.GetModPlayer<CharginTargePlayer>(),
                _ => Player.GetModPlayer<CharginTargePlayer>(),
            };

            shield.activateGracePeriod = false;
            shield.buffDelay = 0;
            shield.chargeActive = false;
            shield.chargeProjectileCreated = false;
            Player.ClearBuff(ModContent.BuffType<MeleeCrit>());
            Player.velocity = new Vector2(0f, 0f);
            Projectile.Kill();
        }

        protected override void ProjectileDestroy(int timeLeft)
        {
            ShieldPlayer shield = Player.GetModPlayer<TF2Player>().shieldType switch
            {
                1 => Player.GetModPlayer<CharginTargePlayer>(),
                _ => Player.GetModPlayer<CharginTargePlayer>(),
            };

            shield.chargeLeft = 0;
            shield.chargeDuration = TF2.Time(1.5);
        }
    }
}