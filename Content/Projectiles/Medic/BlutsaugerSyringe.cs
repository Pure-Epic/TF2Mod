using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Projectiles.Medic
{
    public class BlutsaugerSyringe : ModProjectile
    {
        public override string Texture => "TF2/Content/Projectiles/Medic/Syringe";
        public bool projectileInitialized;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Syringe"); // The English name of the projectile

        public override void SetDefaults()
        {
            Projectile.width = 10; // The width of projectile hitbox
            Projectile.height = 10; // The height of projectile hitbox
            Projectile.aiStyle = 1; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.penetrate = 1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
            AIType = ProjectileID.WoodenArrowFriendly;
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

        public override bool PreAI()
        {
            if (projectileInitialized) return true;
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            Projectile.penetrate = p.pierce;
            projectileInitialized = true;
            return true;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!(Main.player[Projectile.owner].statLife >= Main.player[Projectile.owner].statLifeMax2) && target.type != NPCID.TargetDummy)
            {
                int healingAmount = (int)(Main.player[Projectile.owner].statLifeMax2 * 0.02f);
                Main.player[Projectile.owner].statLife += healingAmount;
                Main.player[Projectile.owner].HealEffect(healingAmount, true);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (!(Main.player[Projectile.owner].statLife >= Main.player[Projectile.owner].statLifeMax2))
            {
                int healingAmount = (int)(Main.player[Projectile.owner].statLifeMax2 * 0.02f);
                Main.player[Projectile.owner].statLife += healingAmount;
                Main.player[Projectile.owner].HealEffect(healingAmount, true);
            }
        }
    }
}