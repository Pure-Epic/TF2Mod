using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.NPCs.Buildings.SentryGun;
using TF2.Content.Projectiles.Engineer;

namespace TF2.Content.Items.Weapons.Engineer
{
    public class Wrangler : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Engineer, Secondary, Unique, Unlock);
            SetWeaponSize(41, 50);
            SetGunUseStyle();
            SetWeaponDamage(projectile: ModContent.ProjectileType<WranglerBeam>(), projectileSpeed: 1f);
            SetWeaponAttackSpeed(0.01666, hide: true);
            SetWeaponAttackIntervals(altClick: true, noAmmo: true);
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        protected override bool WeaponCanAltClick(Player player)
        {
            int sentry = player.GetModPlayer<TF2Player>().sentryWhoAmI;
            return sentry > -1 && Main.npc[sentry].ModNPC is SentryLevel3;
        }
    }
}