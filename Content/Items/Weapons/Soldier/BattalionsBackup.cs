using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class BattalionsBackup : TF2Accessory
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Secondary, Unique, Craft);
            SetWeaponPrice(weapon: 1, reclaimed: 1);
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
            p.bannerType = 2;
            TF2Player.SetPlayerHealth(player, 20);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BuffBanner>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class BattalionsBackupPlayer : BannerPlayer
    {
        public override void PostUpdate()
        {
            bannerID = 2;
            maxRage = 600;
            rage = Utils.Clamp(rage, 0, maxRage);
            if (!Player.GetModPlayer<TF2Player>().hasBanner)
                rage = 0;
            if (buffActive && Player.HasBuff<DefenseRage>())
            {
                rage = 0;
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<DefenseRage>());
                buffDuration = Player.buffTime[buffIndex];
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (buffActive)
                modifiers.FinalDamage *= 0.65f;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (buffActive)
                modifiers.FinalDamage *= 0.5f;
        }
    }
}