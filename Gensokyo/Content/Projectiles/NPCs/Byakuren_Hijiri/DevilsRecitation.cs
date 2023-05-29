using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Projectiles;
using TF2.Gensokyo.Content.NPCs.Byakuren_Hijiri;

namespace TF2.Gensokyo.Content.Projectiles.NPCs.Byakuren_Hijiri
{
    [ExtendsFromMod("Gensokyo")]
    public class DevilsRecitation1 : ModProjectile
    {
        public int ProjectileAI
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private int Owner => Projectile.GetGlobalProjectile<TF2ProjectileBase>().owner;

        public bool projectileInitialized;
        private int timer;
        private Vector2 velocity = Vector2.Zero;
        private int pulseTimer;
        private bool maxPulse;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devil's Recitation");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 40000; // This makes lasers draw offscreen
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.scale = 0.1f;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
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

        public override bool? CanDamage() => timer >= 120;

        public override bool PreAI()
        {
            if (projectileInitialized) return true;
            NPC npc = Main.npc[Owner];
            if (npc == null) return false;
            switch (ProjectileAI)
            {
                case 0:
                    velocity = new Vector2(-2f, -2f);
                    break;
                case 1:
                    velocity = new Vector2(2f, -2f);
                    break;
                case 2:
                    velocity = new Vector2(-5f, 3.75f);
                    break;
                case 3:
                    velocity = new Vector2(5f, 3.75f);
                    break;
                default:
                    break;
            }
            projectileInitialized = true;
            return true;
        }

        public override void AI()
        {
            ByakurenHijiri npc = Main.npc[Owner].ModNPC as ByakurenHijiri;

            if (Utils.HasNaNs(Projectile.velocity) || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            if (!npc.NPC.active)
                Projectile.Kill();

            timer++;
            if (timer < 120)
            {
                Projectile.position += velocity;
                if (pulseTimer >= 10)
                    maxPulse = true;
                else if (pulseTimer <= 0)
                    maxPulse = false;
                if (pulseTimer < 10 && !maxPulse)
                {
                    pulseTimer++;
                    Projectile.scale += 0.025f;
                    Projectile.scale = Utils.Clamp(Projectile.scale, 0.1f, 0.5f);
                    Projectile.Opacity += 0.025f;
                    Projectile.Opacity = Utils.Clamp(Projectile.Opacity, 0.25f, 1f);
                }
                else if (maxPulse)
                {
                    pulseTimer--;
                    Projectile.scale -= 0.025f;
                    Projectile.scale = Utils.Clamp(Projectile.scale, 0f, 0.5f);
                    Projectile.Opacity -= 0.025f;
                    Projectile.Opacity = Utils.Clamp(Projectile.Opacity, 0f, 1f);
                }
            }
            else if (timer >= 120 && timer <= 210)
            {
                Projectile.position.Y -= 5f;
                Projectile.scale += 0.1f;
                Projectile.scale = Utils.Clamp(Projectile.scale, 0.1f, 2.5f);
                Projectile.Opacity += 0.1f;
                Projectile.Opacity = Utils.Clamp(Projectile.Opacity, 0.25f, 1f);
            }
            else if (timer > 210)
            {
                Projectile.scale -= 0.05f;
                Projectile.scale = Utils.Clamp(Projectile.scale, 0f, 0.5f);
                Projectile.Opacity -= 0.05f;
                Projectile.Opacity = Utils.Clamp(Projectile.Opacity, 0f, 1f);
            }
        }

        public override bool ShouldUpdatePosition() => false;
    }

    [ExtendsFromMod("Gensokyo")]
    public class DevilsRecitation2 : ModProjectile
    {
        public bool projectileInitialized;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Devil's Recitation");

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
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
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);
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
    public class DevilsRecitation3 : ModProjectile
    {
        public bool projectileInitialized;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Devil's Recitation");

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1000;
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
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);
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
    public class DevilsRecitation4 : ModProjectile
    {
        public bool projectileInitialized;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Devil's Recitation");

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1000;
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
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.Coral * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 1.1f, 0, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.White * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 0.8f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override bool PreAI()
        {
            if (projectileInitialized) return true;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);
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
    public class DevilsRecitation5 : ModProjectile
    {
        public bool projectileInitialized;
        public int timer;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Devil's Recitation");

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1000;
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
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.Purple * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 1.1f, 0, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.White * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 0.8f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override bool PreAI()
        {
            if (projectileInitialized) return true;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);
            projectileInitialized = true;
            return true;
        }

