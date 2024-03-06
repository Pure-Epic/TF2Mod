using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.Items.Currencies;
using TF2.Content.Projectiles;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Tools
{
    public class AustraliumDrill : TF2Item
    {
		public override void SetDefaults()
		{
			Item.width = 56;
			Item.height = 30;
			Item.useTime = 1;
			Item.useAnimation = 1;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ModContent.ProjectileType<AustraliumDrillProjectile>();
			Item.shootSpeed = 32f;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.rare = ModContent.RarityType<NormalRarity>();
			Item.tileBoost = 10;
			Item.pick = Item.axe = Item.hammer = 55;
            qualityHashSet.Add(Stock);
            availability = Starter;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine nameTooltip = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria");
            AddName(tooltips);
            tooltips.Remove(nameTooltip);
            TooltipLine pickaxeTooltip = tooltips.FirstOrDefault(x => x.Name == "PickPower" && x.Mod == "Terraria");
            tooltips.Remove(pickaxeTooltip);
            TooltipLine axeTooltip = tooltips.FirstOrDefault(x => x.Name == "AxePower" && x.Mod == "Terraria");
            tooltips.Remove(axeTooltip);
            TooltipLine hammerTooltip = tooltips.FirstOrDefault(x => x.Name == "HammerPower" && x.Mod == "Terraria");
            tooltips.Remove(hammerTooltip);
            TooltipLine rangeTooltip = tooltips.FirstOrDefault(x => x.Name == "TileBoost" && x.Mod == "Terraria");
            tooltips.Remove(rangeTooltip);
            RemoveDefaultTooltips(tooltips);
            TooltipLine category = new TooltipLine(Mod, "Weapon Category", Language.GetTextValue("Mods.TF2.UI.TF2MercenaryCreation.Mercenary") + " " + Language.GetTextValue("Mods.TF2.UI.Items.Drill"))
            {
                OverrideColor = new Color(117, 107, 94, 255)
            };
            tooltips.Insert(tooltips.FindLastIndex(x => x.Name == "Name" && x.Mod == "TF2") + 1, category);
            AddOtherAttribute(tooltips, Language.GetTextValue("Mods.TF2.UI.Items.DrillDescription") + " " + Main.LocalPlayer.GetModPlayer<TF2Player>().miningPower + "%");
            if (Item.favorited)
            {
                TooltipLine favorite = new TooltipLine(Mod, "Favorite", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[56].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202, 255)
                };
                tooltips.Add(favorite);
                TooltipLine favoriteDescription = new TooltipLine(Mod, "Favorite Description", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[57].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202, 255)
                };
                tooltips.Add(favoriteDescription);
                if (Main.LocalPlayer.chest != -1)
                {
                    ChestUI.GetContainerUsageInfo(out bool sync, out Item[] chestinv);
                    if (ChestUI.IsBlockedFromTransferIntoChest(Item, chestinv))
                    {
                        TooltipLine noTransfer = new TooltipLine(Mod, "No Transfer", FontAssets.MouseText.Value.CreateWrappedText(Language.GetTextValue("UI.ItemCannotBePlacedInsideItself"), 350f))
                        {
                            OverrideColor = new Color(235, 226, 202, 255)
                        };
                        tooltips.Add(favorite);
                    }
                }
            }
            TooltipLine priceTooltip = tooltips.FirstOrDefault(x => x.Name == "Price" && x.Mod == "Terraria");
            TooltipLine price = priceTooltip;
            tooltips.Add(price);
            tooltips.Remove(priceTooltip);
            TooltipLine specialPriceTooltip = tooltips.FirstOrDefault(x => x.Name == "SpecialPrice" && x.Mod == "Terraria");
            TooltipLine specialPrice = specialPriceTooltip;
            tooltips.Add(specialPrice);
            tooltips.Remove(specialPriceTooltip);
            TooltipLine journeyResearchTooltip = tooltips.FirstOrDefault(x => x.Name == "JourneyResearch" && x.Mod == "Terraria");
            TooltipLine journeyModeTooltip = journeyResearchTooltip;
            tooltips.Add(journeyModeTooltip);
            tooltips.Remove(journeyResearchTooltip);
        }

        protected override bool WeaponModifyDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => false;

        public override void UpdateInventory(Player player) => Item.pick = Item.axe = Item.hammer = player.GetModPlayer<TF2Player>().miningPower;

        public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<Australium>()
				.AddTile<CraftingAnvil>()
				.Register();
		}
	}
}
