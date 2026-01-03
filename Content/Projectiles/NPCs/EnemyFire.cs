using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Projectiles.Pyro;

namespace TF2.Content.Projectiles.NPCs
{
    public class EnemyFire : Fire
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Flames;

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(6, 6);
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = TF2.Time(1);
            Projectile.alpha = 255;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }


        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            FlameThrowerDebuffPlayer burntPlayer = target.GetModPlayer<FlameThrowerDebuffPlayer>();
            burntPlayer.damageMultiplier = TF2.GlobalHealthMultiplier;
            target.ClearBuff(ModContent.BuffType<DegreaserDebuff>());
            target.AddBuff(ModContent.BuffType<FlameThrowerDebuff>(), TF2.Time(10), true);
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            FlameThrowerDebuffNPC npc = target.GetGlobalNPC<FlameThrowerDebuffNPC>();
            npc.damageMultiplier = TF2.GlobalHealthMultiplier;
            TF2.ExtinguishPyroFlames(target, ModContent.BuffType<DegreaserDebuff>());
            target.AddBuff(ModContent.BuffType<FlameThrowerDebuff>(), TF2.Time(10));
        }
    }
}