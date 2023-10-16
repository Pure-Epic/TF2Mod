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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            PyroFlamesNPC npc = target.GetGlobalNPC<PyroFlamesNPC>();
            npc.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<PyroFlames>(), 300);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            PyroFlamesPlayer burntPlayer = target.GetModPlayer<PyroFlamesPlayer>();
            burntPlayer.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<PyroFlames>(), 300);
        }
    }
}