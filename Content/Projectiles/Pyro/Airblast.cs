using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Projectiles.Pyro
{
    public class Airblast : ModProjectile
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Air blast");     // The English name of the projectile

        public override void SetDefaults()
        {
            Projectile.width = 25;               // The width of projectile hitbox
            Projectile.height = 25;              // The height of projectile hitbox
            Projectile.aiStyle = 1;              // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true;          // Can the projectile deal damage to enemies?
            Projectile.hostile = true;           // Can the projectile deal damage to the player?
            Projectile.timeLeft = 600;           // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 64;               // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f;             // How much light emit around the projectile
            Projectile.ignoreWater = false;      // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true;       // Can the projectile collide with tiles?
            Projectile.damage = 1;
            Projectile.knockBack = 20f;
            //projectile.scale = 5f;
            AIType = ProjectileID.Bullet;        // Act exactly like default Bullet
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool CanHitPlayer(Player player) => player != Main.player[Projectile.owner];

        public override void AI() => Projectile.damage = 1;

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

        public override void Kill(int timeLeft)
        {
            // This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
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
                Main.player[Projectile.owner].statLife += (int)(Main.player[Projectile.owner].statLifeMax2 * 0.11428571428);
            }
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
            Main.player[Projectile.owner].statLife += (int)(Main.player[Projectile.owner].statLifeMax2 * 0.11428571428);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            // Modified from Terraria's source code
            damage = 1;
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
    }
}