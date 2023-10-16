using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Projectiles.NPCs
{
    public class SentryRocket : ModProjectile
    {
        public override string Texture => "TF2/Content/Projectiles/Soldier/Rocket";

        public override void SetDefaults()
        {
            Projectile.width = 40;              // The width of projectile hitbox
            Projectile.height = 12;             // The height of projectile hitbox
            Projectile.aiStyle = 1;             // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true;         // Can the projectile deal damage to enemies?
            Projectile.hostile = false;         // Can the projectile deal damage to the player?
            Projectile.penetrate = 1;           // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600;          // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true;      // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true;      // Can the projectile collide with tiles?
            Projectile.extraUpdates = 1;        // Set to above 0 if you want the projectile to update multiple time in a frame
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool PreDraw(ref Color lightColor) => TF2.DrawProjectile(Projectile, ref lightColor);

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.timeLeft == 0)
            {
                Projectile.tileCollide = false;
                // Set to transparent. This projectile technically lives as transparent for about 3 frames
                Projectile.alpha = 255;
                // Change the hitbox size, centered about the original projectile center. This makes the projectile damage enemies during the explosion.
                Projectile.position = Projectile.Center;
                Projectile.width = 100;
                Projectile.height = 100;
                Projectile.Center = Projectile.position;
                Projectile.hostile = true;
                Projectile.damage = 100;
                Projectile.knockBack = 10f;
            }
        }

        public override void OnKill(int timeLeft) => TF2.Explode(Projectile, new SoundStyle("TF2/Content/Sounds/SFX/explode1"), 3);
    }
}