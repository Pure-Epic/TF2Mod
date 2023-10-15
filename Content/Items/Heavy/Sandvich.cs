using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;

namespace TF2.Content.Items.Heavy
{
    public class Sandvich : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Secondary, Unique, Unlock);
            SetWeaponSize(50, 28);
            SetFoodUseStyle();
            SetWeaponAttackSpeed(4.3, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/sandwicheat09");
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        public override bool WeaponCanBeUsed(Player player) => !player.HasBuff<FoodCooldown>();

        protected override void WeaponActiveUpdate(Player player)
        {
            if (player.controlUseTile && !eatingSandvich && !player.HasBuff<FoodCooldown>() && WeaponCanAltClick(player))
            {
                IEntitySource source = player.GetSource_FromThis();
                sandvichItem = Item.NewItem(source, player.getRect(), ModContent.ItemType<DroppedSandvich>(), 1);
                DroppedSandvich spawnedItem = (DroppedSandvich)Main.item[sandvichItem].ModItem;
                spawnedItem.droppedPlayerName = player.name;
                NetMessage.SendData(MessageID.SyncItem, number: sandvichItem);
                player.AddBuff(ModContent.BuffType<FoodCooldown>(), 1800);
            }
            else if (eatingSandvich)
            {
                timer[0]++;
                if (timer[0] >= 60)
                {
                    player.Heal((int)(0.25f * player.statLifeMax2));
                    timer[0] = 0;
                    timer[1]++;
                }
                if (timer[1] >= 4)
                {
                    player.AddBuff(ModContent.BuffType<FoodCooldown>(), 1800);
                    timer[1] = 0;
                    eatingSandvich = false;
                }
            }
        }

        protected override bool? WeaponOnUse(Player player)
        {
            eatingSandvich = true;
            return true;
        }
    }

    public class DroppedSandvich : ModItem
    {
        public string droppedPlayerName = string.Empty;

        public override string Texture => "TF2/Content/Textures/DroppedSandvich";

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 0;

        public override void SetDefaults()
        {
            Item.width = 75;
            Item.height = 41;
        }

        public override bool CanPickup(Player player) => droppedPlayerName != player.name;

        public override bool OnPickup(Player player)
        {
            player.Heal((int)(0.5f * player.statLifeMax2));
            Item.stack = 0;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/medkit"), player.Center);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void NetSend(BinaryWriter writer) => writer.Write(droppedPlayerName);

        public override void NetReceive(BinaryReader reader) => droppedPlayerName = reader.ReadString();
    }
}