using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.NPCs.Buddies;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class BuffBanner : TF2Weapon
    {
        public override Asset<Texture2D> WeaponActiveTexture => ModContent.Request<Texture2D>("TF2/Content/Textures/Items/Soldier/BuffBanner_Bugle");

        protected override string BackTexture => "TF2/Content/Textures/Items/Soldier/BuffBanner";

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Secondary, Unique, Unlock);
            SetWeaponSize(50, 50);
            SetWeaponOffset(4, 1);
            SetBannerUseStyle();
            SetWeaponAttackSpeed(2.645, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/buff_banner_horn");
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        protected override bool WeaponAddTextureCondition(Player player) => player.GetModPlayer<TF2Player>().bannerType == 1;

        protected override Asset<Texture2D> WeaponBackTexture(Player player) => !player.GetModPlayer<BuffBannerPlayer>().bannerBuff ? base.WeaponBackTexture(player) : (player.direction == -1 ? ItemTextures.BuffBannerTextures[0] : ItemTextures.BuffBannerTextures[1]);

        protected override void WeaponAttackAnimation(Player player)
        {
            float direction = -MathHelper.PiOver2 * player.direction;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, direction);          
            player.itemLocation = player.MountedCenter;
        }

        public override bool WeaponCanBeUsed(Player player)
        {
            BuffBannerPlayer bannerPlayer = player.GetModPlayer<BuffBannerPlayer>();
            return bannerPlayer.rage >= bannerPlayer.MaxRage;
        }

        protected override void WeaponActiveUpdate(Player player)
        {
            if (isActive && player.ItemAnimationEndingOrEnded)
            {
                int buff = ModContent.BuffType<BuffBannerBuff>();
                foreach (Player targetPlayer in Main.ActivePlayers)
                    targetPlayer.AddBuff(buff, TF2.Time(10));
                foreach (NPC targetNPC in Main.ActiveNPCs)
                {
                    if (targetNPC.ModNPC is Buddy)
                        targetNPC.AddBuff(buff, TF2.Time(10));
                }
                isActive = false;
            }
        }

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<TF2Player>().bannerType = 1;

        protected override bool? WeaponOnUse(Player player)
        {
            TF2.SetPlayerDirection(player);
            isActive = true;
            return true;
        }
    }

    public abstract class BannerPlayer : ModPlayer
    {
        public virtual int BannerID => 0;

        public virtual int MaxRage => TF2.Time(10);

        public bool bannerBuff;
        public int rage;
        public int buffDuration;

        public override void ResetEffects() => bannerBuff = false;

        public override void OnHurt(Player.HurtInfo info)
        {
            if (!info.PvP) return;
            Player opponent = Main.player[info.DamageSource.SourcePlayerIndex];
            TF2Player p = opponent.GetModPlayer<TF2Player>();
            BannerPlayer banner = opponent.GetModPlayer<BannerPlayer>();
            if (p.HasBanner && banner.BannerID == p.bannerType && !banner.bannerBuff)
                banner.rage += TF2.Round(info.Damage / p.damageMultiplier);
            PostHitPlayer(opponent, info);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type == NPCID.TargetDummy) return;
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (Player.GetModPlayer<TF2Player>().HasBanner && BannerID == p.bannerType && !bannerBuff)
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
            if (bannerBuff && Player.HasBuff<BuffBannerBuff>())
            {
                rage = 0;
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<BuffBannerBuff>());
                buffDuration = Player.buffTime[buffIndex];
            }
        }
    }
}