using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items;
using TF2.Content.Projectiles.Soldier;

namespace TF2.Gensokyo.Content.Items.Soldier
{
    [ExtendsFromMod("Gensokyo")]
    public class OffensiveRocketSystem : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Primary, Unique, Exclusive);
            SetWeaponSize(50, 22);
            SetWeaponOffset(-28.75f);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 45, projectile: ModContent.ProjectileType<Rocket>(), projectileSpeed: 25f, knockback: 5f);
            SetWeaponAttackSpeed(0.26666, 0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/rocket_shoot");
            SetWeaponAttackIntervals(maxAmmo: 6, reloadTime: 0.4, initialReloadTime: 0.46666, reloadSoundPath: "TF2/Content/Sounds/SFX/rocket_reload");
            SetWeaponPrice(weapon: 10);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override bool WeaponCanConsumeAmmo(Player player) => player.itemAnimation >= player.itemAnimationMax - 3 && !reload;
    }
}