using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Projectiles.Sniper
{
    public class JarateProjectile : ModProjectile
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Jarate"); // The English name of the projectile

        public override void SetDefaults()
        {
            Projectile.width = 32; // The width of projectile hitbox
            Projectile.height = 32; // The height of projectile hitbox
            Projectile.aiStyle = 1; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
            AIType = ProjectileID.ToxicFlask;
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/jar_explode"), Main.player[Projectile.owner].Center);
            target.life += damage;
            if (target.friendly)
            {
                for (int i = 0; i < NPC.maxBuffs; i++)
                {
                    int buffTypes = target.buffType[i];
                    if (Main.debuff[buffTypes] && target.buffTime[i] > 0)
                    {
                        target.DelBuff(i);
                        i = -1;
                    }
                }
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    if (Main.player[Projectile.owner].buffType[i] == ModContent.BuffType<Buffs.JarateCooldown>())
                        Main.player[Projectile.owner].buffTime[i] -= 240;
                }
            }
            else
            {
                target.AddBuff(BuffID.Ichor, 600, true);
                target.AddBuff(ModContent.BuffType<Buffs.JarateDebuff>(), 600);
            }
            Projectile.Kill();
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.statLife += damage;
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                int buffTypes = target.buffType[i];
                if (Main.debuff[buffTypes] && target.buffTime[i] > 0 && !BuffID.Sets.NurseCannotRemoveDebuff[buffTypes] && !Buffs.TF2BuffBase.cooldownBuff[buffTypes])
                {
                    target.DelBuff(i);
                    i = -1;
                }
            }
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (Main.player[Projectile.owner].buffType[i] == ModContent.BuffType<Buffs.MadMilkCooldown>() && Main.player[Projectile.owner] != target)
                    Main.player[Projectile.owner].buffTime[i] -= 240;
            }
        }

        public override void Kill(int timeLeft)
        {
            // Projectile.Size = new Vector2(100);
            for (int i = 0; i < 25; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.IchorTorch, 0f, 0f, 100, default, 3f);
                Main.dust[dustIndex].velocity *= 5f;
                dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.IchorTorch, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 3f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = new Vector2(0, 0);
            Projectile.tileCollide = false;
            // Set to transparent. This projectile technically lives as transparent for about 3 frames
            Projectile.alpha = 255;
            // Change the hitbox size, centered about the original projectile center. This makes the projectile damage enemies during the explosion.
            Projectile.position = Projectile.Center;
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.Center = Projectile.position;
            Projectile.hostile = true;
            Projectile.timeLeft = 0;
            return false;
        }

        public override void AI() => Projectile.damage = 1;
    }
}