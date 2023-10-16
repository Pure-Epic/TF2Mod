using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Pyro
{
    public class Axtinguisher : TF2WeaponMelee
    {
        public bool holdingItemForFirstTime;
        public int thisItem;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Pyro's Unlocked Melee");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {           
            Item.width = 50;
            Item.height = 50;
            holsterSpeed = 41;
            Item.useTime = 48 + holsterSpeed;
            Item.useAnimation = 48;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/melee_swing");
            Item.autoReuse = true;

            Item.damage = 44;
            Item.GetGlobalItem<TF2ItemBase>().noRandomCrits = true;

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "Mini-crits burning targets and extinguishes them.\n"
                + "Damage increases based on remaining duration of afterburn")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "-33% damage penalty\n"
                + "No random critical hits\n"
                + "This weapon holsters 35% slower")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
        }

        public override void HoldItem(Player player)
        {
            thisItem = player.selectedItem;
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].type == ModContent.ItemType<Axtinguisher>() && !inHotbar)
                    inHotbar = true;
            }
            if (holdingItemForFirstTime)
            {
                if (inHotbar)
                    player.itemTime = holsterSpeed;
                holdingItemForFirstTime = false;
            }
            if (player.controlUseItem)
            {
                if (player.itemTime <= holsterSpeed)
                    player.itemTime = 0;
                if (player.itemTime == 0)
                    player.itemTime = Item.useTime;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.ModItem is not Axtinguisher)
            {
                if (!player.ItemTimeIsZero && !holdingItemForFirstTime)
                    player.selectedItem = thisItem;
                else if (player.ItemTimeIsZero && !player.cursorItemIconEnabled)
                {
                    holdingItemForFirstTime = true;
                    inHotbar = false;
                    deployTimer = 0;
                }
            }
        }

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (target.HasBuff(ModContent.BuffType<Buffs.PyroFlames>()))
            {
                player.GetModPlayer<TF2Player>().miniCrit = true;
                int buffIndex = target.FindBuffIndex(ModContent.BuffType<Buffs.PyroFlames>());
                damage = (int)((44f + (8f * target.buffTime[buffIndex] / 60f)) * player.GetModPlayer<TF2Player>().classMultiplier);
                target.buffTime[buffIndex] = 0;
            }
            else if (player.GetModPlayer<TF2Player>().critMelee)
            {
                player.GetModPlayer<TF2Player>().crit = true;
                player.ClearBuff(ModContent.BuffType<Buffs.MeleeCrit>());
            }
            else
                crit = false;
        }

        public override void ModifyHitPvp(Player player, Player target, ref int damage, ref bool crit)
        {
            if (target.HasBuff(ModContent.BuffType<Buffs.PyroFlames>()))
            {
                player.GetModPlayer<TF2Player>().miniCrit = true;
                int buffIndex = target.FindBuffIndex(ModContent.BuffType<Buffs.PyroFlames>());
                damage = (int)((44f + (8f * target.buffTime[buffIndex] / 60f)) * player.GetModPlayer<TF2Player>().classMultiplier);
                target.buffTime[buffIndex] = 0;
            }
            else if (player.GetModPlayer<TF2Player>().critMelee)
            {
                player.GetModPlayer<TF2Player>().crit = true;
                player.ClearBuff(ModContent.BuffType<Buffs.MeleeCrit>());
            }
            else
                crit = false;
        }
    }
}