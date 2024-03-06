using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Pyro;

namespace TF2.Content.Items.Consumables
{
    public class SmallHealth : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 0;

        public override void SetDefaults()
        {
            Item.width = 23;
            Item.height = 30;
            Item.rare = ItemRarityID.White;
        }

        public override bool ItemSpace(Player player) => true;

        public override bool OnPickup(Player player)
        {
            player.Heal(!player.GetModPlayer<BackScratcherPlayer>().backScratcherEquipped ? TF2Player.GetPlayerHealthFromPercentage(player, 20) : TF2Player.GetPlayerHealthFromPercentage(player, 30));
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/medkit"), player.Center);
            Item.stack = 0;
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);
    }

    public class SmallHealthPotion : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 10;

        public override void SetDefaults()
        {
            Item.width = 23;
            Item.height = 30;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/medkit");
            Item.consumable = true;
            Item.maxStack = 30;
            Item.potion = true;
            Item.healLife = 20;
            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ItemRarityID.White;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "HealLife");
            if (line != null)
                line.Text = Language.GetTextValue("Restores 20% of max life");
        }

        public override void GetHealLife(Player player, bool quickHeal, ref int healValue) => healValue = !player.GetModPlayer<BackScratcherPlayer>().backScratcherEquipped ? TF2Player.GetPlayerHealthFromPercentage(player, 20) : TF2Player.GetPlayerHealthFromPercentage(player, 30);
    }

    public class MediumHealth : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 0;

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 35;
            Item.rare = ItemRarityID.White;
        }

        public override bool ItemSpace(Player player) => true;

        public override bool OnPickup(Player player)
        {
            player.Heal(!player.GetModPlayer<BackScratcherPlayer>().backScratcherEquipped ? TF2Player.GetPlayerHealthFromPercentage(player, 50) : TF2Player.GetPlayerHealthFromPercentage(player, 75));
            Item.stack = 0;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/medkit"), player.Center);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);
    }

    public class MediumHealthPotion : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 10;

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 35;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/medkit");
            Item.consumable = true;
            Item.maxStack = 30;
            Item.potion = true;
            Item.healLife = 50;
            Item.value = Item.buyPrice(platinum: 2);
            Item.rare = ItemRarityID.White;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "HealLife");
            if (line != null)
                line.Text = Language.GetTextValue("Restores 50% of max life");
        }

        public override void GetHealLife(Player player, bool quickHeal, ref int healValue) => healValue = !player.GetModPlayer<BackScratcherPlayer>().backScratcherEquipped ? TF2Player.GetPlayerHealthFromPercentage(player, 50) : TF2Player.GetPlayerHealthFromPercentage(player, 75);
    }

    public class LargeHealth : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 0;

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 40;
            Item.rare = ItemRarityID.White;
        }

        public override bool ItemSpace(Player player) => true;

        public override bool OnPickup(Player player)
        {
            player.Heal(!player.GetModPlayer<BackScratcherPlayer>().backScratcherEquipped ? TF2Player.TotalHealth(player) : (int)(TF2Player.TotalHealth(player) * 1.5f));
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/medkit"), player.Center);
            Item.stack = 0;
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);
    }

    public class LargeHealthPotion : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 10;

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 40;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/medkit");
            Item.consumable = true;
            Item.maxStack = 30;
            Item.healLife = 100;
            Item.potion = true;
            Item.value = Item.buyPrice(platinum: 5);
            Item.rare = ItemRarityID.White;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "HealLife");
            if (line != null)
                line.Text = Language.GetTextValue("Restores all life");
        }

        public override void GetHealLife(Player player, bool quickHeal, ref int healValue) => healValue = !player.GetModPlayer<BackScratcherPlayer>().backScratcherEquipped ? TF2Player.TotalHealth(player) : TF2Player.GetPlayerHealthFromPercentage(player, 150);
    }
}