using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Projectiles.Soldier
{
    public class RocketJumperRocket : ModProjectile
    {
        public override string Texture => "TF2/Content/Projectiles/Soldier/Rocket";

        private bool prime;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Rocket");

        public override void SetDefaults()
        {
            Projectile.width = 32;                  // The width of projectile hitbox
            Projectile.height = 32;                 // The height of projectile hitbox
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
            Projectile.scale = 0.875f;              // Change projectile size
            Projectile.Name = "Rocket";
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.tileCollide = false;
            // Set to transparent. This projectile technically lives as transparent for about 3 frames
            Projectile.alpha = 255;
            // Change the hitbox size, centered about the original projectile center. This makes the projectile damage enemies during the explosion.
            Projectile.position = Projectile.Center;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.Center = Projectile.position;
            Projectile.timeLeft = 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (TF2.FindPlayer(Projectile, 50f))
            {
                oldVelocity *= 5f;
                oldVelocity.X = Utils.Clamp(oldVelocity.X, -25f, 25f);
                oldVelocity.Y = Utils.Clamp(oldVelocity.Y, -25f, 25f);
                Main.player[Projectile.owner].velocity -= oldVelocity;
            }
            prime = true;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void AI()
        {
            if (prime)
                Projectile.timeLeft = 0;
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft == 0)
            {
                Projectile.tileCollide = false;
                // Set to transparent. This projectile technically lives as transparent for about 3 frames
                Projectile.alpha = 255;
                // Change the hitbox size, centered about the original projectile center. This makes the projectile damage enemies during the explosion.
                Projectile.position = Projectile.Center;
                Projectile.width = 30;
                Projectile.height = 30;
                Projectile.Center = Projectile.position;
            }
            // Make sure to set the rotation accordingly to the velocity, and add some to work around the sprite's rotation
            // Please notice the MathHelper usage, offset the rotation by 112 degrees (to radians because rotation uses radians) because the sprite's rotation is not aligned!
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);
        }

        public override void Kill(int timeLeft) => Explode();

        private void Explode()
        {
            // Play explosion sound
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/rocket_jumper_explode1"), Projectile.Center);
            // Smoke Dust spawn
            for (int i = 0; i < 50; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 1.4f;
            }
            // Fire Dust spawn
            for (int i = 0; i < 80; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 5f;
                dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 3f;
            }
            /*
            // Large Smoke Gore spawn
            for (int g = 0; g < 2; g++)
            {
                var projectileSource = Projectile.GetSource_FromThis();
                int goreIndex = GoreID.Smoke1;
                Gore.NewGore(projectileSource, Projectile.position, new Vector2(Main.rand.Next(61, 64), Main.rand.Next(61, 64)), goreIndex);
                Main.gore[goreIndex].scale = 1.5f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                goreIndex = GoreID.Smoke2;
                Gore.NewGore(projectileSource, Projectile.position, new Vector2(Main.rand.Next(61, 64), Main.rand.Next(61, 64)), goreIndex);
                Main.gore[goreIndex].scale = 1.5f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                goreIndex = GoreID.Smoke3;
                Gore.NewGore(projectileSource, Projectile.position, new Vector2(Main.rand.Next(61, 64), Main.rand.Next(61, 64)), goreIndex);
                Main.gore[goreIndex].scale = 1.5f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
                goreIndex = GoreID.Smoke1;
                Gore.NewGore(projectileSource, Projectile.position, new Vector2(Main.rand.Next(61, 64), Main.rand.Next(61, 64)), goreIndex);
                Main.gore[goreIndex].scale = 1.5f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
            }
            */
            // reset size to normal width and height.
            Projectile.position.X = Projectile.position.X + Projectile.width / 2;
            Projectile.position.Y = Projectile.position.Y + Projectile.height / 2;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.position.X = Projectile.position.X - Projectile.width / 2;
            Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
        }
    }
}