using Terraria;
using TF2.Content.NPCs.Buildings.SentryGun;

namespace TF2.Content.Projectiles.NPCs
{
    public class SentryBullet : Bullet
    {
        public override string Texture => "TF2/Content/Projectiles/Bullet";

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.npc[npcOwner].ModNPC is TF2Sentry sentry)
                TF2.NPCDistanceModifier(sentry.NPC, Projectile, target, ref modifiers);
        }
    }
}