using Terraria;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Heavy;

namespace TF2.Content.Items.Weapons.Heavy
{
    public class Fists : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Melee, Stock, Starter);
            SetLungeUseStyle();
            SetWeaponDamage(damage: 65, projectile: ModContent.ProjectileType<FistProjectile>(), projectileSpeed: 2f);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponAttackIntervals(altClick: true);
        }

        protected override void WeaponAttackAnimation(Player player) => Item.noUseGraphic = true;
    }
}