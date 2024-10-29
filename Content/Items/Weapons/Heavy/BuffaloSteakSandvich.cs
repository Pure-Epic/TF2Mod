using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Heavy
{
    public class BuffaloSteakSandvich : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Secondary, Unique, Craft);
            SetWeaponSize(50, 28);
            SetFoodUseStyle();
            SetWeaponAttackSpeed(4.3, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Voicelines/heavy_sandvich");
            SetTimers(TF2.Time(30));
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        public override bool WeaponCanBeUsed(Player player) => timer[0] >= TF2.Time(30);

        protected override void WeaponActiveUpdate(Player player)
        {
            if (player.controlUseTile && !eatingSandvich && timer[0] >= TF2.Time(30) && WeaponCanAltClick(player))
            {
                IEntitySource source = player.GetSource_FromThis();
                sandvichItem = Item.NewItem(source, player.getRect(), ModContent.ItemType<DroppedBuffaloSteakSandvich>(), 1);
                DroppedBuffaloSteakSandvich spawnedItem = (DroppedBuffaloSteakSandvich)Main.item[sandvichItem].ModItem;
                spawnedItem.droppedPlayerName = player.name;
                NetMessage.SendData(MessageID.SyncItem, number: sandvichItem);
                timer[0] = 0;
            }
            else if (eatingSandvich)
            {
                timer[0] = 0;
                timer[1]++;
                if (timer[1] >= TF2.Time(4.3))
                {
                    player.AddBuff(ModContent.BuffType<BuffaloSteakSandvichBuff>(), TF2.Time(16));
                    timer[1] = 0;
                    eatingSandvich = false;
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
            eatingSandvich = true;
            return true;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<Sandvich>().AddIngredient<ReclaimedMetal>().AddTile<AustraliumAnvil>().Register();
    }

    public class DroppedBuffaloSteakSandvich : DroppedSandvich
    {
        public override string Texture => "TF2/Content/Textures/DroppedBuffaloSteakSandvich";

        public override void SetDefaults()
        {
            Item.width = 75;
            Item.height = 49;
        }
    }
}