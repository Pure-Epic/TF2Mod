using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TF2.Content.Items.Consumables
{
    public class SmallHealth : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Small Health Kit");

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.rare = ItemRarityID.White;
        }

        public override bool OnPickup(Player player)
        {
            player.statLife += (int)(player.statLifeMax2 * 0.2f);
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/medkit"), player.Center);
            Item.stack = 0;
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override bool ItemSpace(Player player) => true;
    }

    public class SmallHealthPotion : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Small Health Kit");

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.noUseGraphic = true;
            Item.useAnimation = 60;
            Item.useTime = 60;
            Item.useTurn = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/medkit");
            Item.maxStack = 30;
            Item.consumable = true;
            Item.rare = ItemRarityID.White;
            Item.value = Item.buyPrice(platinum: 1);

            Item.healLife = 100; // While we change the actual healing value in GetHealLife, Item.healLife still needs to be higher than 0 for the item to be considered a healing item
            Item.potion = true; // Makes it so this item applies potion sickness on use and allows it to be used with quick heal
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Find the tooltip line that corresponds to 'Heals ... life'
            // See https://tmodloader.github.io/tModLoader/html/class_terraria_1_1_mod_loader_1_1_tooltip_line.html for a list of vanilla tooltip line names
            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "HealLife");

            if (line != null)
            {
                // Change the text to 'Heals max/2 (max/4 when quick healing) life'
                line.Text = Language.GetTextValue("Restores 20% of max life");
            }
        }

        public override void GetHealLife(Player player, bool quickHeal, ref int healValue) => healValue = player.statLifeMax2 / 5;
    }

    public class MediumHealth : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Medium Health Kit");

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.rare = ItemRarityID.White;
        }

        public override bool OnPickup(Player player)
        {
            player.statLife += (int)(player.statLifeMax2 * 0.5f);
            Item.stack = 0;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/medkit"), player.Center);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override bool ItemSpace(Player player) => true;
    }

    public class MediumHealthPotion : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Medium Health Kit");

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.noUseGraphic = true;
            Item.useAnimation = 60;
            Item.useTime = 60;
            Item.useTurn = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/medkit");
            Item.maxStack = 30;
            Item.consumable = true;
            Item.rare = ItemRarityID.White;
            Item.value = Item.buyPrice(platinum: 2);

            Item.healLife = 100; // While we change the actual healing value in GetHealLife, Item.healLife still needs to be higher than 0 for the item to be considered a healing item
            Item.potion = true; // Makes it so this item applies potion sickness on use and allows it to be used with quick heal
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Find the tooltip line that corresponds to 'Heals ... life'
            // See https://tmodloader.github.io/tModLoader/html/class_terraria_1_1_mod_loader_1_1_tooltip_line.html for a list of vanilla tooltip line names
            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "HealLife");

            if (line != null)
            {
                // Change the text to 'Heals max/2 (max/4 when quick healing) life'
                line.Text = Language.GetTextValue("Restores 50% of max life");
            }
        }

        public override void GetHealLife(Player player, bool quickHeal, ref int healValue) => healValue = player.statLifeMax2 / 2;
    }

    public class LargeHealth : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Large Health Kit");

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.rare = ItemRarityID.White;
        }

        public override bool OnPickup(Player player)
        {
            player.statLife += player.statLifeMax2;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/medkit"), player.Center);
            Item.stack = 0;
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override bool ItemSpace(Player player) => true;
    }

    public class LargeHealthPotion : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Large Health Kit");

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.noUseGraphic = true;
            Item.useAnimation = 60;
            Item.useTime = 60;
            Item.useTurn = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/medkit");
            Item.maxStack = 30;
            Item.consumable = true;
            Item.rare = ItemRarityID.White;
            Item.value = Item.buyPrice(platinum: 5);

            Item.healLife = 100; // While we change the actual healing value in GetHealLife, Item.healLife still needs to be higher than 0 for the item to be considered a healing item
            Item.potion = true; // Makes it so this item applies potion sickness on use and allows it to be used with quick heal
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Find the tooltip line that corresponds to 'Heals ... life'
            // See https://tmodloader.github.io/tModLoader/html/class_terraria_1_1_mod_loader_1_1_tooltip_line.html for a list of vanilla tooltip line names
            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "HealLife");

            if (line != null)
            {
                // Change the text to 'Heals max/2 (max/4 when quick healing) life'
                line.Text = Language.GetTextValue("Restores all life");
            }
        }

        public override void GetHealLife(Player player, bool quickHeal, ref int healValue) => healValue = player.statLifeMax2;
    }
}
