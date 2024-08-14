using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;

namespace TF2.Content.Items.Consumables
{
    public class SoulItem : TF2Item
    {
        public float damageMultiplier = 1f;
        public float healthMultiplier = 1f;
        public int pierce = 1;

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 0;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useTime = Item.useAnimation = TF2.Time(0.33333);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item4;
            Item.consumable = true;
            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            noThe = true;
            qualityHashSet.Add(Unique);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine nameTooltip = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria");
            AddName(tooltips);
            tooltips.Remove(nameTooltip);
            RemoveDefaultTooltips(tooltips);
            TooltipLine category = new TooltipLine(Mod, "Weapon Category", Language.GetText("Mods.TF2.UI.Items.SoulItemCategory").Format(Language.GetTextValue("Mods.TF2.UI.TF2MercenaryCreation.Mercenary"), Language.GetTextValue("Mods.TF2.UI.Items.SoulItem")))
            {
                OverrideColor = new Color(117, 107, 94)
            };
            tooltips.Insert(tooltips.FindLastIndex(x => x.Name == "Name" && x.Mod == "TF2") + 1, category);
            AddOtherAttribute(tooltips, Language.GetText("Mods.TF2.UI.Items.SoulItemDescription").Format(damageMultiplier, healthMultiplier));
            if (Item.favorited)
            {
                TooltipLine favorite = new TooltipLine(Mod, "Favorite", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[56].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202)
                };
                tooltips.Add(favorite);
                TooltipLine favoriteDescription = new TooltipLine(Mod, "Favorite Description", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[57].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202)
                };
                tooltips.Add(favoriteDescription);
                if (Main.LocalPlayer.chest > -1)
                {
                    ChestUI.GetContainerUsageInfo(out bool sync, out Item[] chestinv);
                    if (ChestUI.IsBlockedFromTransferIntoChest(Item, chestinv))
                    {
                        TooltipLine noTransfer = new TooltipLine(Mod, "No Transfer", FontAssets.MouseText.Value.CreateWrappedText(Language.GetTextValue("UI.ItemCannotBePlacedInsideItself"), 350f))
                        {
                            OverrideColor = new Color(235, 226, 202)
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

        public override void PostUpdate() => Lighting.AddLight(Item.Center, Color.LimeGreen.ToVector3());

        public override bool CanUseItem(Player player) => player.GetModPlayer<TF2Player>().ClassSelected;

        public override bool? UseItem(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.classMultiplier < damageMultiplier)
                p.classMultiplier = damageMultiplier;
            if (p.healthMultiplier < healthMultiplier)
            {
                p.healthMultiplier = healthMultiplier;
                player.statLife = player.statLifeMax;
            }
            if (p.pierce < pierce)
                p.pierce = pierce;
            return true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);
    }
}