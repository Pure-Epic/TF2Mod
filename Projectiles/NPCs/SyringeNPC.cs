using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using TF2.NPCs;

namespace TF2.Projectiles.NPCs
{
    public class SyringeNPC : ModProjectile
    {
        public float time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Medic's Syringe"); // The English name of the projectile
        }

        public override void SetDefaults()
        {
            Projectile.width = 10; // The width of projectile hitbox
            Projectile.height = 10; // The height of projectile hitbox
            Projectile.aiStyle = 1; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.penetrate = 1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.knockBack = 0f;
            Projectile.damage = 75;
            AIType = ProjectileID.WoodenArrowFriendly;
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
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);
        }

        public override bool? CanHitNPC(NPC target)
        {
            int medic = target.type;
            return target.type != medic;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Projectile.penetrate = 0;
            target.statLife += damage;
        }
    }

    public class SyringePlayer : ModPlayer
    {
        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            TFClass p = Player.GetModPlayer<TFClass>();
            if (proj.Name == "Medic's Syringe")
            {
                Player.noKnockback = true;
                Player.statLife += (int)(75 * p.classMultiplier);
                damage = 0;
                crit = false;
            }
        }
    }
}
