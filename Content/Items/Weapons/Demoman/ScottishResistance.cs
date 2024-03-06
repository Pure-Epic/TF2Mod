using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Demoman;

namespace TF2.Content.Items.Weapons.Demoman
{
    public class ScottishResistance : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Secondary, Unique, Unlock);
            SetWeaponSize(50, 28);
            SetGunUseStyle(stickybombLauncher: true);
            SetWeaponDamage(damage: 120, projectile: ModContent.ProjectileType<ScottishResistanceStickybomb>(), knockback: 5f);
            SetWeaponAttackSpeed(0.45);
            SetWeaponAttackIntervals(maxAmmo: 8, maxReserve: 36, reloadTime: 0.67, initialReloadTime: 1.09, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/stickybomblauncher_reload");
            SetStickybombLauncher(capacity: 14, detonationTime: 1.72, maxCharge: 4);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public void ReturnStickybombs(Player player)
        {
            stickybombsAmount -= player.GetModPlayer<ScottishResistancePlayer>().stickybombsReturned;
            player.GetModPlayer<ScottishResistancePlayer>().stickybombsReturned = 0;
        }
    }

    public class ScottishResistancePlayer : ModPlayer
    {
        public int stickybombsReturned;
    }
}