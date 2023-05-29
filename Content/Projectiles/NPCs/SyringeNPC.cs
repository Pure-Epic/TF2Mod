using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Projectiles.NPCs
{
    public class SyringeNPC : ModProjectile
    {
        public override string Texture => "TF2/Content/Projectiles/Medic/Syringe";

        public override void SetStaticDefaults() => DisplayName.SetDefault("Medic's Syringe"); // The English name of the projectile

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

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);

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
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (proj.Name == "Medic's Syringe")
            {
                Player.noKnockback = true;
                if (!p.equalizer)
                    Player.statLife += (int)(75 * p.classMultiplier);
                else
                    Player.statLife += (int)(7.5f * p.classMultiplier);
                damage = 0;
                crit = false;
            }
        }
    }
}
