using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Projectiles;
using TF2.Items.Ammo;
using Terraria.DataStructures;
using Terraria.Audio;


namespace TF2.Items.Sniper
{
	public class SMG : TF2Weapon
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("SMG");
			Tooltip.SetDefault("Sniper's Starter Secondary");
		}

		public override void SafeSetDefaults() 
		{
			Item.width = 40;
			Item.height = 40;
			Item.scale = 1f;
			Item.useTime = 6;
			Item.useAnimation = 6;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.UseSound = new SoundStyle("TF2/Sounds/SFX/smg_shoot");
			Item.autoReuse = true;

			Item.damage = 8;
			Item.shoot = ModContent.ProjectileType<Bullet>();
			Item.shootSpeed = 10f;
			Item.useAmmo = ModContent.ItemType<SecondaryAmmo>();
			Item.knockBack = -1;
			Item.crit = 0;

			Item.rare = ItemRarityID.White;
			ammoCost = 1;
			maxAmmoClip = 25;
			ammoInClip = 25;
			reloadRate = 66;
			magazine = true;
		}
		public override void HoldItem(Player player)
		{
			WeaponSystem clip = player.GetModPlayer<WeaponSystem>();
			clip.ammoMax = maxAmmoClip;
			clip.ammoReloadRate = reloadRate;
			clip.ammoCurrent = ammoInClip;
			UpdateResource();
		}

		public override bool CanConsumeAmmo(Item ammo, Player player)
		{
			return ammoInClip > 0;
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
			if (ammoInClip == 0) {return false;}
			ammoInClip -= 1;
			Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Bullet>(), damage, knockback, player.whoAmI);

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

		public override bool CanUseItem(Player player)
		{
			if (ammoInClip <= 0) { return false; }
			var tf2Player = player.GetModPlayer<WeaponSystem>();
			if (tf2Player.ammoCurrent <= 0)
			{
				tf2Player.ReloadWeapon();
				return false;
			}
			return tf2Player.ammoCurrent >= ammoCost;
		}
	}
}