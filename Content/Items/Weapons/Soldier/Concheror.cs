using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.NPCs.Buddies;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class Concheror : TF2Weapon
    {
        public override Asset<Texture2D> WeaponActiveTexture => ModContent.Request<Texture2D>("TF2/Content/Textures/Items/Soldier/Concheror_Horn");

        protected override string BackTexture => "TF2/Content/Textures/Items/Soldier/Concheror";

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Secondary, Unique, Craft);
            SetWeaponSize(50, 50);
            SetWeaponOffset(4, 1);
            SetBannerUseStyle();
            SetWeaponAttackSpeed(2.645, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/buff_banner_horn");
            SetWeaponPrice(weapon: 1, reclaimed: 1, scrap: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override bool WeaponAddTextureCondition(Player player) => player.GetModPlayer<TF2Player>().bannerType == 3;

        protected override Asset<Texture2D> WeaponBackTexture(Player player) => !player.GetModPlayer<ConcherorPlayer>().bannerBuff ? base.WeaponBackTexture(player) : (player.direction == -1 ? ItemTextures.ConcherorTextures[0] : ItemTextures.ConcherorTextures[1]);

        protected override void WeaponAttackAnimation(Player player)
        {
            float direction = -MathHelper.PiOver2 * player.direction;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, direction);
            player.itemLocation = player.MountedCenter;
        }

        public override bool WeaponCanBeUsed(Player player)
        {
            ConcherorPlayer bannerPlayer = player.GetModPlayer<ConcherorPlayer>();
            return bannerPlayer.rage >= bannerPlayer.MaxRage;
        }

        protected override void WeaponActiveUpdate(Player player)
        {
            if (isActive && player.ItemAnimationEndingOrEnded)
            {
                int buff = ModContent.BuffType<ConcherorBuff>();
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

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<TF2Player>().bannerType = 3;

        protected override bool? WeaponOnUse(Player player)
        {
            TF2.SetPlayerDirection(player);
            isActive = true;
            return true;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<BattalionsBackup>().AddIngredient<ScrapMetal>().AddTile<AustraliumAnvil>().Register();
    }

    public class ConcherorPlayer : BannerPlayer
    {
        public override int BannerID => 3;

        public override int MaxRage => TF2.Time(8);

        private int timer;

        public override void PostUpdate()
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            rage = Utils.Clamp(rage, 0, MaxRage);
            if (!p.HasBanner)
                rage = 0;
            if (bannerBuff && Player.HasBuff<ConcherorBuff>())
            {
                rage = 0;
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<ConcherorBuff>());
                buffDuration = Player.buffTime[buffIndex];
            }
            if (p.stopRegen || !p.HasBanner || p.bannerType != 3) return;
            timer++;
            if (timer >= TF2.Time(1) && !TF2Player.IsAtFullHealth(Player))
            {
                float healAmountValue = 4 * p.HealPenaltyMultiplier;
                Player.Heal(TF2.GetHealth(Player, TF2.Round(TF2.Minimum(ref healAmountValue, 1))));
                timer = 0;
            }
        }

        protected override void PostHitPlayer(Player opponent, Player.HurtInfo info)
        {
            TF2Player p = opponent.GetModPlayer<TF2Player>();
            if (bannerBuff && !TF2Player.IsAtFullHealth(opponent))
            {
                int amount = TF2.Round(info.Damage / p.damageMultiplier);
                amount = Utils.Clamp(amount, 0, TF2Player.GetPlayerHealthFromPercentage(Player, 35));
                opponent.Heal(amount);
            }
        }

        protected override void PostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (bannerBuff && !TF2Player.IsAtFullHealth(Player) && target.type != NPCID.TargetDummy)
            {
                TF2Player p = Player.GetModPlayer<TF2Player>();
                int amount = TF2.Round(damageDone / p.damageMultiplier);
                amount = Utils.Clamp(amount, 0, TF2Player.GetPlayerHealthFromPercentage(Player, 35));
                Player.Heal(amount);
            }
        }
    }
}