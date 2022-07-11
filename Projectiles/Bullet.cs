using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Items;

namespace TF2.Projectiles
{
	public class Bullet : ModProjectile
	{
		//public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BulletHighVelocity;

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
			Projectile.ArmorPenetration = 10000;
			Projectile.friendly = true;
			Projectile.hostile = false;
			AIType = ProjectileID.BulletHighVelocity;
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

		public override void AI()
		{
			TFClass p = Main.player[Projectile.owner].GetModPlayer<TFClass>();
			Projectile.penetrate = p.pierce;
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);
		}
	}
}