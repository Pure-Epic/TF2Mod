using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Weapons.Scout;

namespace TF2.Content.Projectiles.Scout
{
    public class BostonBasherHitbox : TF2Projectile
    {
        public override string Texture => "TF2/Content/Items/Weapons/Scout/BostonBasher";

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(50, 50);
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = TF2.Time(0.5);
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override void ProjectileAI()
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.Center = Player.Center;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox) => hitbox = TF2.MeleeHitbox(Player);

        public override bool CanHitPlayer(Player player) => Projectile.owner != Main.myPlayer;

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!info.PvP) return;
            BostonBasherPlayer p = Player.GetModPlayer<BostonBasherPlayer>();
            p.miss = false;
            p.resetHit = true;
            BleedingPlayer player = target.GetModPlayer<BleedingPlayer>();
            player.damageMultiplier = Player.GetModPlayer<TF2Player>().damageMultiplier;
            target.AddBuff(ModContent.BuffType<Bleeding>(), TF2.Time(5), true);
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Player;
            BostonBasherPlayer p = player.GetModPlayer<BostonBasherPlayer>();
            p.miss = false;
            p.resetHit = true;
            BleedingNPC npc = target.GetGlobalNPC<BleedingNPC>();
            npc.damageMultiplier = player.GetModPlayer<TF2Player>().damageMultiplier;
            target.AddBuff(ModContent.BuffType<Bleeding>(), TF2.Time(5));
            target.immune[player.whoAmI] = player.itemAnimation;
        }
    }
}