using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Projectiles.Spy
{
    // Shortsword projectiles are handled in a special way with how they draw and damage things
    // The "hitbox" itself is closer to the player, the sprite is centered on it
    // However the interactions with the world will occur offset from this hitbox, closer to the sword's tip (CutTiles, Colliding)
    // Values chosen mostly correspond to Iron Shortword
    public class YourEternalRewardProjectile : ModProjectile
    {
        /*
        public const int FadeInDuration = 0;
        public const int FadeOutDuration = 4;
        */

        public const int TotalDuration = 48;

        // The "width" of the blade
        public float CollisionWidth => 10f * Projectile.scale;

        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Knife");
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(18); // This sets width and height to the same value (important when projectiles can rotate)
            Projectile.aiStyle = -1; // Use our own AI to customize how it behaves, if you don't want that, keep this at ProjAIStyleID.ShortSword. You would still need to use the code in SetVisualOffsets() though
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true; // Prevents hits through tiles. Most melee weapons that use projectiles have this
            Projectile.extraUpdates = 1; // Update 1+extraUpdates times per tick
            Projectile.timeLeft = 360; // This value does not matter since we manually kill it earlier, it just has to be higher than the duration we use in AI
            Projectile.hide = true; // Important when used alongside player.heldProj. "Hidden" projectiles have special draw conditions
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


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            TF2Player p = player.GetModPlayer<TF2Player>();
            Timer += 1;
            if (Timer >= TotalDuration)
            {
                // Kill the projectile if it reaches it's intented lifetime
                Projectile.Kill();
                p.backStab = false;
                return;
            }
            else
            {
                // Important so that the sprite draws "in" the player's hand and not fully infront or behind the player
                player.heldProj = Projectile.whoAmI;
                if (p.backStab)
                    player.GetModPlayer<TF2Player>().crit = true;
            }

            /*
            // Fade in and out
            // GetLerpValue returns a value between 0f and 1f - if clamped is true - representing how far Timer got along the "distance" defined by the first two parameters
            // The first call handles the fade in, the second one the fade out.
            // Notice the second call's parameters are swapped, this means the result will be reverted
            Projectile.Opacity = Utils.GetLerpValue(0f, FadeInDuration, Timer, clamped: true) * Utils.GetLerpValue(TotalDuration, TotalDuration - FadeOutDuration, Timer, clamped: true);
            */

            // Keep locked onto the player, but extend further based on the given velocity (Requires ShouldUpdatePosition returning false to work)
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
            if (Timer <= 8)
            {
                Projectile.Center = playerCenter + Projectile.velocity * 2f * (Timer - 1f);
            }
            else
            {
                Projectile.Center = playerCenter + Projectile.velocity * 16f;
            }

            // Set spriteDirection based on moving left or right. Left -1, right 1
            Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();

            // Point towards where it is moving, applied offset for top right of the sprite respecting spriteDirection
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;

            // The code in this method is important to align the sprite with the hitbox how we want it to
            SetVisualOffsets();
        }

        private void SetVisualOffsets()
        {
            // 32 is the sprite size (here both width and height equal)
            const int HalfSpriteWidth = 32 / 2;
            const int HalfSpriteHeight = 32 / 2;

            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;

            // Vanilla configuration for "hitbox in middle of sprite"
            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);
        }

        public override bool ShouldUpdatePosition() => false; // Update Projectile.Center manually

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // "Hit anything between the player and the tip of the sword"
            // shootSpeed is 2.1f for reference, so this is basically plotting 12 pixels ahead from the center
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * 6f;
            float collisionPoint = 0f; // Don't need that variable, but required as parameter
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.immune[Projectile.owner] = 48;

        public override void OnHitPvp(Player target, int damage, bool crit) => target.immuneTime = 48;

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => crit = false;

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit) => crit = false;
    }
}