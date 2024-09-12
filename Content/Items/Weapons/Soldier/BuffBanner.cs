using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class BuffBanner : TF2Accessory
    {
        protected override string BackTexture => "TF2/Content/Textures/Items/Soldier/BuffBanner";

        protected override void WeaponStatistics() => SetWeaponCategory(Soldier, Secondary, Unique, Unlock);

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        protected override bool WeaponAddTextureCondition(Player player) => player.GetModPlayer<TF2Player>().bannerType == 1;

        protected override Asset<Texture2D> WeaponBackTexture(Player player) => !player.GetModPlayer<BuffBannerPlayer>().buffActive ? base.WeaponBackTexture(player) : (player.direction == -1 ? ItemTextures.BuffBannerTextures[0] : ItemTextures.BuffBannerTextures[1]);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.bannerType = 1;
        }
    }

    public abstract class BannerPlayer : ModPlayer
    {
        public virtual int BannerID => 0;

        public virtual int MaxRage => TF2.Time(10);

        public int rage;
        public bool buffActive;
        public int buffDuration;

        public override void ResetEffects() => buffActive = false;

        public override void OnHurt(Player.HurtInfo info)
        {
            if (!info.PvP) return;
            Player opponent = Main.player[info.DamageSource.SourcePlayerIndex];
            TF2Player p = opponent.GetModPlayer<TF2Player>();
            BannerPlayer banner = opponent.GetModPlayer<BannerPlayer>();
            if (p.HasBanner && banner.BannerID == p.bannerType && !banner.buffActive)
                banner.rage += TF2.Round(info.Damage / p.damageMultiplier);
            PostHitPlayer(opponent, info);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type == NPCID.TargetDummy) return;
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (Player.GetModPlayer<TF2Player>().HasBanner && BannerID == p.bannerType && !buffActive)
                rage += TF2.Round(damageDone / p.damageMultiplier);
            PostHitNPC(target, hit, damageDone);
        }

        protected virtual void PostHitPlayer(Player opponent, Player.HurtInfo info)
        { }

        protected virtual void PostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        { }
    }

    public class BuffBannerPlayer : BannerPlayer
    {
        public override int BannerID => 1;

        public override void PostUpdate()
        {
            rage = Utils.Clamp(rage, 0, MaxRage);
            if (!Player.GetModPlayer<TF2Player>().HasBanner)
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