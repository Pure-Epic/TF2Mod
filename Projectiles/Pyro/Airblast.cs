using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace TF2.Projectiles.Pyro
{
    public class Airblast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Air blast");     //The English name of the projectile
        }

        public override void SetDefaults()
        {
            Projectile.width = 25;               //The width of projectile hitbox
            Projectile.height = 25;              //The height of projectile hitbox
            Projectile.aiStyle = 1;              //The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true;          //Can the projectile deal damage to enemies?
            Projectile.hostile = false;          //Can the projectile deal damage to the player?
            Projectile.timeLeft = 600;           //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 64;               //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f;             //How much light emit around the projectile
            Projectile.ignoreWater = false;      //Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true;       //Can the projectile collide with tiles?
            Projectile.damage = 1;
            Projectile.knockBack = 20f;
            //projectile.scale = 5f;
            AIType = ProjectileID.Bullet;        //Act exactly like default Bullet
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
        public override void Kill(int timeLeft)
        {
            // This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!target.friendly)
            {
                target.life += damage;
                target.velocity = new Vector2(-target.velocity.X, -target.velocity.Y);
            }
        }
    }
}
