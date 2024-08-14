using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class Concheror : TF2Accessory
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
            p.bannerType = 3;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<BattalionsBackup>().AddIngredient<ScrapMetal>().AddTile<CraftingAnvil>().Register();
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
            if (buffActive && Player.HasBuff<HealthRage>())
            {
                rage = 0;
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<HealthRage>());
                buffDuration = Player.buffTime[buffIndex];
            }
            if (p.stopRegen || !p.HasBanner || p.bannerType != 3) return;
            timer++;
            if (timer >= TF2.Time(1) && !TF2Player.IsHealthFull(Player))
            {
                Player.Heal(TF2.GetHealth(Player, 4));
                timer = 0;
            }
        }

        protected override void PostHitPlayer(Player opponent, Player.HurtInfo info)
        {
            if (buffActive && opponent.statLife < opponent.statLifeMax2)
            {
                TF2Player p = opponent.GetModPlayer<TF2Player>();
                int amount = TF2.Round(info.Damage / p.classMultiplier);
                amount = Utils.Clamp(amount, 0, TF2Player.GetPlayerHealthFromPercentage(Player, 35));
                opponent.Heal(amount);
            }
        }

        protected override void PostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (buffActive && !TF2Player.IsHealthFull(Player) && target.type != NPCID.TargetDummy)
            {
                TF2Player p = Player.GetModPlayer<TF2Player>();
                int amount = TF2.Round(damageDone / p.classMultiplier);
                amount = Utils.Clamp(amount, 0, TF2Player.GetPlayerHealthFromPercentage(Player, 35));
                Player.Heal(amount);
            }
        }
    }
}