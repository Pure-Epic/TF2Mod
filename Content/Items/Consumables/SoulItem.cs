using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TF2.Common;

namespace TF2.Content.Items.Consumables
{
    public class SoulItem : TF2Item
    {
        protected override string CustomCategory => base.CustomCategory;

        public float healthMultiplier = 1f;
        public float damageMultiplier = 1f;
        public int pierce = 1;
        public int pickaxePower = 50;
        internal int owner;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 0;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useTime = Item.useAnimation = TF2.Time(0.33333);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item4;
            Item.consumable = true;
            Item.value = Item.buyPrice(platinum: 1);
            Item.instanced = true;
            WeaponAddQuality(Unique);
            noThe = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => CustomTooltips(tooltips, Language.GetText("Mods.TF2.UI.Items.ItemCategory").Format(Language.GetTextValue("Mods.TF2.UI.TF2MercenaryCreation.Mercenary"), Language.GetTextValue("Mods.TF2.UI.Items.SoulItem")), Language.GetText("Mods.TF2.UI.Items.SoulItemDescription").Format(damageMultiplier, healthMultiplier, pierce, pickaxePower));

        public override void PostUpdate() => Lighting.AddLight(Item.Center, Color.LimeGreen.ToVector3());

        public override bool CanUseItem(Player player) => player.GetModPlayer<TF2Player>().ClassSelected;

        public override bool? UseItem(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.healthMultiplier < healthMultiplier)
            {
                p.healthMultiplier = healthMultiplier;
                player.statLife = TF2Player.MaxHealth(player);
            }
            if (p.damageMultiplier < damageMultiplier)
                p.damageMultiplier = damageMultiplier;
            if (p.pierce < pierce)
                p.pierce = pierce;
            return true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void LoadData(TagCompound tag)
        {
            healthMultiplier = tag.GetFloat("health");
            damageMultiplier = tag.GetFloat("damage");
            pierce = tag.GetInt("pierce");
            pickaxePower = tag.GetInt("pickaxePower");
        }

        public override void SaveData(TagCompound tag)
        {
            tag["health"] = healthMultiplier;
            tag["damage"] = damageMultiplier;
            tag["pierce"] = pierce;
            tag["pickaxePower"] = pickaxePower;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(healthMultiplier);
            writer.Write(damageMultiplier);
            writer.Write(pierce);
            writer.Write(pickaxePower);
        }

        public override void NetReceive(BinaryReader reader)
        {
            healthMultiplier = reader.ReadSingle();
            damageMultiplier = reader.ReadSingle();
            pierce = reader.ReadInt32();
            pickaxePower = reader.ReadInt32();
        }
    }
}