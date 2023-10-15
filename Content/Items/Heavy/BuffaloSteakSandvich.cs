using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Heavy
{
    public class BuffaloSteakSandvich : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Secondary, Unique, Craft);
            SetWeaponSize(50, 28);
            SetFoodUseStyle();
            SetWeaponAttackSpeed(4.3, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/sandwicheat09");
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        public override bool WeaponCanBeUsed(Player player) => !player.HasBuff<FoodCooldown>();

        protected override void WeaponActiveUpdate(Player player)
        {
            if (player.controlUseTile && !eatingSandvich && !player.HasBuff<FoodCooldown>() && WeaponCanAltClick(player))
            {
                IEntitySource source = player.GetSource_FromThis();
                sandvichItem = Item.NewItem(source, player.getRect(), ModContent.ItemType<DroppedBuffaloSteakSandvich>(), 1);
                DroppedBuffaloSteakSandvich spawnedItem = (DroppedBuffaloSteakSandvich)Main.item[sandvichItem].ModItem;
                spawnedItem.droppedPlayerName = player.name;
                NetMessage.SendData(MessageID.SyncItem, number: sandvichItem);
                player.AddBuff(ModContent.BuffType<FoodCooldown>(), 1800);
            }
            else if (eatingSandvich)
            {
                timer[0]++;
                if (timer[0] >= 258)
                {
                    player.AddBuff(ModContent.BuffType<BuffaloSteakSandvichBuff>(), 960);
                    player.AddBuff(ModContent.BuffType<FoodCooldown>(), 1800);
                    timer[0] = 0;
                    eatingSandvich = false;
                }
            }
        }

        protected override bool? WeaponOnUse(Player player)
        {
            eatingSandvich = true;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ReclaimedMetal>()
                .AddIngredient<Sandvich>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
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