using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace TF2.Content.Projectiles.Scout
{
    public class ShoveHitbox : TF2Projectile
    {
        public override string Texture => "TF2/Content/Projectiles/Demoman/ShieldHitbox";

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(250, 250);
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = TF2.Time(0.2);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override bool ProjectilePreAI()
        {
            if (projectileInitialized) return true;
            noDistanceModifier = false;
            projectileInitialized = true;
            return true;
        }

        protected override void ProjectileAI() => Projectile.Size += new Vector2(25);

        public override bool ShouldUpdatePosition() => false;

        protected override void ProjectileHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            crit = false;
            if (target.hostile && target.whoAmI != Main.myPlayer)
                KnockbackPlayer(target, ref modifiers, 10f);
        }

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableCrit();
            crit = miniCrit = false;
            if (!target.friendly && target.type != NPCID.TargetDummy)
                KnockbackNPC(target, ref modifiers, 10f);
        }
    }
}