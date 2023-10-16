using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Ammo;
using TF2.Content.Projectiles.Soldier;

namespace TF2.Content.Items.Soldier
{
    public class RocketLauncher : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rocket Launcher");
            Tooltip.SetDefault("Soldier's Starter Primary");

            ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Item.type] = true;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 21;
            Item.useTime = 48;
            Item.useAnimation = 48;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/rocket_shoot");
            Item.autoReuse = true;

            Item.damage = 90;
            Item.shoot = ModContent.ProjectileType<Rocket>();
            Item.shootSpeed = 25f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.knockBack = 20f;

            ammoCost = 1;
            maxAmmoClip = 4;
            ammoInClip = 4;
            reloadRate = 48f;
            initialReloadRate = 55f;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/rocket_reload");

            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);
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

            var i = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Rocket>(), damage, knockback, player.whoAmI);
            if (player.GetModPlayer<TF2Player>().focus)
            {
                Main.projectile[i].GetGlobalProjectile<Projectiles.TF2ProjectileBase>().homing = true;
                Main.projectile[i].GetGlobalProjectile<Projectiles.TF2ProjectileBase>().shootSpeed = Item.shootSpeed;
                NetMessage.SendData(MessageID.SyncProjectile, number: i);
            }

            if (ammoInClip <= 0)
                reload = true;
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-25f, 0f);
    }
}