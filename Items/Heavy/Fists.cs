using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using TF2.Projectiles.Heavy;
using Terraria.Audio;

namespace TF2.Items.Heavy
{
    public class Fists : TF2WeaponNoAmmo
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Heavy's Starter Melee");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 65;
            Item.useStyle = ItemUseStyleID.Rapier; // Makes the player do the proper arm motion
            Item.useAnimation = 48;
            Item.useTime = 48;
            Item.width = 50;
            Item.height = 50;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/melee_swing");
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.autoReuse = true;
            Item.noUseGraphic = true; // The sword is actually a "projectile", so the item should not be visible when used
            Item.noMelee = true; // The projectile will do the damage and not the item

            Item.rare = ItemRarityID.White;

            Item.shoot = ModContent.ProjectileType<FistProjectile>(); // The projectile is what makes a shortsword work
            Item.shootSpeed = 2.1f; // This value bleeds into the behavior of the projectile as velocity, keep that in mind when tweaking values
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}

