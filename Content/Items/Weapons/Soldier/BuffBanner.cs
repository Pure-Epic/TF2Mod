using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class BuffBanner : TF2Accessory
    {
        protected override void WeaponStatistics() => SetWeaponCategory(Soldier, Secondary, Unique, Unlock);

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.hasBanner = true;
            p.bannerType = 1;
        }
    }

    public abstract class BannerPlayer : ModPlayer
    {
        public int bannerID;
        public int rage;
        public int maxRage = 600;
        public bool buffActive;
        public int buffDuration;

        public override void ResetEffects() => buffActive = false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (Player.GetModPlayer<TF2Player>().hasBanner && bannerID == p.bannerType && !buffActive)
                rage += (int)(damageDone / p.classMultiplier);
            PostHitNPC(target, hit, damageDone);
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (!info.PvP) return;
            Player opponent = Main.player[info.DamageSource.SourcePlayerIndex];
            TF2Player p = opponent.GetModPlayer<TF2Player>();
            BannerPlayer banner = opponent.GetModPlayer<BannerPlayer>();
            if (p.hasBanner && banner.bannerID == p.bannerType || banner.buffActive)
                opponent.GetModPlayer<BannerPlayer>().rage += (int)(info.Damage / p.classMultiplier);
            PostHitPlayer(info);
        }

        protected virtual void PostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        { }

        protected virtual void PostHitPlayer(Player.HurtInfo info)
        { }
    }

    public class BuffBannerPlayer : BannerPlayer
    {
        public override void PostUpdate()
        {
            bannerID = 1;
            maxRage = 600;
            rage = Utils.Clamp(rage, 0, maxRage);
            if (!Player.GetModPlayer<TF2Player>().hasBanner)
                rage = 0;
            if (buffActive && Player.HasBuff<Rage>())
            {
                rage = 0;
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<Rage>());
                buffDuration = Player.buffTime[buffIndex];
            }
        }
    }
}