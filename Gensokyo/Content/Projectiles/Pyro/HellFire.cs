using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Projectiles.Pyro;

namespace TF2.Gensokyo.Content.Projectiles.Pyro
{
    [ExtendsFromMod("Gensokyo")]
    public class HellFire : Fire
    {
        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            PyroFlamesPlayer burntPlayer = target.GetModPlayer<PyroFlamesPlayer>();
            burntPlayer.damageMultiplier = p.classMultiplier * 3f;
            target.ClearBuff(ModContent.BuffType<PyroFlamesDegreaser>());
            target.AddBuff(ModContent.BuffType<PyroFlames>(), TF2.Time(10), true);
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            PyroFlamesNPC npc = target.GetGlobalNPC<PyroFlamesNPC>();
            npc.damageMultiplier = p.classMultiplier * 3f;
            TF2.ExtinguishPyroFlames(target, ModContent.BuffType<PyroFlamesDegreaser>());
            target.AddBuff(ModContent.BuffType<PyroFlames>(), TF2.Time(10));
        }
    }
}