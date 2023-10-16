using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Ammo;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Pyro;

namespace TF2.Content.Items.Pyro
{
    public class FlareGun : TF2WeaponNoAmmo
    {
        public int cooldown;
        public bool finishReloadSound;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare Gun");
            Tooltip.SetDefault("Pyro's Unlocked Secondary");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/flaregun_shoot");
            Item.autoReuse = true;

            Item.damage = 30;
            Item.shoot = ModContent.ProjectileType<Flare>();
            Item.shootSpeed = 25f;
            Item.useAmmo = ModContent.ItemType<SecondaryAmmo>();

            ammoCost = 1;
            ammoInClip = 1;
            maxAmmoClip = 1;
            reloadRate = 1f;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/flaregun_reload");

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Speed" && x.Mod == "Terraria");
            if (tt != null)
            {
                tt.Text = Language.GetTextValue("Snail speed");
            }
            TooltipLine tt2 = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt2);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "100% critical hit vs burning players")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Neutral Attributes",
                "This weapon will reload when not active")
            {
                OverrideColor = new Color(255, 255, 255)
            };
            tooltips.Add(line2);
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.ModItem is not FlareGun)
                cooldown--;
        }

        public override void HoldItem(Player player)
        {
            WeaponSystem clip = player.GetModPlayer<WeaponSystem>();
            clip.ammoMax = maxAmmoClip;
            clip.ammoReloadRate = reloadRate;
            clip.ammoCurrent = ammoInClip;
            UpdateResource();

            if (reload && !finishReloadSound && cooldown == 100)
            {
                SoundEngine.PlaySound(reloadSound, Main.LocalPlayer.Center);
                finishReloadSound = true;
            }

            if (!reload)
                finishReloadSound = false;

            cooldown--;
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
            cooldown = 120;

            var i = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Flare>(), damage, knockback, player.whoAmI);
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

        public override bool CanUseItem(Player player) => cooldown <= 0;
    }
}