using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Projectiles.Medic
{
    // The following laser shows a channeled ability, after charging up the laser will be fired
    // Using custom drawing, dust effects, and custom collision checks for tiles
    public class HealingBeamKritzkrieg : ModProjectile
    {
        public override string Texture => "TF2/Content/Projectiles/Medic/HealingBeam";

        // Use a different style for constant so it is very clear in code when a constant is used

        // The maximum charge value
        private const float MAX_CHARGE = 0f;
        // The distance charge particle from the player center
        private const float MOVE_DISTANCE = 20f;
        // The actual distance is stored in the ai0 field
        // By making a property to handle this it makes our life easier, and the accessibility more readable
        private bool startUbercharge = false;
        private float myUbercharge = 0;

        public float Distance
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        // The actual charge value is stored in the localAI0 field
        public float Charge
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        // Are we at max charge? With c#6 you can simply use => which indicates this is a get only property
        public bool IsAtMaxCharge => Charge == MAX_CHARGE;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Healing Beam");

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            //Projectile.magic = true;
            Projectile.hide = true;
            Projectile.hostile = true;
            Projectile.Name = "Healing Beam";
            Projectile.alpha = 128;
            Projectile.damage = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // We start drawing the laser if we have charged up
            if (IsAtMaxCharge)
            {
                //beamSprite does nothing
                DrawLaser(Main.spriteBatch, TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center,
                    Projectile.velocity, 10, -1.57f, 1f, (int)MOVE_DISTANCE);
            }
            return false;
        }

        // The core function of drawing a laser
        public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, float rotation = 0f, float scale = 1f, int transDist = 50)
        {
            float r = unit.ToRotation() + rotation;

            // Draws the laser 'body'
            for (float i = transDist; i <= Distance; i += step)
            {
                Color c = Color.White;
                var origin = start + i * unit;
                spriteBatch.Draw(texture, origin - Main.screenPosition,
                    new Rectangle(0, 26, 28, 26), i < transDist ? Color.Transparent : c, r,
                    new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
            }

            // Draws the laser 'tail'
            spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition,
                new Rectangle(0, 0, 28, 26), Color.White, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);

            // Draws the laser 'head'
            spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition,
                new Rectangle(0, 52, 28, 26), Color.White, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
        }

        // Change the way of collision check of the projectile
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // We can only collide if we are at max charge, which is when the laser is actually fired
            if (!IsAtMaxCharge) return false;

            Player player = Main.player[Projectile.owner];
            Vector2 unit = Projectile.velocity;
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center,
                player.Center + unit * Distance, 22, ref point);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => crit = false;

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit) => crit = false;

        // Set custom immunity time on hitting an NPC
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (!target.friendly)
            {
                target.immune[Projectile.owner] = 5;
                target.life += 1;
            }

            if (target.friendly && target.life <= target.lifeMax * 1.5f)
            {
                target.immune[Projectile.owner] = 5;
                if (!startUbercharge)
                {
                    if ((int)(2 * p.classMultiplier) <= 1)
                        target.life += 2;
                    else
                        target.life += (int)(2 * p.classMultiplier);
                }
            }

            if (target.friendly && !startUbercharge)
                myUbercharge += 1;
            else if (target.friendly && startUbercharge)
            {
                target.AddBuff(ModContent.BuffType<Buffs.KritzkriegUbercharge>(), 480);
                startUbercharge = false;
            }
        }

        public override bool? CanHitNPC(NPC target) => target.friendly;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (startUbercharge == false)
            {
                Player player = Main.player[Projectile.owner];
                TF2Player p = player.GetModPlayer<TF2Player>();
                target.lifeRegenTime += (int)(2 * p.classMultiplier);
                myUbercharge += 1;
                //Main.NewText("Ubercharge " + myUbercharge + "/1000", Color.White);
            }
            else if (startUbercharge == true)
            {
                target.AddBuff(ModContent.BuffType<Buffs.KritzkriegUbercharge>(), 480);
                startUbercharge = false;
            }
        }

        public override bool CanHitPlayer(Player player) => player != Main.player[Projectile.owner];

        // The AI of the projectile
        public override void AI()
        {
            Projectile.damage = 1;
            Player player = Main.player[Projectile.owner];
            Projectile.position = player.Center + Projectile.velocity * MOVE_DISTANCE;
            Projectile.timeLeft = 2;

            // By separating large AI into methods it becomes very easy to see the flow of the AI in a broader sense
            // First we update player variables that are needed to channel the laser
            // Then we run our charging laser logic
            // If we are fully charged, we proceed to update the laser's position
            // Finally we spawn some effects like dusts and light

            UpdatePlayer(player);
            ChargeLaser(player);

            // If laser is not charged yet, stop the AI here.
            if (Charge < MAX_CHARGE) return;

            SetLaserPosition(player);
            SpawnDusts(player);
            CastLights();
            if (myUbercharge >= 750)
                myUbercharge = 750;
            if (startUbercharge == true || player.statLife < 1)
                myUbercharge = 0;
        }

        private void SpawnDusts(Player player)
        {
            Vector2 dustPos = player.Center + Projectile.velocity * Distance;
            Vector2 unit;

            for (int i = 0; i < 2; ++i)
            {
                Vector2 offset = Projectile.velocity.RotatedBy(1.57f) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                Dust dust = Main.dust[Dust.NewDust(dustPos + offset - Vector2.One * 4f, 8, 8, DustID.Clentaminator_Red, 0.0f, 0.0f, 100, new Color(), 1.5f)];
                dust.velocity *= 0.5f;
                dust.velocity.Y = -Math.Abs(dust.velocity.Y);
                unit = dustPos - Main.player[Projectile.owner].Center;
                unit.Normalize();
                dust = Main.dust[Dust.NewDust(Main.player[Projectile.owner].Center + 55 * unit, 8, 8, DustID.Clentaminator_Red, 0.0f, 0.0f, 100, new Color(), 1.5f)];
                dust.velocity *= 0.5f;
                dust.velocity.Y = -Math.Abs(dust.velocity.Y);
            }
        }

        /*
		 * Sets the end of the laser position based on mouse position and where it collides with something
		 */
        private void SetLaserPosition(Player player)
        {
            Vector2 vectorToCursor = Main.MouseWorld - player.Center;
            float distanceToCursor = vectorToCursor.Length();
            for (Distance = distanceToCursor; Distance <= 2200f; Distance += 5f)
            {
                //Distance = distanceToCursor
                var start = player.Center * Distance; //+ projectile.velocity
                if (!Collision.CanHit(player.Center, 1, 1, start, 1, 1))
                {
                    Distance -= 5f;
                    break;
                }
            }
        }

        private void ChargeLaser(Player player)
        {
            // Kill the projectile if the player stops channeling
            if (!player.channel)
                Projectile.Kill();
            else
            {
                // Do we still have enough mana? If not, we kill the projectile because we cannot use it anymore
                if (Main.time % 10 < 1 && !player.CheckMana(player.inventory[player.selectedItem].mana, true))
                    Projectile.Kill();
                Vector2 offset = Projectile.velocity;
                offset *= MOVE_DISTANCE - 20;
                Vector2 pos = player.Center + offset - new Vector2(10, 10);
                if (Charge < MAX_CHARGE)
                    Charge++;
                int chargeFact = (int)(Charge / 20f);
                Vector2 dustVelocity = Vector2.UnitX * 18f;
                dustVelocity = dustVelocity.RotatedBy(Projectile.rotation - 1.57f);
                Vector2 spawnPos = Projectile.Center + dustVelocity;
                for (int i = 0; i < chargeFact + 1; i++)
                {
                    Vector2 spawn = spawnPos + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (12f - chargeFact * 2);
                    Dust dust = Main.dust[Dust.NewDust(pos, 20, 20, DustID.Clentaminator_Red, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f)];
                    dust.velocity = Vector2.Normalize(spawnPos - spawn) * 1.5f * (10f - chargeFact * 2f) / 10f;
                    dust.noGravity = true;
                    dust.scale = Main.rand.Next(10, 20) * 0.05f;
                }
            }
        }

        private void UpdatePlayer(Player player)
        {
            // Multiplayer support here, only run this code if the client running it is the owner of the projectile
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 diff = Main.MouseWorld - player.Center;
                diff.Normalize();
                Projectile.velocity = diff;
                Projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
                Projectile.netUpdate = true;
                TF2Player p = player.GetModPlayer<TF2Player>();
                if (myUbercharge > p.ubercharge)
                    p.ubercharge = myUbercharge;
                else
                    myUbercharge = p.ubercharge;
                TF2Player uber = player.GetModPlayer<TF2Player>();
                if (uber.activateUbercharge == true)
                    startUbercharge = true;
                else if (uber.activateUbercharge == false)
                    startUbercharge = false;
                int dir = Projectile.direction;
                player.ChangeDir(dir); // Set player direction to where we are shooting
                player.heldProj = Projectile.whoAmI; // Update player's held projectile
                player.itemTime = 2; // Set item time to 2 frames while we are used
                player.itemAnimation = 2; // Set item animation time to 2 frames while we are used
                player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * dir, Projectile.velocity.X * dir); // Set the item rotation to where we are shooting
            }
        }

        private void CastLights()
        {
            // Cast a light along the line of the laser
            DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * (Distance - MOVE_DISTANCE), 26, DelegateMethods.CastLight);
        }

        public override bool ShouldUpdatePosition() => false;

        /*
		 * Update CutTiles so the laser will cut tiles (like grass)
		 */
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * Distance, (Projectile.width + 16) * Projectile.scale, DelegateMethods.CutTiles);
        }
    }
}