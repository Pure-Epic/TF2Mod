using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.Tools
{
    public class AustraliumDrill : TF2Item
    {
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 30;
            Item.useTime = Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<AustraliumDrillProjectile>();
            Item.shootSpeed = 32f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.tileBoost = 10;
            Item.pick = Item.axe = Item.hammer = 55;
            WeaponAddQuality(Stock);
            availability = Starter;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine pickaxeTooltip = tooltips.FirstOrDefault(x => x.Name == "PickPower" && x.Mod == "Terraria");
            tooltips.Remove(pickaxeTooltip);
            TooltipLine axeTooltip = tooltips.FirstOrDefault(x => x.Name == "AxePower" && x.Mod == "Terraria");
            tooltips.Remove(axeTooltip);
            TooltipLine hammerTooltip = tooltips.FirstOrDefault(x => x.Name == "HammerPower" && x.Mod == "Terraria");
            tooltips.Remove(hammerTooltip);
            TooltipLine rangeTooltip = tooltips.FirstOrDefault(x => x.Name == "TileBoost" && x.Mod == "Terraria");
            tooltips.Remove(rangeTooltip);
            CustomTooltips(tooltips, Language.GetTextValue("Mods.TF2.UI.Items.Tool"), Language.GetText("Mods.TF2.UI.Items.DrillDescription").Format(Main.LocalPlayer.GetModPlayer<TF2Player>().miningPower));
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse != 2)
                Item.pick = Item.axe = Item.hammer = player.GetModPlayer<TF2Player>().miningPower;
            else
            {
                Item.pick = Item.axe = 0;
                Item.hammer = player.GetModPlayer<TF2Player>().miningPower;
            }
            return base.UseItem(player);
        }
    }
}
