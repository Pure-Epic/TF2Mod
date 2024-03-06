using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.NPCs;

namespace TF2.Content.Projectiles.Engineer
{
    public class WrenchHitbox : TF2Projectile
    {
        public override string Texture => "TF2/Content/Items/Weapons/Engineer/Wrench";

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(50, 50);
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = TF2.Time(0.8);
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox) => hitbox = TF2.MeleeHitbox(Player);

        protected override void ProjectileAI()
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.Center = Player.Center;
        }

        public override bool CanHitPlayer(Player player) => false;

        public override bool? CanHitNPC(NPC target) => target.GetGlobalNPC<TF2GlobalNPC>().building;

        protected override void ProjectileHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            miniCrit = false;
            crit = false;
            modifiers.HideCombatText();
            modifiers.DisableCrit();
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Sentry sentryNPC;
            float healMultiplier = 1f;
            if (target.type == ModContent.NPCType<SentryLevel1>() || target.type == ModContent.NPCType<SentryLevel2>() || target.type == ModContent.NPCType<SentryLevel3>() || target.type == ModContent.NPCType<MiniSentry>() || target.type == ModContent.NPCType<DispenserLevel1>() || target.type == ModContent.NPCType<DispenserLevel2>() || target.type == ModContent.NPCType<DispenserLevel3>())
            {
                if (target.type == ModContent.NPCType<SentryLevel1>() || target.type == ModContent.NPCType<SentryLevel2>() || target.type == ModContent.NPCType<SentryLevel3>())
                {
                    sentryNPC = (Sentry)target.ModNPC;
                    if (sentryNPC.wrangled)
                        healMultiplier = 0.66f;
                    else
                        healMultiplier = 1;
                }
                target.life += 1;
                if (target.life >= target.lifeMax)
                {
                    target.life = target.lifeMax;
                    return;
                }
                TF2Player p = Player.GetModPlayer<TF2Player>();
                int cost = 102;
                if (!(p.metal >= cost / 3)) return;
                if (healMultiplier < 0) { healMultiplier = 0; }
                target.life += (int)(cost * p.classMultiplier * healMultiplier);
                p.metal -= cost / 3;
                target.HealEffect((int)(cost * p.classMultiplier * healMultiplier));
            }
        }
    }
}