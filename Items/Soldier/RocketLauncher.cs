using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Items.Ammo;
using Terraria.DataStructures;
using TF2.Projectiles.Soldier;
using Terraria.Audio;

namespace TF2.Items.Soldier
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
			Item.width = 40;
			Item.height = 40;
			Item.scale = 1f;
			Item.useTime = 48;
			Item.useAnimation = 48;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.UseSound = new SoundStyle("TF2/Sounds/SFX/rocket_shoot");
			Item.autoReuse = true;

			Item.damage = 90;
			Item.shoot = ModContent.ProjectileType<Rocket>();
			Item.shootSpeed = 25f;
			Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
			Item.knockBack = -1;
			Item.crit = 0;

			Item.rare = ItemRarityID.White;
			ammoCost = 1;
			maxAmmoClip = 4;
			ammoInClip = 4;
			reloadRate = 48f;
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

			Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Rocket>(), damage, knockback, player.whoAmI);

			if (ammoInClip <= 0)
			{
				reload = true;
			}
			return false;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-25, 0);
		}
	}
}