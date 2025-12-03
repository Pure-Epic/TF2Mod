using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Weapons.Heavy
{
    public class Sandvich : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Secondary, Unique, Unlock);
            SetWeaponSize(50, 28);
            SetFoodUseStyle();
            SetWeaponAttackSpeed(4.3, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Voicelines/heavy_sandvich");
            SetTimers(TF2.Time(30));
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        public override bool WeaponCanBeUsed(Player player) => timer[0] >= TF2.Time(30);

        protected override void WeaponActiveUpdate(Player player)
        {
            if (player.controlUseTile && !isActive && timer[0] >= TF2.Time(30) && WeaponCanAltClick(player))
            {
                IEntitySource source = player.GetSource_FromThis();
                sandvichItem = Item.NewItem(source, player.getRect(), ModContent.ItemType<DroppedSandvich>(), 1);
                DroppedSandvich spawnedItem = (DroppedSandvich)Main.item[sandvichItem].ModItem;
                spawnedItem.droppedPlayerName = player.name;
                NetMessage.SendData(MessageID.SyncItem, number: sandvichItem);
                timer[0] = 0;
            }
            else if (isActive)
            {
                timer[0] = 0;
                timer[1]++;
                if (timer[1] >= TF2.Time(1))
                {
                    int remainingHealth = TF2Player.TotalHealth(player) - player.statLife;
                    if (!TF2Player.IsHealthFull(player))
                        player.Heal(TF2.GetHealth(player, 75) <= remainingHealth ? TF2.GetHealth(player, 75) : remainingHealth);
                    timer[1] = 0;
                    timer[2]++;
                }
                if (timer[2] >= 4)
                {
                    timer[2] = 0;
                    isActive = false;
                }
            }
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            if (timer[0] < TF2.Time(30))
                timer[0]++;
        }

        protected override bool? WeaponOnUse(Player player)
        {
            isActive = true;
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
            player.Heal(TF2Player.GetPlayerHealthFromPercentage(player, 50));
            Item.stack = 0;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/medkit"), player.Center);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void NetSend(BinaryWriter writer) => writer.Write(droppedPlayerName);

        public override void NetReceive(BinaryReader reader) => droppedPlayerName = reader.ReadString();
    }
}