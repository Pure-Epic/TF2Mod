using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Projectiles.Spy
{
    public class KnifeProjectile : ModProjectile
    {
        public override string Texture => "TF2/Content/Items/Spy/Knife";

        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public float CollisionWidth => 10f * Projectile.scale;

        public const int TotalDuration = 48;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(18);
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 360;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool PreDraw(ref Color lightColor) => TF2.DrawProjectile(Projectile, ref lightColor);

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            TF2Player p = player.GetModPlayer<TF2Player>();
            Timer++;
            if (Timer >= TotalDuration)
            {
                Projectile.Kill();
                p.backStab = false;
                return;
            }
            else
            {
                player.heldProj = Projectile.whoAmI;
                if (p.backStab)
                    player.GetModPlayer<TF2Player>().crit = true;
            }

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.immune[Projectile.owner] = 48;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!info.PvP) return;
            target.immuneTime = 48;
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (!modifiers.PvP) return;
            Main.player[Projectile.owner].GetModPlayer<TF2Player>().crit = false;
        }
    }

    public class YourEternalRewardProjectile : KnifeProjectile
    {
        public override string Texture => "TF2/Content/Items/Spy/YourEternalReward";
    }

    public class ConniversKunaiProjectile : KnifeProjectile
    {
        public override string Texture => "TF2/Content/Items/Spy/ConniversKunai";

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            player.Heal((int)((float)target.life / target.lifeMax * player.statLifeMax2));
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (!modifiers.PvP) return;
            Player player = Main.player[Projectile.owner];
            player.Heal(target.statLife);
            player.GetModPlayer<TF2Player>().crit = false;
        }
    }
}