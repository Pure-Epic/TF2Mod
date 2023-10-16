using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items;
using TF2.Content.Items.Demoman;

namespace TF2.Content.Projectiles.Demoman
{
    public class ShieldHitbox : ModProjectile
    {
        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public float CollisionWidth => 10f * Projectile.scale;

        public int totalDuration = 90;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(10); // This sets width and height to the same value (important when projectiles can rotate)
            Projectile.aiStyle = -1; // Use our own AI to customize how it behaves, if you don't want that, keep this at ProjAIStyleID.ShortSword. You would still need to use the code in SetVisualOffsets() though
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true; // Prevents hits through tiles. Most melee weapons that use projectiles have this
            Projectile.extraUpdates = 1; // Update 1+extraUpdates times per tick
            Projectile.timeLeft = 720; // This value does not matter since we manually kill it earlier, it just has to be higher than the duration we use in AI
            Projectile.hide = false; // Important when used alongside player.heldProj. "Hidden" projectiles have special draw conditions
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.GetModPlayer<ClaidheamhMorPlayer>().claidheamhMorInInventory)
                totalDuration = 120;
            ShieldPlayer shield = player.GetModPlayer<TF2Player>().shieldType switch
            {
                1 => player.GetModPlayer<CharginTargePlayer>(),
                _ => player.GetModPlayer<CharginTargePlayer>(),
            };
            shield.chargeLeft = totalDuration - Timer;
            shield.chargeDuration = totalDuration;
            TF2Weapon weapon = player.HeldItem.ModItem as TF2Weapon;
            if (player.controlUseItem && player.HeldItem.ModItem is TF2WeaponMelee && weapon.WeaponCanBeUsed(player))
            {
                shield.activateGracePeriod = true;
                shield.chargeActive = false;
                shield.chargeProjectileCreated = false;
                player.velocity = new Vector2(0f, 0f);
                Projectile.Kill();
                return;
            }

            Timer++;

            if (Timer >= totalDuration)
            {
                shield.chargeActive = false;
                shield.chargeProjectileCreated = false;
                player.ClearBuff(ModContent.BuffType<MeleeCrit>());
                player.velocity = new Vector2(0f, 0f);
                Projectile.Kill();
                return;
            }

            if (Timer >= 30)
                player.AddBuff(ModContent.BuffType<MeleeCrit>(), 600);

            Projectile.Center = player.Center;
            Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();
            Projectile.rotation = 0f;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * 6f;
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            player.immuneTime += 60;
            Timer = totalDuration;

            ShieldPlayer shield = player.GetModPlayer<TF2Player>().shieldType switch
            {
                1 => player.GetModPlayer<CharginTargePlayer>(),
                _ => player.GetModPlayer<CharginTargePlayer>(),
            };

            shield.activateGracePeriod = false;
            shield.buffDelay = 0;
            shield.chargeActive = false;
            shield.chargeProjectileCreated = false;
            player.ClearBuff(ModContent.BuffType<MeleeCrit>());
            player.velocity = new Vector2(0f, 0f);
            Projectile.Kill();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!info.PvP) return;
            Player player = Main.player[Projectile.owner];
            player.immuneTime += 60;
            Timer = totalDuration;

            ShieldPlayer shield = player.GetModPlayer<TF2Player>().shieldType switch
            {
                1 => player.GetModPlayer<CharginTargePlayer>(),
                _ => player.GetModPlayer<CharginTargePlayer>(),
            };

            shield.activateGracePeriod = false;
            shield.buffDelay = 0;
            shield.chargeActive = false;
            shield.chargeProjectileCreated = false;
            player.ClearBuff(ModContent.BuffType<MeleeCrit>());
            player.velocity = new Vector2(0f, 0f);
            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            ShieldPlayer shield = player.GetModPlayer<TF2Player>().shieldType switch
            {
                1 => player.GetModPlayer<CharginTargePlayer>(),
                _ => player.GetModPlayer<CharginTargePlayer>(),
            };

            shield.chargeLeft = 0;
            shield.chargeDuration = 90;
        }
    }
}