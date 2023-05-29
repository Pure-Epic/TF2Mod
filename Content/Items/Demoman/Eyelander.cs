using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Demoman
{
    public class Eyelander : TF2WeaponMelee
    {
        public bool holdingItemForFirstTime;
        public int thisItem;
        public bool eyelanderInHotbar;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Demoman's Unlocked Melee\n"
                + "This weapon has a large melee range and deploys and holsters slower");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            deploySpeed = 53;
            holsterSpeed = 53;

            Item.width = 50;
            Item.height = 50;
            Item.scale = 2f;
            Item.useTime = 48 + holsterSpeed;
            Item.useAnimation = 48;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/melee_swing");
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.damage = 65;
            Item.knockBack = 0;
            Item.crit = 0;
            Item.GetGlobalItem<TF2ItemBase>().noRandomCrits = true;

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Negative Attributes",
                "No random critical hits\n"
                + "-14% max health on wearer")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Neutral Attributes",
                "Gives increased speed and health with every head you take.")
            {
                OverrideColor = new Color(255, 255, 255)
            };
            tooltips.Add(line2);
        }

        public override bool CanUseItem(Player player) => deployTimer >= deploySpeed;

        public override void HoldItem(Player player)
        {
            thisItem = player.selectedItem;
            deployTimer++;
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].type == Type && !eyelanderInHotbar)
                    eyelanderInHotbar = true;
            }
            if (holdingItemForFirstTime)
            {
                if (eyelanderInHotbar)
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
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].type == Type && !inHotbar)
                    inHotbar = true;
            }
            if (!inHotbar && !ModContent.GetInstance<TF2ConfigClient>().InventoryStats) return;
            player.statLifeMax2 = (int)(player.statLifeMax2 * 0.85714285714f);
            player.GetModPlayer<EyelanderPlayer>().eyelanderInInventory = true;
            if (player.GetModPlayer<EyelanderPlayer>().eyelanderInInventory)
            {
                player.statLifeMax2 += player.GetModPlayer<EyelanderPlayer>().heads * (int)(player.statLifeMax2 * 0.1f);
                if (!player.controlMount)
                {
                    player.moveSpeed += player.GetModPlayer<EyelanderPlayer>().heads;
                    player.GetModPlayer<TF2Player>().speedMultiplier += (int)(player.GetModPlayer<EyelanderPlayer>().heads * 0.05f);
                }
            }
            else
            {
                player.statLifeMax2 = 0;
                player.moveSpeed = 0;
                player.GetModPlayer<TF2Player>().speedMultiplier = 1f;
            }

            if (player.HeldItem.ModItem is not Eyelander)
            {
                if (!player.ItemTimeIsZero && !holdingItemForFirstTime)
                    player.selectedItem = thisItem;
                else if (player.ItemTimeIsZero && !player.cursorItemIconEnabled)
                {
                    holdingItemForFirstTime = true;
                    eyelanderInHotbar = false;
                    deployTimer = 0;
                }
            }
            inHotbar = false;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            //Main.NewText(player.GetModPlayer<TF2Player>().heads);
            if (player.GetModPlayer<TF2Player>().crit || player.GetModPlayer<TF2Player>().critMelee)
            {
                player.GetModPlayer<EyelanderPlayer>().heads += 1;
                player.statLife += (int)(0.1f * player.statLifeMax2);
                if (player.GetModPlayer<TF2Player>().critMelee)
                    player.GetModPlayer<TF2Player>().crit = true;
                player.ClearBuff(ModContent.BuffType<Buffs.MeleeCrit>());
            }
            else
                crit = false;
        }

        public override void ModifyHitPvp(Player player, Player target, ref int damage, ref bool crit)
        {
            if (player.GetModPlayer<TF2Player>().crit || player.GetModPlayer<TF2Player>().critMelee)
            {
                player.GetModPlayer<EyelanderPlayer>().heads += 1;
                player.statLife += (int)(0.1f * player.statLifeMax2);
                if (player.GetModPlayer<TF2Player>().critMelee)
                    player.GetModPlayer<TF2Player>().crit = true;
                player.ClearBuff(ModContent.BuffType<Buffs.MeleeCrit>());
            }
            else
                crit = false;
        }
    }

    public class EyelanderPlayer : ModPlayer
    {
        public int heads;
        public bool eyelanderInInventory;

        public override void ResetEffects() => eyelanderInInventory = false;

        public override void UpdateDead()
        {
            if (heads > 0)
                Player.statLifeMax2 -= heads * (int)(Player.statLifeMax2 * 0.1f);
            heads = 0;
        }

        public override void OnRespawn(Player player) => heads = 0;

        public override void PreUpdateMovement() => Player.moveSpeed += heads * 10f;

        public override void PreUpdate()
        {
            heads = Utils.Clamp(heads, 0, 4);
            Player.statLifeMax2 += heads * (int)(Player.statLifeMax2 * 0.1f);
        }
    }
}