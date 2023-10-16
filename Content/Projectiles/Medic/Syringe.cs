using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Projectiles.Medic
{
    public class Syringe : ModProjectile
    {
        public bool projectileInitialized;

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool PreDraw(ref Color lightColor) => TF2.DrawProjectile(Projectile, ref lightColor);

        public override bool PreAI()
        {
            if (projectileInitialized) return true;
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            Projectile.penetrate = p.pierce;
            projectileInitialized = true;
            return true;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation();
    }
}