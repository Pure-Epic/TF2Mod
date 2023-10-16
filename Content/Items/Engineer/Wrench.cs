using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Engineer
{
    public class Wrench : TF2WeaponMelee
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Engineer's Starter Melee\n"
                             + "Repairs buildings on hit.");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.scale = 1f;
            Item.useTime = 48;
            Item.useAnimation = 48;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/wrench_swing");
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.shoot = ModContent.ProjectileType<WrenchHitbox>();
            Item.rare = ModContent.RarityType<NormalRarity>();

            Item.damage = 65;
            Item.knockBack = 0;
            Item.crit = 0;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, 1, knockback, player.whoAmI);
            return false;
        }
    }

    public class WrenchHitbox : ModProjectile
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Wrench");

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.damage = 0;
            Projectile.timeLeft = 1;
        }

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.velocity = Main.player[Main.myPlayer].velocity;
        }

        public override bool CanHitPlayer(Player player) => false;

        public override bool? CanHitNPC(NPC target) => target.GetGlobalNPC<TF2GlobalNPC>().building;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.netMode != NetmodeID.SinglePlayer) return;
            NPCs.Sentry sentryNPC;
            float healMultiplier = 1f;
            if (target.type == ModContent.NPCType<NPCs.SentryLevel1>() || target.type == ModContent.NPCType<NPCs.SentryLevel2>() || target.type == ModContent.NPCType<NPCs.SentryLevel3>() || target.type == ModContent.NPCType<NPCs.DispenserLevel1>() || target.type == ModContent.NPCType<NPCs.DispenserLevel2>() || target.type == ModContent.NPCType<NPCs.DispenserLevel3>())
            {
                if (target.type == ModContent.NPCType<NPCs.SentryLevel1>() || target.type == ModContent.NPCType<NPCs.SentryLevel2>() || target.type == ModContent.NPCType<NPCs.SentryLevel3>())
                {
                    sentryNPC = target.ModNPC as NPCs.Sentry;
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
                TF2Player p = Main.player[Main.myPlayer].GetModPlayer<TF2Player>();
                int cost = 102;
                if (!(p.metal >= cost / 3)) return;
                if (healMultiplier < 0) { healMultiplier = 0; }
                target.life += (int)(cost * p.classMultiplier * healMultiplier);
                p.metal -= cost / 3;
            }
        }

    }
}