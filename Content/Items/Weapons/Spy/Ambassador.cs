using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.Weapons.Spy
{
    public class Ambassador : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Spy, Secondary, Unique, Unlock);
            SetWeaponSize(50, 24);
            SetGunUseStyle(focus: true, automatic: true);
            SetWeaponDamage(damage: 34, projectile: ModContent.ProjectileType<Bullet>(), noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.6);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/ambassador_shoot");
            SetWeaponAttackIntervals(maxAmmo: 6, maxReserve: 24, reloadTime: 1.133, usesMagazine: true, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/revolver_reload");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPostFireProjectile(Player player, int projectile)
        {
            if (TF2Player.IsHealthFull(player))
                (Main.projectile[projectile].ModProjectile as TF2Projectile).crit = true;
        }
    }
}