using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Projectiles.Scout
{
    public class ShoveHitbox : ModProjectile
    {
        public override string Texture => "TF2/Content/Projectiles/Demoman/ShieldHitbox";

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(250); // This sets width and height to the same value (important when projectiles can rotate)
            Projectile.aiStyle = -1; // Use our own AI to customize how it behaves, if you don't want that, keep this at ProjAIStyleID.ShortSword. You would still need to use the code in SetVisualOffsets() though
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true; // Prevents hits through tiles. Most melee weapons that use projectiles have this
            Projectile.extraUpdates = 1; // Update 1+extraUpdates times per tick
            Projectile.timeLeft = 10;
            Projectile.hide = false; // Important when used alongside player.heldProj. "Hidden" projectiles have special draw conditions
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI() => Projectile.Size += new Vector2(25);

        public override bool ShouldUpdatePosition()
        {
            // Update Projectile.Center manually
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableCrit();
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
                knockbackPower = (target.noGravity ? (knockbackPower * -0.5f) : (knockbackPower * -0.75f));
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