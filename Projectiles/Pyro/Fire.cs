using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using TF2.Buffs;

namespace TF2.Projectiles.Pyro
{
    // The unique behaviors of Flamethrower projectiles are shown in this class.
    // Simply put, the projectile is actually not drawn and what the player sees is just dust spawning to give the look of a stream of flames.
    public class Fire : ModProjectile
    {
        // Since the texture is useless and not drawn, we can reuse the vanilla texture
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Flames;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flamethrower Projectile"); // The English name of the projectile
        }

        public override void SetDefaults()
        {
            Projectile.width = 6; // The width of projectile hitbox
            Projectile.height = 6; // The height of projectile hitbox
            Projectile.alpha = 255; // This makes the projectile invisible, only showing the dust.
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.penetrate = 3; // How many monsters the projectile can penetrate. Change this to make the flamethrower pierce more mobs.
            Projectile.timeLeft = 30; // A short life time for this projectile to get the flamethrower effect.
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            //projectile.ranged = true;
            Projectile.extraUpdates = 2;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void AI()
        {
            if (Projectile.wet)
            {
                Projectile.Kill(); //This kills the projectile when touching water. However, since our projectile is a cursed flame, we will comment this so that it won't run it. If you want to test this, feel free to uncomment this.
            }
            // Using a timer, we scale the earliest spawned dust smaller than the rest.
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

                // Some dust will be large, the others small and with gravity, to give visual variety.
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
            TFClass p = Main.player[Projectile.owner].GetModPlayer<TFClass>();
            PyroFlamesNPC npc = target.GetGlobalNPC<PyroFlamesNPC>();
            npc.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<PyroFlames>(), 600, false); //Gives pyro flames to target for 4 seconds. (60 = 1 second, 240 = 4 seconds)
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            TFClass p = Main.player[Projectile.owner].GetModPlayer<TFClass>();
            PyroFlamesPlayer burntPlayer = target.GetModPlayer<PyroFlamesPlayer>();
            burntPlayer.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<PyroFlames>(), 600, false);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            // By using ModifyDamageHitbox, we can allow the flames to damage enemies in a larger area than normal without colliding with tiles.
            // Here we adjust the damage hitbox. We adjust the normal 6x6 hitbox and make it 66x66 while moving it left and up to keep it centered.
            int size = 30;
            hitbox.X -= size;
            hitbox.Y -= size;
            hitbox.Width += size * 2;
            hitbox.Height += size * 2;
        }
    }
}
