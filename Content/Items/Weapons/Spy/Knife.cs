using System.Collections.Generic;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Spy;

namespace TF2.Content.Items.Weapons.Spy
{
    public class Knife : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Spy, Melee, Stock, Starter);
            SetLungeUseStyle(knife: true);
            SetWeaponDamage(damage: 40, projectile: ModContent.ProjectileType<KnifeProjectile>(), projectileSpeed: 2f, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/knife_swing");
            SetKnife();
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);
    }
}