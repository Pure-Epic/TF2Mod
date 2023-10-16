using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TF2.Content.Projectiles.Demoman
{
    public class LochnLoadGrenade : Grenade
    {
        public override string Texture => "TF2/Content/Projectiles/Demoman/Grenade";

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.active = false;
            return false;
        }

        public override bool PreAI()
        {
            if (Timer >= maxPower)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
                Timer = maxPower;
            }
            if (!projectileInitialized)
            {
                velocity = Projectile.velocity;
                projectileInitialized = true;
            }
            return true;
        }

        public override void AI()
        {
            Timer++;
            if (Projectile.timeLeft == 0)
            {
                Projectile.tileCollide = false;
                Projectile.alpha = 255;
                Projectile.position = Projectile.Center;
                Projectile.width = 113;
                Projectile.height = 113;
                Projectile.Center = Projectile.position;
                GrenadeJump(velocity);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Temporary solution
            ModLoader.TryGetMod("Gensokyo", out Mod gensokyo);
            modifiers.SourceDamage *= ((gensokyo != null && target.ModNPC?.Mod == gensokyo && target.boss) || target.TypeName == "Byakuren Hijiri") ? 1.2f : 1f;
        }

        public override void GrenadeJump(Vector2 velocity)
        {
            if (TF2.FindPlayer(Projectile, 50f))
            {
                velocity *= 10f;
                velocity.X = Utils.Clamp(velocity.X, -25f, 25f);
                Player player = Main.player[Projectile.owner];
                player.velocity -= velocity;
                if (player.immuneNoBlink) return;
                int selfDamage = Convert.ToInt32(Math.Floor(player.statLifeMax2 * 0.2f));
                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " blew themself to smithereens."), selfDamage, 0);
            }
            Projectile.timeLeft = 0;
        }
    }
}