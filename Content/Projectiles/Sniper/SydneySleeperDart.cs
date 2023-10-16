using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items;

namespace TF2.Content.Projectiles.Sniper
{
    public class SydneySleeperDart : ModProjectile
    {
        public bool projectileInitialized;
        public int jarateDuration;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BulletHighVelocity);
            Projectile.width = 50;
            Projectile.height = 4;
            Projectile.DamageType = ModContent.GetInstance<TF2DamageClass>();
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            AIType = ProjectileID.BulletHighVelocity;
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            target.AddBuff(BuffID.Ichor, jarateDuration);
            target.AddBuff(ModContent.BuffType<JarateDebuff>(), jarateDuration);
            if (Projectile.GetGlobalProjectile<TF2ProjectileBase>().miniCrit)
            {
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    if (player.buffType[i] == ModContent.BuffType<JarateCooldown>())
                        player.buffTime[i] -= 60;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Player player = Main.player[Projectile.owner];
            if (!info.PvP) return;
            target.AddBuff(BuffID.Ichor, jarateDuration, true);
            target.AddBuff(ModContent.BuffType<JarateDebuff>(), jarateDuration);
            if (Projectile.GetGlobalProjectile<TF2ProjectileBase>().miniCrit)
            {
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    if (player.buffType[i] == ModContent.BuffType<JarateCooldown>())
                        player.buffTime[i] -= 60;
                }
            }
        }
    }
}