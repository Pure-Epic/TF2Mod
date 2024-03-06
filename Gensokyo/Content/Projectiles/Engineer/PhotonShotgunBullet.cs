using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Projectiles;

namespace TF2.Gensokyo.Content.Projectiles.Engineer
{
    [ExtendsFromMod("Gensokyo")]
    public class PhotonShotgunBullet : Bullet
    {
        public override string Texture => "TF2/Content/Projectiles/Bullet";

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            PyroFlamesPlayer burntPlayer = target.GetModPlayer<PyroFlamesPlayer>();
            burntPlayer.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<PyroFlames>(), TF2.Time(5));
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            PyroFlamesNPC npc = target.GetGlobalNPC<PyroFlamesNPC>();
            npc.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<PyroFlames>(), TF2.Time(5));
        }
    }
}