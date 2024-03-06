using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Gensokyo.Content.NPCs.Byakuren_Hijiri;

namespace TF2.Gensokyo.Content.Projectiles.NPCs.Byakuren_Hijiri
{
    [ExtendsFromMod("Gensokyo")]
    public class AmagimiHijirisAirScroll : ModProjectile
    {
        private int Owner => Projectile.owner;

        public bool projectileInitialized;
        public int timer;
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
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.Blue * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 1.1f, 0, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(texture.Bounds), Color.White * Projectile.Opacity, Projectile.rotation, Utils.Size(texture) * 0.5f, Projectile.scale * 0.8f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override bool PreAI()
        {
            if (projectileInitialized) return true;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if ((ByakurenHijiri)Main.npc[Owner].ModNPC == null) return false;
            projectileInitialized = true;
            return true;
        }

        public override void AI()
        {
            timer++;
            if (timer >= 120)
            {
                ByakurenHijiri npc = (ByakurenHijiri)Main.npc[Owner].ModNPC;
                Vector2 vector = npc.targetCenter - Projectile.Center;
                Projectile.velocity = Vector2.Normalize(vector) * 10f;
            }
            if (Projectile.timeLeft <= 30)
                Projectile.scale *= 0.875f;
            Projectile.netUpdate = true;
        }
    }
}