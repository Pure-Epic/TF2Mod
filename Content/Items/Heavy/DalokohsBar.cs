using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Heavy
{
    public class DalokohsBar : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Secondary, Unique, Craft);
            SetWeaponSize(50, 42);
            SetFoodUseStyle();
            SetWeaponAttackSpeed(4.3, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/sandwicheat09");
            SetWeaponPrice(weapon: 1, scrap: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNeutralAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player) => !player.HasBuff<FoodCooldown>();

        protected override void WeaponActiveUpdate(Player player)
        {
            if (player.controlUseTile && !eatingSandvich && !player.HasBuff<FoodCooldown>() && WeaponCanAltClick(player))
            {
                IEntitySource source = player.GetSource_FromThis();
                sandvichItem = Item.NewItem(source, player.getRect(), ModContent.ItemType<DroppedDalokohsBar>(), 1);
                DroppedDalokohsBar spawnedItem = (DroppedDalokohsBar)Main.item[sandvichItem].ModItem;
                spawnedItem.droppedPlayerName = player.name;
                NetMessage.SendData(MessageID.SyncItem, number: sandvichItem);
                player.AddBuff(ModContent.BuffType<FoodCooldown>(), 600);
            }
            else if (eatingSandvich)
            {
                timer[0]++;
                if (timer[0] >= 60)
                {
                    player.Heal((int)(0.08333333333f * player.statLifeMax2));
                    timer[0] = 0;
                    timer[1]++;
                }
                if (timer[1] >= 4)
                {
                    player.AddBuff(ModContent.BuffType<DalokohsBarBuff>(), 1800);
                    player.AddBuff(ModContent.BuffType<FoodCooldown>(), 600);
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

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ScrapMetal>()
                .AddIngredient<Sandvich>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class DroppedDalokohsBar : DroppedSandvich
    {
        public override string Texture => "TF2/Content/Textures/DroppedDalokohsBar";

        public override void SetDefaults()
        {
            Item.width = 75;
            Item.height = 49;
        }

        public override bool OnPickup(Player player)
        {
            player.Heal((int)(0.2f * player.statLifeMax2));
            Item.stack = 0;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/medkit"), player.Center);
            return false;
        }
    }
}