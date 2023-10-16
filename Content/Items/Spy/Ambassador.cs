using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.Spy
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
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/revolver_shoot");
            SetWeaponAttackIntervals(maxAmmo: 6, reloadTime: 1.133, usesMagazine: true, reloadSoundPath: "TF2/Content/Sounds/SFX/revolver_reload");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPostFireProjectile(Player player, int projectile)
        {
            if (player.statLife == player.statLifeMax2)
                Main.projectile[projectile].GetGlobalProjectile<TF2ProjectileBase>().crit = true;
        }
    }
}