        public override void AI()
        {
            timer++;

            if (timer >= 120)
                Projectile.velocity = Vector2.UnitY * 10f;
            if (Projectile.timeLeft <= 30)
                Projectile.scale *= 0.875f;
        }
    }

    [ExtendsFromMod("Gensokyo")]
    public class DevilsRecitation6 : ModProjectile
    {
        public override string Texture => "TF2/Gensokyo/Content/Projectiles/NPCs/Byakuren_Hijiri/DevilsRecitation1";

        public int ProjectileAI
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private int Owner => Projectile.GetGlobalProjectile<TF2ProjectileBase>().owner;

        public bool projectileInitialized;
        private int timer;
        private float pulseTimer;
        private bool maxPulse;
        private int direction;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devil's Recitation");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 40000; // This makes lasers draw offscreen
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.scale = 0.1f;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 760;
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

        public override bool? CanDamage() => timer >= 120;

        public override bool PreAI()
        {
            if (projectileInitialized) return true;
            NPC npc = Main.npc[Owner];
            if (npc == null) return false;
            switch (ProjectileAI)
            {
                case 0:
                    direction = 1;
                    break;
                case 1:
                    direction = -1;
                    break;
                case 2:
                    direction = -1;
                    break;
                case 3:
                    direction = 1;
                    break;
                default:
                    break;
            }
            projectileInitialized = true;
            return true;
        }

        public override void AI()
        {
            ByakurenHijiri npc = Main.npc[Owner].ModNPC as ByakurenHijiri;

            if (Utils.HasNaNs(Projectile.velocity) || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            if (!npc.NPC.active)
                Projectile.Kill();

            timer++;
            if (timer < 120)
            {
                if (pulseTimer >= 10f)
                    maxPulse = true;
                else if (pulseTimer <= 0f)
                    maxPulse = false;
                if (pulseTimer < 10 && !maxPulse)
                {
                    pulseTimer++;
                    Projectile.scale += 0.025f;
                    Projectile.scale = Utils.Clamp(Projectile.scale, 0.1f, 0.5f);
                    Projectile.Opacity += 0.025f;
                    Projectile.Opacity = Utils.Clamp(Projectile.Opacity, 0.25f, 1f);
                }
                else if (maxPulse)
                {
                    pulseTimer--;
                    Projectile.scale -= 0.025f;
                    Projectile.scale = Utils.Clamp(Projectile.scale, 0f, 0.5f);
                    Projectile.Opacity -= 0.025f;
                    Projectile.Opacity = Utils.Clamp(Projectile.Opacity, 0f, 1f);
                }
            }
            else if (timer >= 120 && timer <= 640)
            {
                Projectile.scale += 0.1f;
                Projectile.scale = Utils.Clamp(Projectile.scale, 0.1f, 2.5f);
                Projectile.Opacity += 0.1f;
                Projectile.Opacity = Utils.Clamp(Projectile.Opacity, 0.25f, 1f);

                if (pulseTimer >= 2.5f)
                    maxPulse = true;
                else if (pulseTimer <= -2.5f)
                    maxPulse = false;
                if (pulseTimer < 2.5f && !maxPulse)
                {
                    pulseTimer += 0.1f;
                    Projectile.velocity = Utils.RotatedBy(Vector2.UnitY, MathHelper.ToRadians(pulseTimer * direction));
                }
                else if (maxPulse)
                {
                    pulseTimer -= 0.1f;
                    Projectile.velocity = Utils.RotatedBy(Vector2.UnitY, MathHelper.ToRadians(pulseTimer * direction));
                }
            }
        }

        public override bool ShouldUpdatePosition() => false;
    }

    [ExtendsFromMod("Gensokyo")]
    public class DevilsRecitation7 : ModProjectile
    {
        public override string Texture => "TF2/Gensokyo/Content/Projectiles/NPCs/Byakuren_Hijiri/DevilsRecitation5";

        public bool projectileInitialized;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Devil's Recitation");

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1000;
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
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.Purple * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 1.1f, 0, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.White * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 0.8f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override bool PreAI()
        {
            if (projectileInitialized) return true;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);
            projectileInitialized = true;
            return true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft <= 30)
                Projectile.scale *= 0.875f;
        }
    }
}