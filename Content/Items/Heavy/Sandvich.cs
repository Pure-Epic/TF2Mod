using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Heavy
{
    public class Sandvich : TF2WeaponNoAmmo
    {
        public int timer;
        public int timer2;
        public int item;
        public bool eatenSandvich;
        public bool droppedSandvich;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Heavy's Unlocked Secondary\n"
                + "Eat to regain full health.\n"
                + "Alt-fire: Share a Sandvich with a friend (Medium Health Kit).");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useAnimation = 258;
            Item.useTime = 258;
            Item.useTurn = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/sandwicheat09");

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

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = null;
                Item.noUseGraphic = true;
                Item.useAnimation = 1;
            }
            else
            {
                Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/sandwicheat09");
                Item.noUseGraphic = false;
                Item.useAnimation = 258;
            }
            return !player.GetModPlayer<Buffs.FoodPlayer>().foodCooldown;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.altFunctionUse == 2)
                    droppedSandvich = true;
                else
                    eatenSandvich = true;
                if (droppedSandvich && !player.HasBuff<Buffs.FoodCooldown>())
                {
                    var source = player.GetSource_FromThis();
                    item = Item.NewItem(source, player.getRect(), ModContent.ItemType<DroppedSandvich>(), 1);
                    DroppedSandvich spawnedItem = Main.item[item].ModItem as DroppedSandvich;
                    spawnedItem.droppedPlayerName = player.name;
                    NetMessage.SendData(MessageID.SyncItem, number: item);
                    droppedSandvich = false;
                    player.AddBuff(ModContent.BuffType<Buffs.FoodCooldown>(), 1800);
                    return false;
                }
                player.AddBuff(ModContent.BuffType<Buffs.FoodCooldown>(), 1800);
            }

            return true;
        }

        public override void HoldItem(Player player)
        {
            if (eatenSandvich)
            {
                timer++;
                if (timer >= 60)
                {
                    player.statLife += (int)(0.25f * player.statLifeMax2);
                    timer = 0;
                    timer2++;
                }
                if (timer2 >= 4)
                {
                    eatenSandvich = false;
                    timer2 = 0;
                }
            }
        }
    }

    public class DroppedSandvich : ModItem
    {
        public string droppedPlayerName = string.Empty;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Sandvich");

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
        }

        public override bool CanPickup(Player player) => !(droppedPlayerName == player.name);

        public override bool OnPickup(Player player)
        {
            player.statLife += (int)(player.statLifeMax2 * 0.5f);
            Item.stack = 0;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/medkit"), player.Center);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void NetSend(BinaryWriter writer) => writer.Write(droppedPlayerName);

        public override void NetReceive(BinaryReader reader) => droppedPlayerName = reader.ReadString();
    }
}