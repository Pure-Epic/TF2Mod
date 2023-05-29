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

namespace TF2.Content.Items.Engineer
{
    public class Gunslinger : TF2WeaponMelee
    {
        public int hits;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Engineer's Unlocked Melee\n"
                             + "Can repair buildings");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.scale = 1f;
            Item.useTime = 48;
            Item.useAnimation = 48;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/wrench_swing");
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.shoot = ModContent.ProjectileType<WrenchHitbox>();

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

            var line = new TooltipLine(Mod, "Positive Attributes",
                "+20% max health on wearer\n"
                + "-23% sentry cost\n"
                + "Third successful punch in a row always crits.")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "No random critical hits\n"
                + "Replaces the Sentry with a Mini-Sentry")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
        }

        public override void UpdateInventory(Player player)
        {
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].type == Type && !inHotbar)
                    inHotbar = true;
            }
            if (!ModContent.GetInstance<TF2ConfigClient>().InventoryStats && !inHotbar) return;
            player.statLifeMax2 = (int)(player.statLifeMax2 * 1.2f);
            player.GetModPlayer<GunslingerPlayer>().gunslingerEquipped = true;
            inHotbar = false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, 1, knockback, player.whoAmI);
            return false;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (player.GetModPlayer<TF2Player>().critMelee)
                player.GetModPlayer<TF2Player>().crit = true;

            hits++;
            if (hits >= 3)
            {
                player.GetModPlayer<TF2Player>().crit = true;
                hits = 0;
            }
            else
                crit = false;
        }

        public override void ModifyHitPvp(Player player, Player target, ref int damage, ref bool crit)
        {
            hits++;
            if (hits >= 3)
            {
                player.GetModPlayer<TF2Player>().crit = true;
                hits = 0;
            }
            else
                crit = false;
        }
    }

    public class GunslingerPlayer : ModPlayer
    {
        public bool gunslingerEquipped;

        public override void ResetEffects() => gunslingerEquipped = false;
    }
}