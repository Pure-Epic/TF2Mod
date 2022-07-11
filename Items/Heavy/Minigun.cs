using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Items.Ammo;
using Terraria.DataStructures;
using TF2.Projectiles;
using Terraria.Audio;

namespace TF2.Items.Heavy
{
    public class Minigun : TF2WeaponNoAmmo
    {
        int spinTime = 0;
        private const int maxWindUp = 5;
        public bool shooting => Charge == maxWindUp;

        public int Charge
        {
            get => spinTime;
            set => spinTime = value;
        }

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Heavy's Starter Primary");
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
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.channel = true;

            Item.damage = 9;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.knockBack = -1;
            Item.crit = 0;

            Item.rare = ItemRarityID.White;
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

        public override void HoldItem(Player player)
        {
            if (!player.channel)
            {
                spinTime = 0;
            }
            else
            {
                player.moveSpeed *= 0.53f;
            }
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (spinTime >= 10)
            {
                return true;
            }
            else
            {
                spinTime += 1;
                return false;
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (spinTime >= 10)
            {
                SoundEngine.PlaySound(SoundID.Item11, player.Center);
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
                Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Bullet>(), damage, knockback, player.whoAmI);
            }
            else
            {
                SoundEngine.PlaySound(SoundID.Item13, player.Center);
            }
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 15);
        }
    }
}