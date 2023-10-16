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
using TF2.Content.Items.Ammo;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Soldier;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Soldier
{
    public class RocketJumper : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rocket Jumper");
            Tooltip.SetDefault("Soldier's Crafted Primary");
            ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Item.type] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 24;
            Item.useTime = 48;
            Item.useAnimation = 48;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/rocket_jumper_shoot");
            Item.autoReuse = true;

            Item.damage = 0;
            Item.shoot = ModContent.ProjectileType<RocketJumperRocket>();
            Item.shootSpeed = 25f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            
            ammoCost = 1;
            maxAmmoClip = 4;
            ammoInClip = 4;
            reloadRate = 48f;
            initialReloadRate = 55f;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/rocket_reload");

            Item.value = Item.buyPrice(platinum: 6, gold: 6);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "+50% chance to save ammo\n"
                + "No self inflicted blast damage taken")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "-100% damage penalty\n"
                + "No random critical hits")

            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);

            var line3 = new TooltipLine(Mod, "Neutral Attributes",
                "A special rocket launcher for learning rocket jump tricks and patterns.\n"
                + "This weapon deals ZERO damage.")
            {
                OverrideColor = new Color(255, 255, 255)
            };
            tooltips.Add(line3);
        }

        public override void HoldItem(Player player)
        {
            WeaponSystem clip = player.GetModPlayer<WeaponSystem>();
            clip.ammoMax = maxAmmoClip;
            clip.ammoReloadRate = reloadRate;
            clip.initialAmmoReloadRate = initialReloadRate;
            clip.ammoCurrent = ammoInClip;
            if (clip.startReload)
            {
                reload = true;
                clip.startReload = false;
            }
            UpdateResource();
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.classAccessory && !p.classHideVanity)
                Item.noUseGraphic = true;
            else
                Item.noUseGraphic = false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            reload = false;
            ammoInClip -= ammoCost;

            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<RocketJumperRocket>(), damage, knockback, player.whoAmI);

            if (ammoInClip <= 0)
                reload = true;
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-25f, -5f);

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextFloat() >= 0.5f;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Gunboats>(3)
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}