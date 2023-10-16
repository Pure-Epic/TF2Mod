using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.NPCs;

namespace TF2.Content.Items.Engineer
{
    public class Wrench : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Engineer, Melee, Stock, Starter);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/wrench_swing");
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        protected override bool? WeaponOnUse(Player player)
        {
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<WrenchHitbox>(), 1, 0f);
            return true;
        }
    }

    public class WrenchHitbox : ModProjectile
    {
        public override string Texture => "TF2/Content/Items/Engineer/Wrench";

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 48;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox) => hitbox = TF2.MeleeHitbox(Main.player[Projectile.owner]);

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.velocity = Main.player[Projectile.owner].velocity;
        }

        public override bool CanHitPlayer(Player player) => false;

        public override bool? CanHitNPC(NPC target) => target.GetGlobalNPC<TF2GlobalNPC>().building;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Main.player[Projectile.owner].GetModPlayer<TF2Player>().crit = false;
            modifiers.HideCombatText();
            modifiers.DisableCrit();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
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
                TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
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