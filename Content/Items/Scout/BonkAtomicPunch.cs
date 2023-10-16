using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Scout
{
    public class BonkAtomicPunch : TF2WeaponNoAmmo
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bonk Atomic Punch");
            Tooltip.SetDefault("Scout's Unlocked Secondary\n"
                + "Drink to become invulnerable for 8 seconds.\n"
                + "Cannot attack during this time.\n"
                + "Damage absorbed will slow you when the effect ends.");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 72;
            Item.useTime = 72;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.classAccessory && !p.classHideVanity)
                Item.noUseGraphic = true;
            else
                Item.noUseGraphic = false;
        }

        public override bool CanUseItem(Player player) => !player.GetModPlayer<Buffs.BonkPlayer>().bonkCooldown;

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                player.AddBuff(ModContent.BuffType<Buffs.Radiation>(), 480);
                player.AddBuff(ModContent.BuffType<Buffs.BonkCooldown>(), 1320);
            }
            return true;
        }
    }
}