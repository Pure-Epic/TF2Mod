using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Projectiles;
using TF2.Gensokyo.Content.NPCs.Byakuren_Hijiri;

namespace TF2.Gensokyo.Content.Projectiles.NPCs.Byakuren_Hijiri
{
    [ExtendsFromMod("Gensokyo")]
    public class MysticFragranceofaMakaiButterfly1 : ModProjectile
    {
        public int ProjectileAI
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private int Owner => Projectile.GetGlobalProjectile<TF2ProjectileBase>().owner;

        public bool projectileInitialized;
        public Vector2 center;
        private float speed;
        private int timer;
        private float distance;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Mystic Fragrance of a Makai Butterfly");

        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 68;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            speed = 2;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.Blue * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 1.1f, 0, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.White * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 0.8f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override bool PreAI()
        {
            if (Main.npc[Owner].ModNPC as ByakurenHijiri == null) return false;
            if (projectileInitialized) return true;
            distance = 25;
            projectileInitialized = true;
            return true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);
            ByakurenHijiri npc = Main.npc[Owner].ModNPC as ByakurenHijiri;

            if (npc.BossAI == 0 || !npc.NPC.active)
                Projectile.Kill();

            switch (ProjectileAI)
            {
                case 0:
                    Projectile.position.X = center.X - (int)(Math.Cos(MathHelper.ToRadians(timer * -speed)) * distance) - Projectile.width / 2;
                    Projectile.position.Y = center.Y - (int)(Math.Sin(MathHelper.ToRadians(timer * -speed)) * distance) - Projectile.height / 2;
                    break;
                case 1:
                    Projectile.position.X = center.X - (int)(Math.Cos(MathHelper.ToRadians(timer * speed)) * distance) - Projectile.width / 2;
                    Projectile.position.Y = center.Y - (int)(Math.Sin(MathHelper.ToRadians(timer * speed)) * distance) - Projectile.height / 2;
                    break;
                case 2:
                    Projectile.position.X = center.X - (int)(Math.Cos(MathHelper.ToRadians(timer * -speed)) * distance) - Projectile.width / 2;
                    Projectile.position.Y = center.Y - (int)(Math.Sin(MathHelper.ToRadians(timer * -speed)) * distance) - Projectile.height / 2;
                    break;
                case 3:
                    Projectile.position.X = center.X - (int)(Math.Cos(MathHelper.ToRadians(timer * speed)) * distance) - Projectile.width / 2;
                    Projectile.position.Y = center.Y - (int)(Math.Sin(MathHelper.ToRadians(timer * speed)) * distance) - Projectile.height / 2;
                    break;
                default:
                    break;
            }

