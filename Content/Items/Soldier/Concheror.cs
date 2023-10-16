using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Soldier
{
    public class Concheror : TF2AccessorySecondary
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Secondary, Unique, Craft);
            SetWeaponPrice(weapon: 1, reclaimed: 1, scrap: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNeutralAttribute(description);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.hasBanner = true;
            p.bannerType = 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ScrapMetal>()
                .AddIngredient<BattalionsBackup>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class ConcherorPlayer : BannerPlayer
    {
        private int timer;

        public override void PostUpdate()
        {
            bannerID = 3;
            maxRage = 480;
            TF2Player p = Player.GetModPlayer<TF2Player>();
            rage = Utils.Clamp(rage, 0, maxRage);
            if (!p.hasBanner)
                rage = 0;
            if (buffActive && Player.HasBuff<HealthRage>())
            {
                rage = 0;
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<HealthRage>());
                buffDuration = Player.buffTime[buffIndex];
            }
            if (p.stopRegen || !p.hasBanner) return;
            timer++;
            if (timer >= 60 && Player.statLife < Player.statLifeMax2)
            {
                Player.Heal((int)(0.02f * p.regenMultiplier * Player.statLifeMax2));
                timer = 0;
            }
        }

        protected override void PostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (buffActive && Player.statLife < Player.statLifeMax2)
            {
                TF2Player p = Player.GetModPlayer<TF2Player>();
                int amount = (int)(damageDone / p.classMultiplier);
                amount = Utils.Clamp(amount, 0, (int)(Player.statLifeMax2 * 0.35f));
                Player.Heal(amount);
            }
        }

        protected override void PostHitPlayer(Player.HurtInfo info)
        {
            if (!info.PvP) return;
            Player opponent = Main.player[info.DamageSource.SourcePlayerIndex];
            if (buffActive && opponent.statLife < opponent.statLifeMax2)
            {
                TF2Player p = opponent.GetModPlayer<TF2Player>();
                int amount = (int)(info.Damage / p.classMultiplier);
                amount = Utils.Clamp(amount, 0, (int)(opponent.statLifeMax2 * 0.35f));
                opponent.Heal(amount);
            }
        }
    }
}