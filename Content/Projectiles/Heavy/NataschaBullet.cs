using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items;
using TF2.Common;

namespace TF2.Content.Projectiles.Heavy
{
    public class NataschaBullet : ModProjectile
    {
        public override string Texture => "TF2/Content/Projectiles/Bullet";
        private bool projectileInitialized;

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

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!target.friendly)
            {
                target.AddBuff(ModContent.BuffType<Buffs.NataschaDebuff>(), 1);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Slow, 1);
        }
    }
}