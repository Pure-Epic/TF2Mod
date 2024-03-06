using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Gensokyo.Content.NPCs.Byakuren_Hijiri;

namespace TF2.Gensokyo.Content.Projectiles.NPCs.Byakuren_Hijiri
{
    [ExtendsFromMod("Gensokyo")]
    public class StarMaelstrom1 : ModProjectile
    {
        public int ProjectileAI
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private int Owner => Projectile.owner;

        public Vector2 center;
        public float angleOffset;
        private float speed;
        private int timer;
        public float distance;

        public override void SetDefaults()
        {
            Projectile.width = 175;
            Projectile.height = 10;
            Projectile.scale = 1f;
            Projectile.Opacity = 0.25f;
            DrawOffsetX = -37;
            DrawOriginOffsetY = -9;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            speed = 1;
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
            if ((ByakurenHijiri)Main.npc[Owner].ModNPC == null) return false;
            return true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(timer * speed + angleOffset + 90);
            ByakurenHijiri npc = (ByakurenHijiri)Main.npc[Owner].ModNPC;

            if (npc.Stage == 0 || !npc.NPC.active)
                Projectile.Kill();

            Projectile.position.X = center.X - (int)(Math.Cos(MathHelper.ToRadians(timer * speed + angleOffset)) * distance) - Projectile.width / 2;
            Projectile.position.Y = center.Y - (int)(Math.Sin(MathHelper.ToRadians(timer * speed + angleOffset)) * distance) - Projectile.height / 2;

            timer++;

            if (timer >= 45 && timer <= 985)
            {
                Projectile.Opacity += 0.05f;
                Projectile.Opacity = Utils.Clamp(Projectile.Opacity, 0.25f, 1f);
            }
            else if (timer > 985)
            {
                Projectile.Opacity -= 0.05f;
                Projectile.Opacity = Utils.Clamp(Projectile.Opacity, 0f, 1f);
            }
            Projectile.netUpdate = true;
        }

        public override bool? CanDamage() => timer >= 45;
    }

    [ExtendsFromMod("Gensokyo")]
    public class StarMaelstrom2 : ModProjectile
    {
        public bool projectileInitialized;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 15;
            Projectile.scale = 2.5f;
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
            Projectile.rotation = Projectile.velocity.ToRotation();
            projectileInitialized = true;
            return true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft <= 30)
                Projectile.scale *= 0.875f;
            Projectile.netUpdate = true;
        }
    }
}