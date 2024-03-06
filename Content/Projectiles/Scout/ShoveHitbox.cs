using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace TF2.Content.Projectiles.Scout
{
    public class ShoveHitbox : TF2Projectile
    {
        public override string Texture => "TF2/Content/Projectiles/Demoman/ShieldHitbox";

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(250, 250);
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = TF2.Time(0.2);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.extraUpdates = 1;
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

        protected override void ProjectileAI() => Projectile.Size += new Vector2(25);

        public override bool ShouldUpdatePosition() => false;

        protected override void ProjectileHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            crit = false;
            // Modified from Terraria's source code
            float knockbackPower = 12.5f;
            if (target.hostile && target.whoAmI != Main.myPlayer)
            {
                if (modifiers.HitDirection < 0 && target.velocity.X > 0f - knockbackPower)
                {
                    if (target.velocity.X > 0f)
                    {
                        target.velocity.X -= knockbackPower;
                    }
                    target.velocity.X -= knockbackPower;
                    if (target.velocity.X < 0f - knockbackPower)
                    {
                        target.velocity.X = 0f - knockbackPower;
                    }
                }
                else if (modifiers.HitDirection > 0 && target.velocity.X < knockbackPower)
                {
                    if (target.velocity.X < 0f)
                    {
                        target.velocity.X += knockbackPower;
                    }
                    target.velocity.X += knockbackPower;
                    if (target.velocity.X > knockbackPower)
                    {
                        target.velocity.X = knockbackPower;
                    }
                }
                knockbackPower *= -0.5f;
                if (target.velocity.Y > knockbackPower)
                {
                    target.velocity.Y += knockbackPower;
                    if (target.velocity.Y < knockbackPower)
                    {
                        target.velocity.Y = knockbackPower;
                    }
                }
            }
        }

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableCrit();
            crit = miniCrit = false;
            // Modified from Terraria's source code
            float knockbackPower = 12.5f;
            if (!target.friendly && target.type != NPCID.TargetDummy)
            {
                if (modifiers.HitDirection < 0 && target.velocity.X > 0f - knockbackPower)
                {
                    if (target.velocity.X > 0f)
                    {
                        target.velocity.X -= knockbackPower;
                    }
                    target.velocity.X -= knockbackPower;
                    if (target.velocity.X < 0f - knockbackPower)
                    {
                        target.velocity.X = 0f - knockbackPower;
                    }
                }
                else if (modifiers.HitDirection > 0 && target.velocity.X < knockbackPower)
                {
                    if (target.velocity.X < 0f)
                    {
                        target.velocity.X += knockbackPower;
                    }
                    target.velocity.X += knockbackPower;
                    if (target.velocity.X > knockbackPower)
                    {
                        target.velocity.X = knockbackPower;
                    }
                }
                knockbackPower = target.noGravity ? (knockbackPower * -0.5f) : (knockbackPower * -0.75f);
                if (target.velocity.Y > knockbackPower)
                {
                    target.velocity.Y += knockbackPower;
                    if (target.velocity.Y < knockbackPower)
                    {
                        target.velocity.Y = knockbackPower;
                    }
                }
            }
        }
    }
}