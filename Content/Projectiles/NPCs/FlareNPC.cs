using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Common;

namespace TF2.Content.Projectiles.NPCs
{
    public class FlareNPC : ModProjectile
    {
        public override string Texture => "TF2/Content/Projectiles/Pyro/Flare";

        public override void SetStaticDefaults() => DisplayName.SetDefault("Flare"); // The English name of the projectile

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
            Projectile.knockBack = 0f;
            Projectile.damage = 75;
            Projectile.scale = 0.75f;
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            PyroFlamesNPC npc = target.GetGlobalNPC<PyroFlamesNPC>();
            target.AddBuff(ModContent.BuffType<PyroFlames>(), 240, false);
            npc.damageMultiplier = p.classMultiplier;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.HasBuff(ModContent.BuffType<PyroFlames>()))
                Main.player[Projectile.owner].GetModPlayer<TF2Player>().crit = true;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);
    }
}