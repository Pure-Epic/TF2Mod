using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Soldier
{
    public class BuffBanner : TF2AccessorySecondary
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Buff Banner");
            Tooltip.SetDefault("Soldier's Unlocked Secondary\n"
                + "Provides an offensive buff that causes all team members to do mini-crits.\n"
                + "Rage increases through damage done.");
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.accessory = true;

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.bannerType = 1;
            p.buffBanner = true;
        }
    }

    public class BannerPlayer : ModPlayer
    {
        public int rage;
        public bool buffActive;
        public int buffDuration;

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if ((!Player.GetModPlayer<TF2Player>().buffBanner) || buffActive) return;
            rage += (int)(damage / Player.GetModPlayer<TF2Player>().classMultiplier);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (proj.GetGlobalProjectile<Projectiles.TF2ProjectileBase>().spawnedFromNPC) return;
            if ((!Player.GetModPlayer<TF2Player>().buffBanner) || buffActive) return;
            rage += (int)(damage / Player.GetModPlayer<TF2Player>().classMultiplier);
        }

        public override void OnHitPvp(Item item, Player target, int damage, bool crit)
        {
            if ((!Player.GetModPlayer<TF2Player>().buffBanner) || buffActive) return;
            rage += (int)(damage / Player.GetModPlayer<TF2Player>().classMultiplier);
        }

        public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
        {
            if (proj.GetGlobalProjectile<Projectiles.TF2ProjectileBase>().spawnedFromNPC) return;
            if ((!Player.GetModPlayer<TF2Player>().buffBanner) || buffActive) return;
            rage += (int)(damage / Player.GetModPlayer<TF2Player>().classMultiplier);
        }
    }

    public class BuffBannerPlayer : BannerPlayer
    {
        public override void ResetEffects()
        {
            buffActive = false;
            rage = Utils.Clamp(rage, 0, 600);
            if (!Player.GetModPlayer<TF2Player>().buffBanner)
                rage = 0;
        }

        public override void PostUpdate()
        {
            if (buffActive && Player.HasBuff<Buffs.Rage>())
            {
                rage = 0;
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<Buffs.Rage>());
                buffDuration = Player.buffTime[buffIndex];
            }
        }
    }
}