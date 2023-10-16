using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Projectiles;
using TF2.Gensokyo.Content.NPCs.Byakuren_Hijiri;

namespace TF2.Gensokyo.Content.Projectiles.NPCs.Byakuren_Hijiri
{
    [ExtendsFromMod("Gensokyo")]
    public class OmenofPurpleClouds1 : ModProjectile
    {
        public int ProjectileAI
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private int Owner => Projectile.GetGlobalProjectile<TF2ProjectileBase>().owner;

        public bool projectileInitialized;
        private float speed;
        private int timer;
        private float distance;
        private int direction;

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
            speed = 5;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.White * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 1.1f, 0, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.White * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 0.8f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override bool PreAI()
        {
            if ((ByakurenHijiri)Main.npc[Owner].ModNPC == null) return false;
            if (projectileInitialized) return true;
            distance = 250;
            projectileInitialized = true;
            return true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            ByakurenHijiri npc = (ByakurenHijiri)Main.npc[Owner].ModNPC;

            if (npc.State == 0 || !npc.NPC.active)
                Projectile.Kill();

            switch (ProjectileAI)
            {
                case 0:
                    Projectile.position.X = npc.NPC.Center.X - (int)(Math.Cos(MathHelper.ToRadians(timer * speed)) * distance) - Projectile.width / 2;
                    Projectile.position.Y = npc.NPC.Center.Y - (int)(Math.Sin(MathHelper.ToRadians(timer * speed)) * distance) - Projectile.height / 2;
                    break;

                case 1:
                    Projectile.position.X = npc.NPC.Center.X - (int)(Math.Cos(MathHelper.ToRadians(timer * -speed + 180f)) * distance) - Projectile.width / 2;
                    Projectile.position.Y = npc.NPC.Center.Y - (int)(Math.Sin(MathHelper.ToRadians(timer * -speed + 180f)) * distance) - Projectile.height / 2;
                    break;

                default:
                    break;
            }

            timer++;
            distance -= 5f;
            distance = Utils.Clamp(distance, -25, 250);

            if (timer % 5 == 0)
            {
                Vector2 velocity = Vector2.UnitY;
                switch (ProjectileAI)
                {
                    case 0:
                        velocity = Utils.RotatedBy(Vector2.UnitY, MathHelper.ToRadians(timer + 90)) * -5f;
                        direction = 1;
                        break;

                    case 1:
                        velocity = Utils.RotatedBy(Vector2.UnitY, MathHelper.ToRadians(timer + 270)) * -5f;
                        direction = -1;
                        break;

                    default:
                        break;
                }
                if (Main.netMode == NetmodeID.MultiplayerClient) return;
                int projectile = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<OmenofPurpleClouds2>(), 30, 0f, npc.NPC.target);
                OmenofPurpleClouds2 omenofPurpleClouds2 = (OmenofPurpleClouds2)Main.projectile[projectile].ModProjectile;
                omenofPurpleClouds2.startTimer = 240 - timer;
                omenofPurpleClouds2.direction = direction;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile);
            }
            Projectile.netUpdate = true;
        }
    }

    [ExtendsFromMod("Gensokyo")]
    public class OmenofPurpleClouds2 : ModProjectile
    {
        public bool projectileInitialized;
        public int timer;
        public int startTimer;
        public int direction;

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 17;
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
            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
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
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.timeLeft += startTimer;
            projectileInitialized = true;
            return true;
        }

        public override void AI()
        {
            timer++;
            if (timer >= 60)
                Projectile.velocity = Utils.RotatedBy(Projectile.velocity, Projectile.ai[1] * MathHelper.ToRadians(15f * direction), default);
            if (Projectile.timeLeft <= 30)
                Projectile.scale *= 0.875f;
        }
    }

    [ExtendsFromMod("Gensokyo")]
    public class OmenofPurpleClouds3 : ModProjectile
    {
        public int ProjectileAI
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private int Owner => Projectile.GetGlobalProjectile<TF2ProjectileBase>().owner;

        public bool projectileInitialized;
        private float speed;
        private int timer;
        private float distance;

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
            speed = 5;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.Magenta * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 1.1f, 0, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.White * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 0.8f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override bool PreAI()
        {
            if ((ByakurenHijiri)Main.npc[Owner].ModNPC == null) return false;
            if (projectileInitialized) return true;
            distance = 150;
            projectileInitialized = true;
            return true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            ByakurenHijiri npc = (ByakurenHijiri)Main.npc[Owner].ModNPC;

            if (npc.State == 0 || !npc.NPC.active)
                Projectile.Kill();

            switch (ProjectileAI)
            {
                case 0:
                    Projectile.position.X = npc.NPC.Center.X - (int)(Math.Cos(MathHelper.ToRadians(timer * speed + 90f)) * distance) - Projectile.width / 2;
                    Projectile.position.Y = npc.NPC.Center.Y - (int)(Math.Sin(MathHelper.ToRadians(timer * speed + 90f)) * distance) - Projectile.height / 2;
                    break;

                case 1:
                    Projectile.position.X = npc.NPC.Center.X - (int)(Math.Cos(MathHelper.ToRadians(timer * -speed + 270f)) * distance) - Projectile.width / 2;
                    Projectile.position.Y = npc.NPC.Center.Y - (int)(Math.Sin(MathHelper.ToRadians(timer * -speed + 270f)) * distance) - Projectile.height / 2;
                    break;

                default:
                    break;
            }

            timer++;
            distance -= 3f;
            distance = Utils.Clamp(distance, -15, 150);

            if (timer % 5 == 0)
            {
                Vector2 velocity = Vector2.UnitY;
                int direction = 1;
                switch (ProjectileAI)
                {
                    case 0:
                        velocity = Utils.RotatedBy(Vector2.UnitY, MathHelper.ToRadians(timer)) * -5.25f;
                        direction = 1;
                        break;

                    case 1:
                        velocity = Utils.RotatedBy(Vector2.UnitY, MathHelper.ToRadians(timer + 180f)) * -5.25f;
                        direction = -1;
                        break;

                    default:
                        break;
                }
                if (Main.netMode == NetmodeID.MultiplayerClient) return;
                int projectile = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<OmenofPurpleClouds4>(), 30, 0f, npc.NPC.target);
                OmenofPurpleClouds4 omenofPurpleClouds4 = (OmenofPurpleClouds4)Main.projectile[projectile].ModProjectile;
                omenofPurpleClouds4.startTimer = 240 - timer;
                omenofPurpleClouds4.direction = direction;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectile);
            }
            Projectile.netUpdate = true;
        }
    }

    [ExtendsFromMod("Gensokyo")]
    public class OmenofPurpleClouds4 : ModProjectile
    {
        public bool projectileInitialized;
        public int timer;
        public int startTimer;
        public int direction;

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
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.Magenta * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 1.1f, 0, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.White * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 0.8f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override bool PreAI()
        {
            if (projectileInitialized) return true;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.timeLeft += startTimer;
            projectileInitialized = true;
            return true;
        }

        public override void AI()
        {
            timer++;
            if (timer >= 60)
                Projectile.velocity = Utils.RotatedBy(Projectile.velocity, Projectile.ai[1] * MathHelper.ToRadians(15f * direction), default);
            if (Projectile.timeLeft <= 30)
                Projectile.scale *= 0.875f;
            Projectile.netUpdate = true;
        }
    }
}