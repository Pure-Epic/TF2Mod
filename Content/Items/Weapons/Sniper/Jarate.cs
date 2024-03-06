using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Sniper;

namespace TF2.Content.Items.Weapons.Sniper
{
    public class Jarate : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Sniper, Secondary, Unique, Unlock);
            SetWeaponSize();
            SetThrowableUseStyle();
            SetWeaponDamage(projectile: ModContent.ProjectileType<JarateProjectile>());
            SetWeaponAttackSpeed(0.5, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Voicelines/sniper_jaratetoss");
            SetUtilityWeapon(itemUseGraphic: false);
            SetTimers(TF2.Time(20));
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddNeutralAttribute(description);
            AddPositiveAttribute(description);
            AddLore(description);
        }

        public override bool WeaponCanBeUsed(Player player) => timer[0] >= TF2.Time(20);

        protected override void WeaponPassiveUpdate(Player player)
        {
            if (timer[0] < TF2.Time(20))
                timer[0]++;
        }

        protected override bool? WeaponOnUse(Player player)
        {
            timer[0] = 0;
            return true;
        }
    }
}