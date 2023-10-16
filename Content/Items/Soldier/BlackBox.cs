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
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Soldier;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Soldier
{
    public class BlackBox : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Black Box");
            Tooltip.SetDefault("Soldier's Crafted Primary");
            ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Item.type] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 27;
            Item.useTime = 48;
            Item.useAnimation = 48;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/rocket_blackbox_shoot");
            Item.autoReuse = true;

            Item.damage = 90;
            Item.shoot = ModContent.ProjectileType<BlackBoxRocket>();
            Item.shootSpeed = 20f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.knockBack = 20f;

            ammoCost = 1;
            maxAmmoClip = 3;
            ammoInClip = 3;
            reloadRate = 48f;
            initialReloadRate = 55f;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/rocket_reload");

            Item.value = Item.buyPrice(platinum: 1, gold: 6);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "On Hit: Gain up to 10% of maximum health per attack")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "-25% clip size")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
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

            var i = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BlackBoxRocket>(), damage, knockback, player.whoAmI);
            if (player.GetModPlayer<TF2Player>().focus)
            {
                Main.projectile[i].GetGlobalProjectile<TF2ProjectileBase>().homing = true;
                Main.projectile[i].GetGlobalProjectile<TF2ProjectileBase>().shootSpeed = Item.shootSpeed;
                NetMessage.SendData(MessageID.SyncProjectile, number: i);
            }

            if (ammoInClip <= 0)
                reload = true;
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-20f, 0f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DirectHit>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}