using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using TF2.Projectiles.Spy;
using Terraria.Audio;

namespace TF2.Items.Spy
{
    public class Knife : TF2WeaponNoAmmo
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Spy's Starter Melee");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 40;
            Item.useStyle = ItemUseStyleID.Rapier; // Makes the player do the proper arm motion
            Item.useAnimation = 48;
            Item.useTime = 48;
            Item.width = 50;
            Item.height = 50;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/knife_swing");
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.autoReuse = true;
            Item.noUseGraphic = true; // The sword is actually a "projectile", so the item should not be visible when used
            Item.noMelee = true; // The projectile will do the damage and not the item

            Item.rare = ItemRarityID.White;

            Item.shoot = ModContent.ProjectileType<KnifeProjectile>(); // The projectile is what makes a shortsword work
            Item.shootSpeed = 2.1f; // This value bleeds into the behavior of the projectile as velocity, keep that in mind when tweaking values
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            if (player.altFunctionUse == 2)
            {
                damage = (int)(240 * p.classMultiplier);
                p.backStab = true;
                player.velocity = velocity * 12.5f;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                player.immuneTime += 24;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

