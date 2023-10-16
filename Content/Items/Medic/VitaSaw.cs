using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Medic
{
    public class VitaSaw : TF2WeaponMelee
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vita-Saw");
            Tooltip.SetDefault("Medic's Crafted Melee");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.scale = 1f;
            Item.useTime = 58;
            Item.useAnimation = 58;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/melee_swing");
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.damage = 65;
            Item.knockBack = 0;
            Item.crit = 0;

            Item.value = Item.buyPrice(platinum: 1, gold: 4);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "Collect the organs of people you hit")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "-10 max health on wearer")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);

            var line3 = new TooltipLine(Mod, "Neutral Attributes",
                "A percentage of your ÜberCharge level is retained on death,\n"
                + " based on the number of organs harvested (15% per).\n"
                + "Total ÜberCharge retained on spawn caps at 60%.")
            {
                OverrideColor = new Color(255, 255, 255)
            };
            tooltips.Add(line3);
        }

        public override void HoldItem(Player player) => player.statLifeMax2 = (int)(player.statLifeMax2 * 0.93f);

        public override void UpdateInventory(Player player)
        {
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].type == Type && !inHotbar)
                    inHotbar = true;
            }
            if (!inHotbar && !ModContent.GetInstance<TF2ConfigClient>().InventoryStats) return;
            player.statLifeMax2 = (int)(player.statLifeMax2 * 0.93f);
            inHotbar = false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            player.GetModPlayer<TF2Player>().organs += 1;
            player.GetModPlayer<TF2Player>().organs = Utils.Clamp(player.GetModPlayer<TF2Player>().organs, 0, 4);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Ubersaw>()
                .AddIngredient<Materials.ScrapMetal>(2)
                .AddTile<Tiles.Crafting.CraftingAnvil>()
                .Register();
        }
    }

    public class VitaSawPlayer : ModPlayer
    {
        public int deathUbercharge;
        public bool giveUberchargeFromVitaSaw;
        public int timer;

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (p.ubercharge >= 600)
                deathUbercharge = (int)(p.ubercharge * 0.6f);
            else
                deathUbercharge = Utils.Clamp((int)p.ubercharge, 0, 150 * p.organs);
        }

        public override void OnRespawn(Player player) => giveUberchargeFromVitaSaw = true;

        public override void PostUpdate()
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (giveUberchargeFromVitaSaw)
            {
                timer++;
                if (timer >= 2) // MUST ALWAYS be 2!
                {
                    giveUberchargeFromVitaSaw = false;
                    p.organs = 0;
                    deathUbercharge = 0;
                    timer = 0;
                }
            }
        }
    }
}