using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Common;

namespace TF2.Content.Items.Engineer
{
    public class SouthernHospitality : TF2WeaponMelee
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Engineer's Crafted Melee");
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

            Item.value = Item.buyPrice(platinum: 1, gold: 2);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "On Hit: Bleed for 5 seconds")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "No random critical hits\n"
                + "20% contact damage vulnerability on wearer")
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
            if (!inHotbar && !ModContent.GetInstance<TF2ConfigClient>().InventoryStats) return;
            player.GetModPlayer<SouthernHospitalityPlayer>().southernHospitalityEquipped = true;
            inHotbar = false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, 1, knockback, player.whoAmI);
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            BleedingNPC npc = target.GetGlobalNPC<BleedingNPC>();
            npc.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<Bleeding>(), 300, false);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Spy.Ambassador>()
                .AddIngredient<Materials.ScrapMetal>()
                .AddTile<Tiles.Crafting.CraftingAnvil>()
                .Register();
        }
    }

    public class SouthernHospitalityPlayer : ModPlayer
    {
        public bool southernHospitalityEquipped;

        public override void ResetEffects() => southernHospitalityEquipped = false;

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (southernHospitalityEquipped)
                damage = (int)(damage * 1.2f);
        }
    }
}