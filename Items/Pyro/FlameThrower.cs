using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Projectiles;
using TF2.Items.Ammo;
using Terraria.DataStructures;
using TF2.Projectiles.Pyro;
using Terraria.Audio;

namespace TF2.Items.Pyro
{
    public class FlameThrower : TF2WeaponNoAmmo
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame Thrower");
            Tooltip.SetDefault("Pyro's Starter Secondary");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1f;
            Item.useTime = 6;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = null;
            Item.autoReuse = true;

            Item.damage = 78;
            Item.shoot = ModContent.ProjectileType<Fire>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.knockBack = -1;
            Item.crit = 0;
            Item.mana = 25;

            Item.rare = ItemRarityID.White;
        }
        public override void HoldItem(Player player)
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
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shootSpeed = 25f;
                Item.scale = 1f;
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.useTime = 45;
                Item.useAnimation = 45;
                Item.reuseDelay = 6;
                Item.damage = 1;
                Item.shoot = ModContent.ProjectileType<Airblast>();
            }
            else
            {
                Item.shootSpeed = 10f;
                Item.scale = 1f;
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.useTime = 6;
                Item.useAnimation = 30;
                Item.reuseDelay = 6;
                Item.damage = 78;
                Item.shoot = ModContent.ProjectileType<Fire>();
            }
            return base.CanUseItem(player);
        }
        public override bool AltFunctionUse(Player player)
        {
            //item.scale = 1f;
            //item.useStyle = ItemUseStyleID.HoldingOut;
            //item.useTime = 45;
            //item.useAnimation = 45;
            //item.reuseDelay = 45;
            //item.damage = 0;
            //item.shoot = ProjectileID.Beenade;
            return true;
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse != 2)
            {
                reduce -= 25;
            }
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (player.altFunctionUse != 2)
            {
                //To make this item only consume ammo during the first jet, we check to make sure the animation just started. ConsumeAmmo is called 5 times because of item.useTime and item.useAnimation values in SetDefaults above.
                return player.itemAnimation >= player.itemAnimationMax - 5;
            }
            else
            {
                //if (player.statMana >= 25)
                //{
                    //player.statMana -= 25;
                    //return true;
                //}
                return false;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                Vector2 muzzleOffset = Vector2.Normalize(velocity) * 54f; //This gets the direction of the flame projectile, makes its length to 1 by normalizing it. It then multiplies it by 54 (the item width) to get the position of the tip of the flamethrower.
                if (Collision.CanHit(position, 6, 6, position + muzzleOffset, 6, 6))
                {
                    position += muzzleOffset;
                    SoundEngine.PlaySound(SoundID.Item34, player.Center);
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Fire>(), damage, knockback, player.whoAmI);

                }
                // This is to prevent shooting through blocks and to make the fire shoot from the muzzle.
                //return true;
                //}
                return false;
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/flame_thrower_airblast"), player.Center);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Airblast>(), damage, knockback, player.whoAmI);
                return false;
            }
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, 0);
        }
    }
}