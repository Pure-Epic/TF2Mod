using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Spy
{
    public class KnifeProjectile : TF2Projectile
    {
        public override string Texture => "TF2/Content/Items/Weapons/Spy/Knife";

        public float CollisionWidth => 10f * Projectile.scale;

        public int totalDuration = TF2.Time(0.8);

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(18, 18);
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = TF2.Time(6);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override void ProjectileAI()
        {
            if (Timer >= totalDuration)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Player.heldProj = Projectile.whoAmI;
                if (Player.GetModPlayer<TF2Player>().backStab)
                    backStab = true;
            }
            Vector2 playerCenter = Player.RotatedRelativePoint(Player.MountedCenter, reverseRotation: false, addGfxOffY: false);
            if (Timer <= 8)
                Projectile.Center = playerCenter + Projectile.velocity * 2f * (Timer - 1f);
            else
                Projectile.Center = playerCenter + Projectile.velocity * 16f;
            Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;
            SetVisualOffsets();
        }

        private void SetVisualOffsets()
        {
            const int HalfSpriteWidth = 32 / 2;
            const int HalfSpriteHeight = 32 / 2;
            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;
            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * 6f;
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
        }

        protected override void ProjectileHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (!modifiers.PvP || !backStab) return;
            target.KillMe(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[7].Format(target.name, Player.name)), target.statLife, 0);
        }

        protected override void ProjectileDestroy(int timeLeft) => Player.GetModPlayer<TF2Player>().backStab = false;
    }

    public class YourEternalRewardProjectile : KnifeProjectile
    {
        public override string Texture => "TF2/Content/Items/Weapons/Spy/YourEternalReward";
    }

    public class ConniversKunaiProjectile : KnifeProjectile
    {
        public override string Texture => "TF2/Content/Items/Weapons/Spy/ConniversKunai";

        protected override void ProjectileHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (!modifiers.PvP) return;
            TF2.Overheal(Player, target.statLife, 2f);
            target.KillMe(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[7].Format(target.name, Player.name)), target.statLife, 0);
        }

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.type != NPCID.TargetDummy)
                TF2.Overheal(Player, Utils.Clamp(TF2.Round((float)((float)target.life / target.lifeMax * TF2.GetHealth(Player, 210))), TF2.GetHealth(Player, 75), TF2.GetHealth(Player, 210)), 2f);
        }
    }

    public class BigEarnerProjectile : KnifeProjectile
    {
        public override string Texture => "TF2/Content/Items/Weapons/Spy/BigEarner";

        protected override void ProjectileHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (!modifiers.PvP || !backStab) return;
            Player.AddBuff(ModContent.BuffType<BigEarnerBuff>(), TF2.Time(3));
            target.KillMe(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[7].Format(target.name, Player.name)), target.statLife, 0);
        }

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.type != NPCID.TargetDummy && backStab)
                Player.AddBuff(ModContent.BuffType<BigEarnerBuff>(), TF2.Time(3));
        }
    }
}