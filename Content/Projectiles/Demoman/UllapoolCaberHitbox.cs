using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace TF2.Content.Projectiles.Demoman
{
    public class UllapoolCaberHitbox : TF2Projectile
    {
        public NPC sparedNPC;
        public Player sparedPlayer;

        public override string Texture => "TF2/Content/Items/Weapons/Demoman/UllapoolCaber";

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(100, 100);
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = TF2.Time(0.0167);
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool CanHitPlayer(Player target) => target != sparedPlayer;

        public override bool? CanHitNPC(NPC target) => target != sparedNPC;

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (crit)
                modifiers.SetCrit();
            else
                modifiers.DisableCrit();
        }

        protected override void ProjectileDestroy(int timeLeft)
        {
            if (FindOwner(Projectile, 100f))
            {
                Vector2 velocity = new Vector2(15f * Player.direction, 15f);
                Player.velocity -= velocity;
                if (Player.immuneNoBlink) return;
                int selfDamage = TF2.GetHealth(Player, 55);
                Player.Hurt(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[2].ToNetworkText(Player.name)), selfDamage, 0, cooldownCounter: 5);
            }
        }
    }
}