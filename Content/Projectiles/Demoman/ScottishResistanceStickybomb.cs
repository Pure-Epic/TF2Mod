using Microsoft.Xna.Framework;
using Terraria;

namespace TF2.Content.Projectiles.Demoman
{
    public class ScottishResistanceStickybomb : Stickybomb
    {
        protected override void ProjectileAI()
        {
            if (Timer >= TF2.Time(5))
                noDistanceModifier = true;
            if (Projectile.timeLeft == 0)
            {
                Projectile.position = Projectile.Center;
                Projectile.Size = new Vector2(250, 250);
                Projectile.friendly = true;
                Projectile.hide = true;
                Projectile.tileCollide = false;
                Projectile.Center = Projectile.position;
                StickyJump(velocity);
            }
            if (!Stick && Projectile.timeLeft != 0)
                StartingAI();
            else
                GroundAI();
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (Projectile.Hitbox.Intersects(npc.Hitbox) && npc.boss && !npc.friendly && !npc.dontTakeDamage)
                {
                    TargetWhoAmI = npc.whoAmI;
                    Stick = true;
                    StickOnEnemy = true;
                    Projectile.netUpdate = true;
                }
            }
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (Projectile.Hitbox.Intersects(projectile.Hitbox) && TF2.CanParryProjectile(projectile) && projectile.hostile && !projectile.friendly)
                    projectile.Kill();
            }
        }
    }
}