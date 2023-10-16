using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items;
using TF2.Common;
using System;

namespace TF2.Content.Projectiles.Scout
{
    public class ForceANatureBullet : ModProjectile
    {
        public override string Texture => "TF2/Content/Projectiles/Bullet";
        public bool projectileInitialized;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bullet"); // The English name of the projectile
            ProjectileID.Sets.TrailingMode[Type] = 0; // Creates a trail behind the golf ball.
            ProjectileID.Sets.TrailCacheLength[Type] = 5; // Sets the length of the trail.
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BulletHighVelocity);
            Projectile.width = 50; // The width of projectile hitbox
            Projectile.height = 4; // The height of projectile hitbox
            Projectile.DamageType = ModContent.GetInstance<TF2DamageClass>();
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
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
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            Projectile.penetrate = p.pierce;
            projectileInitialized = true;
            return true;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            // Modified from Terraria's source code
            float knockbackPower = 10f;
            if (!target.friendly && target.boss == true)
            {
                if (hitDirection < 0 && target.velocity.X > 0f - knockbackPower)
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
                else if (hitDirection > 0 && target.velocity.X < knockbackPower)
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

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (TF2.FindPlayer(Projectile, 50f))
            {
                oldVelocity.X = Utils.Clamp(oldVelocity.X, -2f, 2f);
                oldVelocity.Y = Utils.Clamp(oldVelocity.Y, -2f, 2f);
                Main.player[Projectile.owner].velocity -= oldVelocity;
            }
            return true;
        }
    }
}