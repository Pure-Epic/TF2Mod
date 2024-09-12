using Microsoft.Xna.Framework;
using Terraria;
using TF2.Content.NPCs.Buildings.SentryGun;
using TF2.Content.Projectiles.Soldier;

namespace TF2.Content.Projectiles.NPCs
{
    public class RocketNPC : Rocket
    {
        public override string Texture => "TF2/Content/Projectiles/Soldier/Rocket";

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.npc[npcOwner].ModNPC is TF2Sentry sentry)
                TF2.NPCDistanceModifier(sentry.NPC, Projectile, target, ref modifiers);
        }

        public override void RocketJump(Vector2 velocity) => Projectile.timeLeft = 0;
    }
}