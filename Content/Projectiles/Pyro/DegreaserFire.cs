using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Pyro
{
    public class DegreaserFire : Fire
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            PyroFlamesDegreaserNPC npc = target.GetGlobalNPC<PyroFlamesDegreaserNPC>();
            npc.damageMultiplier = p.classMultiplier;
            TF2.ExtinguishPyroFlames(target, ModContent.BuffType<PyroFlames>());
            target.AddBuff(ModContent.BuffType<PyroFlamesDegreaser>(), 600);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            PyroFlamesDegreaserPlayer burntPlayer = target.GetModPlayer<PyroFlamesDegreaserPlayer>();
            burntPlayer.damageMultiplier = p.classMultiplier;
            target.ClearBuff(ModContent.BuffType<PyroFlames>());
            target.AddBuff(ModContent.BuffType<PyroFlamesDegreaser>(), 600);
        }
    }
}