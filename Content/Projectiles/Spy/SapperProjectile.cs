using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Common;

namespace TF2.Content.Projectiles.Spy
{
    public class SapperProjectile : ModProjectile
    {
        public bool StickOnEnemy
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }

        public int TargetWhoAmI
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public int GravityDelayTimer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public float StickTimer
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        public override void SetStaticDefaults() => DisplayName.SetDefault("Sapper");

        public override void SetDefaults()
        {
            Projectile.width = 32;                  // The width of projectile hitbox
            Projectile.height = 32;                 // The height of projectile hitbox
            Projectile.aiStyle = 0;                 // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true;             // Can the projectile deal damage to enemies?
            Projectile.hostile = false;             // Can the projectile deal damage to the player?
            Projectile.penetrate = 2;               // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600;              // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 255;                 // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.ignoreWater = true;          // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true;          // Can the projectile collide with tiles?
            Projectile.hide = true;                 // Makes the projectile completely invisible. We need this to draw our projectile behind enemies/tiles in DrawBehind()
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        private const int GravityDelay = 45;

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
            UpdateAlpha();
            // Run either the sapping AI or passive AI
            // Separating into different methods helps keeps your AI clean
            if (!StickOnEnemy)
                StartingAI();
            else
                SapAI();
        }

        private void StartingAI()
        {
            GravityDelayTimer++; // doesn't make sense.

            // For a little while, the javelin will travel with the same speed, but after this, the javelin drops velocity very quickly.
            if (GravityDelayTimer >= GravityDelay)
            {
                GravityDelayTimer = GravityDelay;

                // Wind resistance
                Projectile.velocity.X *= 0.98f;
                // Gravity
                Projectile.velocity.Y += 0.12f;
            }

            // Offset the rotation by 90 degrees because the sprite is oriented vertiacally.
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);
        }

        private const int stickTime = 600;

        private void SapAI()
        {
            Projectile.ignoreWater = true; // Make sure the projectile ignores water
            Projectile.tileCollide = false; // Make sure the projectile doesn't collide with tiles anymore
            StickTimer += 1f;

            int npcTarget = TargetWhoAmI;
            if (StickTimer >= stickTime || npcTarget < 0 || npcTarget >= 200)
                Projectile.Kill(); // If the index is past its limits, kill it
            else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage)
            {
                // If the target is active and can take damage
                // Set the projectile's position relative to the target's center
                Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
                Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;

                // The damage and stun comes from the debuff, not the projectile
            }
            else
                Projectile.Kill(); // Otherwise, kill the projectile
        }

        public override void Kill(int timeLeft)
        {
            if (StickTimer < stickTime) return;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position); // Play a death sound
            Vector2 usePos = Projectile.position; // Position to use for dusts

            Vector2 rotationVector = (Projectile.rotation - MathHelper.ToRadians(0f)).ToRotationVector2(); // rotation vector to use for dust velocity
            usePos += rotationVector * 16f;

            // Spawn some dusts upon javelin death
            for (int i = 0; i < 20; i++)
            {
                // Create a new dust
                Dust dust = Dust.NewDustDirect(usePos, Projectile.width, Projectile.height, DustID.Electric);
                dust.position = (dust.position + Projectile.Center) / 2f;
                dust.velocity += rotationVector * 2f;
                dust.velocity *= 0.5f;
                dust.noGravity = true;
                usePos -= rotationVector * 8f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            SappedPlayer sappedPlayer = target.GetModPlayer<SappedPlayer>();
            sappedPlayer.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<Sapped>(), 600, false);
        }

        private const int maxSappers = 1; // This is the max. amount of sappers being able to attach
        private readonly Point[] activeSappers = new Point[maxSappers]; // The point array holding for sticking sappers


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            SappedNPC npc = target.GetGlobalNPC<SappedNPC>();
            npc.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<Sapped>(), 600, false);
            StickOnEnemy = true;
            TargetWhoAmI = target.whoAmI; // Set the target whoAmI
            Projectile.netUpdate = true;
            Projectile.damage = 0;
            Projectile.KillOldestJavelin(Projectile.whoAmI, Type, target.whoAmI, activeSappers);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            // If attached to an NPC, draw behind tiles (and the npc) if that NPC is behind tiles, otherwise just behind the NPC.
            if (StickOnEnemy)
            {
                int npcIndex = TargetWhoAmI;
                if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
                {
                    if (Main.npc[npcIndex].behindTiles)
                        behindNPCsAndTiles.Add(index);
                    else
                        behindNPCsAndTiles.Add(index);
                    return;
                }
            }
            // Since we aren't attached, add to this list
            behindNPCsAndTiles.Add(index);
        }

        // Change this number if you want to alter how the alpha changes
        private const int AlphaFadeInSpeed = 25;

        private void UpdateAlpha()
        {
            // Slowly remove alpha as it is present
            if (Projectile.alpha > 0)
                Projectile.alpha -= AlphaFadeInSpeed;

            // If alpha gets lower than 0, set it to 0
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
        }
    }
}