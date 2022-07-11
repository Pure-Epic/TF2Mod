using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Items;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace TF2.Items.Ammo
{
    public class PrimaryAmmo : ModItem
    {
        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Large Ammo Box");
			Tooltip.SetDefault("Ammo for Primary Weapons");
			ItemID.Sets.gunProj[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 0;
			Item.width = 14;
			Item.height = 14;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.knockBack = 0f;
			Item.rare = ItemRarityID.White;
			Item.shoot = ProjectileID.Bullet;
			Item.ammo = Item.type; // The first item in an ammo class sets the AmmoID to it's type
			Item.value = Item.buyPrice(copper: 1);
		}

		public override bool OnPickup(Player player)
		{
			SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/ammo_pickup"), player.Center);
			return true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.Lerp(lightColor, Color.White, 0.4f);
		}
	}

	public class SecondaryAmmo : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Small Ammo Box");
			Tooltip.SetDefault("Ammo for Secondary Weapons");
			ItemID.Sets.gunProj[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 0;
			Item.width = 14;
			Item.height = 14;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.knockBack = 0f;
			Item.rare = ItemRarityID.White;
			Item.shoot = ProjectileID.Bullet;
			Item.ammo = Item.type; // The first item in an ammo class sets the AmmoID to it's type
			Item.value = Item.buyPrice(copper: 1);
		}

		public override bool OnPickup(Player player)
		{
			SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/ammo_pickup"), player.Center);
			return true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.Lerp(lightColor, Color.White, 0.4f);
		}
	}
}
