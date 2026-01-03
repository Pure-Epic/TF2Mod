using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Heavy
{
    public class DalokohsBar : TF2Weapon
    {
        protected override int HealthBoost => 50;

        protected override bool TemporaryHealthBoost => true;

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Secondary, Unique, Craft);
            SetWeaponSize(50, 42);
            SetFoodUseStyle();
            SetWeaponAttackSpeed(4.3, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Voicelines/heavy_sandvich");
            SetTimers(TF2.Time(10));
            SetWeaponPrice(weapon: 1, scrap: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNeutralAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player) => timer[0] >= TF2.Time(10);

        protected override bool WeaponModifyHealthCondition(Player player) => player.GetModPlayer<DalokohsBarBuffPlayer>().dalokohsBarBuff;

        protected override void WeaponActiveUpdate(Player player)
        {
            if (player.controlUseTile && !isActive && timer[0] >= TF2.Time(10) && WeaponCanAltClick(player))
            {
                IEntitySource source = player.GetSource_FromThis();
                sandvichItem = Item.NewItem(source, player.getRect(), ModContent.ItemType<DroppedDalokohsBar>(), 1);
                DroppedDalokohsBar spawnedItem = (DroppedDalokohsBar)Main.item[sandvichItem].ModItem;
                spawnedItem.droppedPlayerName = player.name;
                NetMessage.SendData(MessageID.SyncItem, number: sandvichItem);
                timer[0] = 0;
            }
            else if (isActive)
            {
                player.AddBuff(ModContent.BuffType<DalokohsBarBuff>(), TF2.Time(30));
                timer[1]++;
                if (timer[1] >= TF2.Time(1))
                {
                    int remainingHealth = TF2Player.MaxHealth(player) - player.statLife;
                    if (!TF2Player.IsAtFullHealth(player))
                        player.Heal(TF2.GetHealth(player, 25) <= remainingHealth ? TF2.GetHealth(player, 25) : remainingHealth);
                    timer[1] = 0;
                    timer[2]++;
                }
                if (timer[2] >= 4)
                {
                    timer[0] = 0;
                    timer[2] = 0;
                    isActive = false;
                }
            }
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            if (timer[0] < TF2.Time(10))
                timer[0]++;
        }

        protected override bool? WeaponOnUse(Player player)
        {
            isActive = true;
            return true;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<Sandvich>().AddIngredient<ScrapMetal>().AddTile<AustraliumAnvil>().Register();
    }

    public class DroppedDalokohsBar : DroppedSandvich
    {
        public override string Texture => "TF2/Content/Textures/Items/Heavy/DroppedDalokohsBar";

        public override void SetDefaults()
        {
            Item.width = 75;
            Item.height = 49;
        }

        public override bool OnPickup(Player player)
        {
            player.Heal(TF2Player.GetPlayerHealthFromPercentage(player, 20));
            Item.stack = 0;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/medkit"), player.Center);
            return false;
        }
    }
}