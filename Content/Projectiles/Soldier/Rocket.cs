using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TF2.Content.Projectiles.Soldier
{
    public class Rocket : ModProjectile
    {
        public bool prime;

        public override void SetDefaults()
        {
            Projectile.width = 40;                  // The width of projectile hitbox
            Projectile.height = 12;                 // The height of projectile hitbox
            Projectile.aiStyle = 0;                 // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true;             // Can the projectile deal damage to enemies?
            Projectile.hostile = false;             // Can the projectile deal damage to the player?
            Projectile.penetrate = -1;              // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 360;              // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0;                   // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0f;                  // How much light emit around the projectile
            Projectile.ignoreWater = true;          // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true;          // Can the projectile collide with tiles?
            Projectile.extraUpdates = 1;            // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => prime = true;

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => prime = true;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            prime = true;
            return false;
        }

        public override bool PreDraw(ref Color lightColor) => TF2.DrawProjectile(Projectile, ref lightColor);

        public override void AI()
        {
            if (prime)
                Projectile.timeLeft = 0;
            if (Projectile.timeLeft == 0)
            {
                Projectile.tileCollide = false;
                Projectile.alpha = 255;
                Projectile.position = Projectile.Center;
                Projectile.width = 100;
                Projectile.height = 100;
                Projectile.Center = Projectile.position;
                RocketJump(Projectile.velocity);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft) => TF2.Explode(Projectile, new SoundStyle("TF2/Content/Sounds/SFX/explode1"), 3);

        public virtual void RocketJump(Vector2 velocity)
        {
            if (TF2.FindPlayer(Projectile, 50f))
            {
                velocity *= 5f;
                velocity.X = Utils.Clamp(velocity.X, -25f, 25f);
                velocity.Y = Utils.Clamp(velocity.Y, -25f, 25f);
                Player player = Main.player[Projectile.owner];
                player.velocity -= velocity;
                if (player.immuneNoBlink) return;
                int selfDamage = Convert.ToInt32(Math.Floor(player.statLifeMax2 * 0.15f));
                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " blew themself to smithereens."), selfDamage, 0);
            }
        }
    }
}