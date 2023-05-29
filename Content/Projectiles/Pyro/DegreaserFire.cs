using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Pyro
{
    public class DegreaserFire : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Flames;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Fire");

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 30;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
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
            if (Projectile.wet)
                Projectile.Kill();
            float dustScale = 1f;
            if (Projectile.ai[0] == 0f)
                dustScale = 0.25f;
            else if (Projectile.ai[0] == 1f)
                dustScale = 0.5f;
            else if (Projectile.ai[0] == 2f)
                dustScale = 0.75f;

            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);

                if (Main.rand.NextBool(3))
                {
                    dust.noGravity = true;
                    dust.scale *= 3f;
                    dust.velocity.X *= 2f;
                    dust.velocity.Y *= 2f;
                }

                dust.scale *= 1.5f;
                dust.velocity *= 1.2f;
                dust.scale *= dustScale;
            }
            Projectile.ai[0] += 1f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            PyroFlamesDegreaserNPC npc = target.GetGlobalNPC<PyroFlamesDegreaserNPC>();
            npc.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<PyroFlamesDegreaser>(), 600, false);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            PyroFlamesDegreaserPlayer burntPlayer = target.GetModPlayer<PyroFlamesDegreaserPlayer>();
            burntPlayer.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<PyroFlamesDegreaser>(), 600, false);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = 30;
            hitbox.X -= size;
            hitbox.Y -= size;
            hitbox.Width += size * 2;
            hitbox.Height += size * 2;
        }
    }
}