            timer++;
            if (timer % 5 == 0)
            {
                Vector2 velocity = Vector2.UnitY;
                switch (ProjectileAI)
                {
                    case 0:
                        velocity = Utils.RotatedBy(Vector2.UnitY, MathHelper.ToRadians(timer + 180f)) * -5f;
                        break;
                    case 1:
                        velocity = Utils.RotatedBy(Vector2.UnitY, -MathHelper.ToRadians(timer + 180f)) * -5f;
                        break;
                    case 2:
                        velocity = Utils.RotatedBy(Vector2.UnitY, MathHelper.ToRadians(timer + 180f)) * -5f;
                        break;
                    case 3:
                        velocity = Utils.RotatedBy(Vector2.UnitY, -MathHelper.ToRadians(timer + 180f)) * -5f;
                        break;
                    default:
                        break;
                }
                SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
                int projectile = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<MysticFragranceofaMakaiButterfly2>(), 25, 0f, npc.NPC.target);
                MysticFragranceofaMakaiButterfly2 mysticFragranceofaMakaiButterfly2 = Main.projectile[projectile].ModProjectile as MysticFragranceofaMakaiButterfly2;
                mysticFragranceofaMakaiButterfly2.startTimer = 900 - timer;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile);
            }
        }
    }

    [ExtendsFromMod("Gensokyo")]
    public class MysticFragranceofaMakaiButterfly2 : ModProjectile
    {
        public bool projectileInitialized;
        public int startTimer;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Mystic Fragrance of a Makai Butterfly");

        public override void SetDefaults()
        {
            Projectile.width = 27;
            Projectile.height = 27;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
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

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.Blue * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 1.1f, 0, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.White * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 0.8f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override bool PreAI()
        {
            if (projectileInitialized) return true;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);
            Projectile.timeLeft += startTimer;
            projectileInitialized = true;
            return true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft <= 30)
                Projectile.scale *= 0.875f;
        }
    }

    [ExtendsFromMod("Gensokyo")]
    public class MysticFragranceofaMakaiButterfly3 : ModProjectile
    {
        private int timer;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mystic Fragrance of a Makai Butterfly");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 40000; // This makes lasers draw offscreen
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.scale = 0.1f;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.Opacity = 0.25f;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            float length = 5000f + Projectile.scale * 5f;
            Vector2 scale = new(Projectile.scale, 1f);
            DrawLaser(texture, Projectile.Center - Main.screenPosition, Projectile.Center + Projectile.velocity * length - Main.screenPosition, scale, 25, 24, 24);
            return false;
        }

        // Gensokyo mod laser drawing hook. It is based from Utils.DrawLaser()
        private void DrawLaser(Texture2D texture, Vector2 start, Vector2 end, Vector2 scale, int bodyLength, int headLength, int tailLength)
        {
            Vector2 vector = Vector2.Normalize(end - start);
            float length = (end - start).Length();
            float unitVector = Utils.ToRotation(vector) - MathHelper.ToRadians(90f);
            if (Utils.HasNaNs(vector)) return;
            Rectangle rectangle;

            // Draw the tail
            rectangle = new Rectangle(0, 0, texture.Width, tailLength);
            Vector2 vector2 = Utils.Size(rectangle) / 2f;
            Main.EntitySpriteDraw(texture, start, new Rectangle?(rectangle), Color.White * Projectile.Opacity, unitVector, Utils.Size(rectangle) / 2f, scale, 0, 0);

            // Draw the body
            float height = rectangle.Height;
            float maxHeight = height * scale.Y;
            Vector2 vector3 = start + vector * (rectangle.Height - vector2.Y) * scale.Y;
            while (maxHeight + 1f < length)
            {
                rectangle = new Rectangle(0, tailLength + 2, texture.Width, bodyLength);
                height = rectangle.Height;
                vector2 = new Vector2(rectangle.Width / 2f, 0f);
                if (length - maxHeight < rectangle.Height)
                {
                    height *= (length - maxHeight) / rectangle.Height;
                    rectangle.Height = (int)(length - maxHeight + 1f);
                }
                maxHeight += height * scale.Y;
                Main.EntitySpriteDraw(texture, vector3, new Rectangle?(rectangle), Color.White * Projectile.Opacity, unitVector, vector2, scale, 0, 0);
                vector3 += height * scale.Y * vector;
            }

            // Draw the head
            rectangle = new Rectangle(0, tailLength + bodyLength + 4, texture.Width, headLength);
            vector2 = new Vector2(rectangle.Width / 2f, 0f);
            Main.EntitySpriteDraw(texture, vector3, new Rectangle?(rectangle), Color.White * Projectile.Opacity, unitVector, vector2, scale, 0, 0);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = Projectile.velocity;
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + unit * 5000f, 16, ref point);
        }

        public override bool? CanDamage() => timer >= 30;

        public override void AI()
        {
            if (Utils.HasNaNs(Projectile.velocity) || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            timer++;
            if (timer >= 30 && timer <= 90)
            {
                Projectile.scale += 0.05f;
                Projectile.scale = Utils.Clamp(Projectile.scale, 0.1f, 0.5f);
                Projectile.Opacity += 0.05f;
                Projectile.Opacity = Utils.Clamp(Projectile.Opacity, 0.25f, 1f);
            }
            else if (timer > 90)
            {
                Projectile.scale -= 0.05f;
                Projectile.scale = Utils.Clamp(Projectile.scale, 0f, 0.5f);
                Projectile.Opacity -= 0.05f;
                Projectile.Opacity = Utils.Clamp(Projectile.Opacity, 0f, 1f);
            }
        }

        public override bool ShouldUpdatePosition() => false;
    }
}