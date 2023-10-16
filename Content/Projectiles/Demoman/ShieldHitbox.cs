using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TF2.Content.Projectiles.Demoman
{
    // Shortsword projectiles are handled in a special way with how they draw and damage things
    // The "hitbox" itself is closer to the player, the sprite is centered on it
    // However the interactions with the world will occur offset from this hitbox, closer to the sword's tip (CutTiles, Colliding)
    // Values chosen mostly correspond to Iron Shortword
    public class ShieldHitbox : ModProjectile
    {
        /*
        public const int FadeInDuration = 0;
        public const int FadeOutDuration = 4;
        */

        public const int TotalDuration = 120;

        // The "size" of the bash
        public float CollisionWidth => 10f * Projectile.scale;

        public float CollisionHeight => 10f * Projectile.scale;

        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public bool activateGracePeriod;
        public int buffDelay;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Shield Bash");

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(10); // This sets width and height to the same value (important when projectiles can rotate)
            Projectile.aiStyle = -1; // Use our own AI to customize how it behaves, if you don't want that, keep this at ProjAIStyleID.ShortSword. You would still need to use the code in SetVisualOffsets() though
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true; // Prevents hits through tiles. Most melee weapons that use projectiles have this
            Projectile.extraUpdates = 1; // Update 1+extraUpdates times per tick
            Projectile.timeLeft = 720; // This value does not matter since we manually kill it earlier, it just has to be higher than the duration we use in AI
            Projectile.hide = false; // Important when used alongside player.heldProj. "Hidden" projectiles have special draw conditions
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.controlUseItem && player.HeldItem.ModItem is Items.TF2WeaponMelee)
            {
                player.GetModPlayer<Items.Demoman.CharginTargePlayer>().activateGracePeriod = true;
                player.GetModPlayer<Items.Demoman.ShieldPlayer>().chargeActive = false;
                player.GetModPlayer<Items.Demoman.ShieldPlayer>().chargeProjectileCreated = false;
                player.velocity = new Vector2(0f, 0f);
                Projectile.Kill();
                return;
            }

            if (!activateGracePeriod) { Timer += 1; }

            if (Timer >= TotalDuration)
            {
                // Kill the projectile if it reaches it's intented lifetime
                player.GetModPlayer<Items.Demoman.ShieldPlayer>().chargeActive = false;
                player.GetModPlayer<Items.Demoman.ShieldPlayer>().chargeProjectileCreated = false;
                player.ClearBuff(ModContent.BuffType<Buffs.MeleeCrit>());
                player.velocity = new Vector2(0f, 0f);
                Projectile.Kill();
                return;
            }

            if (Timer >= 30 && !activateGracePeriod)
            {
                player.AddBuff(ModContent.BuffType<Buffs.MeleeCrit>(), 600);
            }
            /*
            // Fade in and out
            // GetLerpValue returns a value between 0f and 1f - if clamped is true - representing how far Timer got along the "distance" defined by the first two parameters
            // The first call handles the fade in, the second one the fade out.
            // Notice the second call's parameters are swapped, this means the result will be reverted
            Projectile.Opacity = Utils.GetLerpValue(0f, FadeInDuration, Timer, clamped: true) * Utils.GetLerpValue(TotalDuration, TotalDuration - FadeOutDuration, Timer, clamped: true);
            */

            // Make the player lock to the projectile.
            Projectile.Center = player.Center;

            // Set spriteDirection based on moving left or right. Left -1, right 1
            Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();

            Projectile.rotation = 0f;
        }

        public override bool ShouldUpdatePosition()
        {
            // Update Projectile.Center manually
            return false;
        }

        /*
		public override void CutTiles() {
			// "cutting tiles" refers to breaking pots, grass, queen bee larva, etc.
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Vector2 start = Projectile.Center;
			Vector2 end = start + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 10f;
			Utils.PlotTileLine(start, end, CollisionWidth, DelegateMethods.CutTiles);
		}
		*/

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (activateGracePeriod) return false;
            // "Hit anything between the player and the tip of the sword"
            // shootSpeed is 2.1f for reference, so this is basically plotting 12 pixels ahead from the center
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * 6f;
            float collisionPoint = 0f; // Don't need that variable, but required as parameter
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[Projectile.owner].immuneTime += 60;
            Timer = TotalDuration;
            Main.player[Projectile.owner].GetModPlayer<Items.Demoman.ShieldPlayer>().chargeActive = false;
            Main.player[Projectile.owner].GetModPlayer<Items.Demoman.ShieldPlayer>().chargeProjectileCreated = false;
            Main.player[Projectile.owner].ClearBuff(ModContent.BuffType<Buffs.MeleeCrit>());
            Main.player[Projectile.owner].velocity = new Vector2(0f, 0f);
            Projectile.Kill();
            return;
        }
    }
}