using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Consumables
{
    public class SoulItem : TF2Item
    {
        protected override string CustomCategory => base.CustomCategory;

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

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CustomTooltips(tooltips, Language.GetText("Mods.TF2.UI.Items.ItemCategory").Format(Language.GetTextValue("Mods.TF2.UI.TF2MercenaryCreation.Mercenary"), Language.GetTextValue("Mods.TF2.UI.Items.SoulItem")), Language.GetText("Mods.TF2.UI.Items.SoulItemDescription").Format(damageMultiplier, healthMultiplier));

        public override void PostUpdate() => Lighting.AddLight(Item.Center, Color.LimeGreen.ToVector3());

        public override bool CanUseItem(Player player) => player.GetModPlayer<TF2Player>().ClassSelected;

        public override bool? UseItem(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.damageMultiplier < damageMultiplier)
                p.damageMultiplier = damageMultiplier;
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