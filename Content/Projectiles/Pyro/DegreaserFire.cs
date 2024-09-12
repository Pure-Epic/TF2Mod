using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Pyro
{
    public class DegreaserFire : Fire
    {
        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            PyroFlamesDegreaserPlayer burntPlayer = target.GetModPlayer<PyroFlamesDegreaserPlayer>();
            burntPlayer.damageMultiplier = p.damageMultiplier;
            target.ClearBuff(ModContent.BuffType<PyroFlames>());
            target.AddBuff(ModContent.BuffType<PyroFlamesDegreaser>(), TF2.Time(5.4));
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            PyroFlamesDegreaserNPC npc = target.GetGlobalNPC<PyroFlamesDegreaserNPC>();
            npc.damageMultiplier = p.damageMultiplier;
            TF2.ExtinguishPyroFlames(target, ModContent.BuffType<PyroFlames>());
            target.AddBuff(ModContent.BuffType<PyroFlamesDegreaser>(), TF2.Time(5.4));
        }
    }
}