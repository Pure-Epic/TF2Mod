using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Items.Ammo;
using Terraria.DataStructures;
using TF2.Projectiles.Demoman;
using Terraria.Audio;

namespace TF2.Items.Demoman
{
    public class GrenadeLauncher : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grenade Launcher");
            Tooltip.SetDefault("Demoman's Starter Primary");
            ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Item.type] = true;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1f;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/grenade_launcher_shoot");
            Item.autoReuse = true;

            Item.damage = 100; //Damage set on projectile
            Item.shoot = ModContent.ProjectileType<Grenade>();
            Item.shootSpeed = 12.5f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.knockBack = 20;
            Item.rare = ItemRarityID.White;

            ammoCost = 1;
            maxAmmoClip = 4;
            ammoInClip = 4;
            reloadRate = 36f;
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

            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Grenade>(), damage, knockback, player.whoAmI);

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