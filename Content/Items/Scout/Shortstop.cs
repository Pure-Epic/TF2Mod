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
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Scout;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Scout
{
    public class Shortstop : TF2Weapon
    {
        private int shoveCooldown;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Scout's Crafted Primary");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/scatter_gun_shoot");
            Item.autoReuse = true;

            Item.damage = 12;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();

            ammoCost = 1;
            maxAmmoClip = 4;
            ammoInClip = 4;
            reloadRate = 91f;
            magazine = true;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/short_stop_reload");

            Item.value = Item.buyPrice(platinum: 1, gold: 6);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "+100% damage bonus\n"
                + "+40% faster firing speed\n"
                + "40% more accurate")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "-34% clip size\n"
                + "-60% bullets per shot\n"
                + "Increase in push force taken from damage while item is active")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);

            var line3 = new TooltipLine(Mod, "Neutral Attributes",
                "Alt-Fire to reach and shove someone!")
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
            clip.ammoCurrent = ammoInClip;
            if (clip.startReload)
            {
                reload = true;
                clip.startReload = false;
            }
            UpdateResource();

            if (Main.mouseRight && shoveCooldown <= 0)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, player.DirectionTo(Main.MouseWorld), ModContent.ProjectileType<ShoveHitbox>(), 1, 0f, player.whoAmI);
                shoveCooldown = 90;
            }

            shoveCooldown--;
            if (shoveCooldown <= 0)
                shoveCooldown = 0;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => ammoInClip > 0;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            Item.scale = 0.8f;
            TF2Player p = player.GetModPlayer<TF2Player>();
            if ((p.classAccessory && !p.classHideVanity) || player.altFunctionUse == 2)
                Item.noUseGraphic = true;
            else
                Item.noUseGraphic = false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (ammoInClip == 0 || reload) return false;
            ammoInClip -= ammoCost;

            for (int i = 0; i < 4; i++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(7.2f));
                Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Bullet>(), damage, knockback, player.whoAmI);
            }

            if (ammoInClip <= 0)
                reload = true;
            return false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player) => !(ammoInClip <= 0 || reload || player.altFunctionUse == 2);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ForceANature>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}