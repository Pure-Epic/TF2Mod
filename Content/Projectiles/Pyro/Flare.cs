using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Pyro
{
    public class Flare : TF2Projectile
    {
        protected override void ProjectileStatistics()
        {
            SetProjectileSize(20, 11);
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override void ProjectileAI() => SetRotation();

        protected override void ProjectileHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (target.HasBuff(ModContent.BuffType<PyroFlames>()) || target.HasBuff(ModContent.BuffType<PyroFlamesDegreaser>()))
                Player.GetModPlayer<TF2Player>().crit = true;
        }

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.HasBuff(ModContent.BuffType<PyroFlames>()) || target.HasBuff(ModContent.BuffType<PyroFlamesDegreaser>()))
                Player.GetModPlayer<TF2Player>().crit = true;
        }

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            PyroFlamesPlayer burntPlayer = target.GetModPlayer<PyroFlamesPlayer>();
            burntPlayer.damageMultiplier = p.classMultiplier;
            target.ClearBuff(ModContent.BuffType<PyroFlamesDegreaser>());
            target.AddBuff(ModContent.BuffType<PyroFlames>(), 450);
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            PyroFlamesNPC npc = target.GetGlobalNPC<PyroFlamesNPC>();
            npc.damageMultiplier = p.classMultiplier;
            TF2.ExtinguishPyroFlames(target, ModContent.BuffType<PyroFlamesDegreaser>());
            target.AddBuff(ModContent.BuffType<PyroFlames>(), 450);
        }
    }
}