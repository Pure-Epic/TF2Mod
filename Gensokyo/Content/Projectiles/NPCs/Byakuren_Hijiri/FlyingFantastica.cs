using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Gensokyo.Content.Projectiles.NPCs.Byakuren_Hijiri
{
    [ExtendsFromMod("Gensokyo")]
    public class FlyingFantastica1 : ModProjectile
    {
        public bool projectileInitialized;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Flying Fantastica");

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.friendly = false;
            Projectile.hostile = true;
            AIType = ProjectileID.Bullet;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
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

        public override bool PreAI()
        {
            if (projectileInitialized) return true;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            projectileInitialized = true;
            return true;
        }

        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(0.2f);
            Projectile.position.Y += 2.5f;
            if (Projectile.timeLeft <= 30)
                Projectile.scale *= 0.875f;
        }
    }

    [ExtendsFromMod("Gensokyo")]
    public class FlyingFantastica2 : FlyingFantastica1 { }

    [ExtendsFromMod("Gensokyo")]
    public class FlyingFantastica3 : FlyingFantastica1 { }

    [ExtendsFromMod("Gensokyo")]
    public class FlyingFantastica4 : FlyingFantastica1 { }

    [ExtendsFromMod("Gensokyo")]
    public class FlyingFantastica5 : FlyingFantastica1 { }
}