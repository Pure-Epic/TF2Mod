using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Soldier
{
    public class BattalionsBackup : BuffBanner
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Battalion's Backup");
            Tooltip.SetDefault("Soldier's Crafted Secondary");
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.accessory = true;

            Item.value = Item.buyPrice(platinum: 1, gold: 6);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "+10% max health on wearer")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Neutral Attributes",
                "Provides a defensive buff that protects nearby team members\n"
                + "from incoming projectile damage by 50% and 35% from melee damage.")
            {
                OverrideColor = new Color(255, 255, 255)
            };
            tooltips.Add(line2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.buffBanner = true;
            p.bannerType = 2;
            player.statLifeMax2 = (int)(player.statLifeMax2 * 1.1f);
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
        public override void ResetEffects()
        {
            buffActive = false;
            rage = Utils.Clamp(rage, 0, 600);
            if (!Player.GetModPlayer<TF2Player>().buffBanner)
                rage = 0;
        }

        public override void PostUpdate()
        {
            if (buffActive && Player.HasBuff<Buffs.DefenseRage>())
            {
                rage = 0;
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<Buffs.DefenseRage>());
                buffDuration = Player.buffTime[buffIndex];
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (buffActive)
            {
                crit = false;
                damage = (int)(damage * 0.65f);
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (buffActive)
            {
                crit = false;
                damage = (int)(damage * 0.5f);
            }
        }
    }
}