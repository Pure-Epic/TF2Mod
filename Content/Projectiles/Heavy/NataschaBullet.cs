using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Heavy
{
    public class NataschaBullet : Bullet
    {
        public override string Texture => "TF2/Content/Projectiles/Bullet";

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            target.GetModPlayer<NataschaDebuffPlayer>().slowMultiplier = Utils.Clamp(Vector2.Distance(Main.player[Projectile.owner].Center, target.Center) / 1000f, 0.3f, 1f);
            target.AddBuff(ModContent.BuffType<NataschaDebuff>(), TF2.Time(1));
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.GetGlobalNPC<NataschaDebuffNPC>().slowMultiplier = Utils.Clamp(Vector2.Distance(Main.player[Projectile.owner].Center, target.Center) / 1000f, 0.3f, 1f);
            target.AddBuff(ModContent.BuffType<NataschaDebuff>(), TF2.Time(1));
        }
    }
}