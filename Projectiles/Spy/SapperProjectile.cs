using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.GameContent.Achievements;
using Terraria.GameContent;
using TF2.Buffs;

namespace TF2.Projectiles.Spy
{
    public class SapperProjectile : ModProjectile
    {
        // this code is actually useless
        public bool stickOnEnemy = false;
        //{
			//get => Projectile.ai[0] == 1f;
			//set => Projectile.ai[0] = value? 1f : 0f;
		//}

        public int targetWhoAmI
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public int timeLimit = 120;
        public override void SetDefaults()
        {
            Projectile.width = 32;                   //The width of projectile hitbox
            Projectile.height = 32;                  //The height of projectile hitbox
            Projectile.aiStyle = 1;                 //The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true;             //Can the projectile deal damage to enemies?
            Projectile.hostile = false;             //Can the projectile deal damage to the player?
            Projectile.penetrate = 1;              //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600;             //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0;                   //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0f;                  //How much light emit around the projectile
            Projectile.ignoreWater = true;          //Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true;          //Can the projectile collide with tiles?
            Projectile.extraUpdates = 1;            //Set to above 0 if you want the projectile to update multiple time in a frame
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

        public override bool PreAI()
        {
            return true;
        }
        public override void AI()
        {
            if (!stickOnEnemy)
            {
                StartingAI();
            }
            else
            {
                SapAI();
            }

            // Make sure to set the rotation accordingly to the velocity, and add some to work around the sprite's rotation
            // Please notice the MathHelper usage, offset the rotation by 90 degrees (to radians because rotation uses radians) because the sprite's rotation is not aligned!
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);

        }

        private void StartingAI()
        {
            Projectile.timeLeft = 600;
        }

        private void SapAI()
        {
            // These 2 could probably be moved to the ModifyNPCHit hook, but in vanilla they are present in the AI
            Projectile.ignoreWater = true; // Make sure the projectile ignores water
            Projectile.tileCollide = false; // Make sure the projectile doesn't collide with tiles anymore
            const int aiFactor = 5; // Change this factor to change the 'lifetime' of this sticking sapper
            Projectile.localAI[0] += 1f;

            // Every 60 ticks, the sapper will perform a hit effect
            bool hitEffect = Projectile.localAI[0] % 60f == 0f;
            int projTargetIndex = (int)targetWhoAmI;
            if (Projectile.localAI[0] >= 60 * aiFactor || projTargetIndex < 0 || projTargetIndex >= 200)
            { // If the index is past its limits, kill it
                Projectile.Kill();
            }
            else if (Main.npc[projTargetIndex].active && !Main.npc[projTargetIndex].dontTakeDamage)
            { // If the target is active and can take damage
              // Set the projectile's position relative to the target's center
                Projectile.Center = Main.npc[projTargetIndex].Center - Projectile.velocity * 2f;
                Projectile.gfxOffY = Main.npc[projTargetIndex].gfxOffY;
                if (hitEffect)
                { // Perform a hit effect here
                    Main.npc[projTargetIndex].velocity = new Vector2(0f, 0f);
                    Main.npc[projTargetIndex].HitEffect(0, 1.0);
                }
            }
            else
            { // Otherwise, kill the projectile
                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {

        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            TFClass p = Main.player[Projectile.owner].GetModPlayer<TFClass>();
            SappedPlayer sappedPlayer = target.GetModPlayer<SappedPlayer>();
            sappedPlayer.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<Sapped>(), 600, false);
        }

        private const int maxSappers = 1; // This is the max. amount of sappers being able to attach
        private readonly Point[] _activeSappers = new Point[maxSappers]; // The point array holding for sticking sappers

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            TFClass p = Main.player[Projectile.owner].GetModPlayer<TFClass>();
            SappedNPC npc = target.GetGlobalNPC<SappedNPC>();
            npc.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<Sapped>(), 600, false);
            stickOnEnemy = true;
            targetWhoAmI = target.whoAmI; // Set the target whoAmI
            Projectile.velocity = (target.Center - Projectile.Center) * 0.25f; // Change velocity based on delta center of targets (difference between entity centers)
            Projectile.netUpdate = true;
            UpdateActiveSappers(target);
        }

        private void UpdateActiveSappers(NPC target)
        {
            int currentSapperIndex = 0; // The sapper index

            for (int i = 0; i < Main.maxProjectiles; i++) // Loop all projectiles
            {
                Projectile currentProjectile = Main.projectile[i];
                if (i != Projectile.whoAmI // Make sure the looped projectile is not the current sapper
                    && currentProjectile.active // Make sure the projectile is active
                    && currentProjectile.owner == Main.myPlayer // Make sure the projectile's owner is the client's player
                    && currentProjectile.type == Projectile.type // Make sure the projectile is of the same type as this sapper
                    && currentProjectile.ModProjectile is SapperProjectile sapperProjectile // Use a pattern match cast so we can access the projectile like an ExamplesapperProjectile
                    && sapperProjectile.stickOnEnemy // the previous pattern match allows us to use our properties
                    && sapperProjectile.targetWhoAmI == target.whoAmI)
                {

                    _activeSappers[currentSapperIndex++] = new Point(i, currentProjectile.timeLeft); // Add the current projectile's index and timeleft to the point array
                    if (currentSapperIndex >= _activeSappers.Length)  // If the sapper's index is bigger than or equal to the point array's length, break
                        break;
                }
            }

            // Remove the oldest sapper if we exceeded the maximum
            if (currentSapperIndex >= maxSappers)
            {
                int oldSapperIndex = 0;
                // Loop our point array
                for (int i = 1; i < maxSappers; i++)
                {
                    // Remove the already existing sapper if it's timeLeft value (which is the Y value in our point array) is smaller than the new sapper's timeLeft
                    if (_activeSappers[i].Y < _activeSappers[oldSapperIndex].Y)
                    {
                        oldSapperIndex = i; // Remember the index of the removed sapper
                    }
                }
                // Remember that the X value in our point array was equal to the index of that sapper, so it's used here to kill it.
                Main.projectile[_activeSappers[oldSapperIndex].X].Kill();
            }
        }
    }
}