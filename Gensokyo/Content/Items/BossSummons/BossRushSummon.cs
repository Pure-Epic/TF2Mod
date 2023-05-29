using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Gensokyo.Events;
using TF2.Content.Items;
using Terraria.Localization;
using TF2.Gensokyo.Common;

namespace TF2.Gensokyo.Content.Items.BossSummons
{
    [ExtendsFromMod("Gensokyo")]
    public class BossRushSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Disenchanted Sutra Scroll");
            Tooltip.SetDefault("Starts Boss Rush");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 48;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.buyPrice(silver: 10);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer && GensokyoDLC.gensokyoLoaded)
            {
                if (!GensokyoDLC.bossRush && !SpawnByakuren.startSpawningByakuren)
                {
                    string text = "Boss Rush Initiated!";
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(text, 175, 75, 255);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(25, -1, -1, NetworkText.FromLiteral(text), 255, 175f, 75f, 255f, 0, 0, 0);
                    SoundEngine.PlaySound(SoundID.ForceRoar, player.position);
                    GensokyoBossRush.GetPlayer(player);
                    GensokyoBossRush.StartBossRush();
                }
                else if (GensokyoDLC.bossRush)
                {
                    string text = "Boss Rush Canceled";
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(text, 175, 75, 255);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(25, -1, -1, NetworkText.FromLiteral(text), 255, 175f, 75f, 255f, 0, 0, 0);
                    GensokyoBossRush.EndBossRush();
                }
            }
            return true;
        }

        public override void AddRecipes()
        {
            if (GensokyoDLC.Gensokyo != null)
            {
                GensokyoDLC.Gensokyo.TryFind("PointItem", out ModItem pointItem);
                CreateRecipe()
                .AddIngredient(pointItem.Type, 25)
                .AddIngredient(ItemID.LunarBar, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
            }
        }
    }
}
