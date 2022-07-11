using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Items.Ammo;
using Terraria.DataStructures;
using TF2.Projectiles;
using Terraria.Audio;

namespace TF2.Items.Scout
{
    public class Scattergun : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Scout's Starter Primary");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1f;
            Item.useTime = 37;
            Item.useAnimation = 37;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/scatter_gun_shoot");
            Item.autoReuse = true;

            Item.damage = 6;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.knockBack = -1;
            Item.crit = 0;

            Item.rare = ItemRarityID.White;
            ammoCost = 1;
            maxAmmoClip = 6;
            ammoInClip = 6;
            reloadRate = 30f;
        }

        public override void HoldItem(Player player)
        {
            WeaponSystem clip = player.GetModPlayer<WeaponSystem>();
            clip.ammoMax = maxAmmoClip;
            clip.ammoReloadRate = reloadRate;
            clip.ammoCurrent = ammoInClip;
            UpdateResource();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            WeaponSystem clip = player.GetModPlayer<WeaponSystem>();
            clip.ammoReloadRate = 10f;
            TFClass p = player.GetModPlayer<TFClass>();
            if (p.classAccessory == true && !p.classHideVanity)
            {
                Item.scale = 0f;
            }
            else
            {
                Item.scale = 1f;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            reload = false;
            ammoInClip -= 1;

            for (int i = 0; i < 10; i++)

            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(12));
                Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Bullet>(), damage, knockback, player.whoAmI); //shotguns use this code
            }

            if (ammoInClip <= 0)
            {
                reload = true;
            }
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, 0);
        }
    }
}