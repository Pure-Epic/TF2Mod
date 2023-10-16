using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Ammo
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
            Item.knockBack = 0;
            Item.rare = ItemRarityID.White;
            Item.shoot = ProjectileID.Bullet;
            Item.ammo = Item.type; // The first item in an ammo class sets the AmmoID to it's type
            Item.value = Item.buyPrice(copper: 1);
        }

        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup"), player.Center);
            if (player.GetModPlayer<TF2Player>().invisWatchEquipped)
            {
                player.GetModPlayer<Buffs.CloakPlayer>().cloakMeter = 600;
            }
            if (player.GetModPlayer<TF2Player>().cloakandDaggerEquipped && !player.HasBuff<Buffs.CloakandDagger>())
            {
                player.GetModPlayer<Buffs.CloakandDaggerPlayer>().cloakMeter += 390;
            }
            return true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);
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
            Item.knockBack = 0;
            Item.rare = ItemRarityID.White;
            Item.shoot = ProjectileID.Bullet;
            Item.ammo = Item.type; // The first item in an ammo class sets the AmmoID to it's type
            Item.value = Item.buyPrice(copper: 1);
        }

        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup"), player.Center);
            if (player.GetModPlayer<TF2Player>().invisWatchEquipped)
            {
                player.GetModPlayer<Buffs.CloakPlayer>().cloakMeter = 600;
            }
            if (player.GetModPlayer<TF2Player>().cloakandDaggerEquipped && !player.HasBuff<Buffs.CloakandDagger>())
            {
                player.GetModPlayer<Buffs.CloakandDaggerPlayer>().cloakMeter += 390;
            }
            return true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);
    }
}
