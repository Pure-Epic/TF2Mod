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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            PyroFlamesNPC npc = target.GetGlobalNPC<PyroFlamesNPC>();
            npc.damageMultiplier = p.classMultiplier * 3f;
            target.AddBuff(ModContent.BuffType<PyroFlames>(), 600);
            target.RequestBuffRemoval(ModContent.BuffType<PyroFlamesDegreaser>());
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            PyroFlamesPlayer burntPlayer = target.GetModPlayer<PyroFlamesPlayer>();
            burntPlayer.damageMultiplier = p.classMultiplier * 3f;
            target.AddBuff(ModContent.BuffType<PyroFlames>(), 600);
            target.ClearBuff(ModContent.BuffType<PyroFlamesDegreaser>());
        }
    }
}