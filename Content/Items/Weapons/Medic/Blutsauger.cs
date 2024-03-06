using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles.Medic;

namespace TF2.Content.Items.Weapons.Medic
{
    public class Blutsauger : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Primary, Unique, Unlock);
            SetWeaponSize(50, 25);
            SetGunUseStyle(focus: true, syringeGun: true);
            SetWeaponDamage(damage: 10, projectile: ModContent.ProjectileType<BlutsaugerSyringe>(), projectileSpeed: 25f);
            SetWeaponAttackSpeed(0.105);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/syringegun_shoot");
            SetWeaponAttackIntervals(maxAmmo: 40, maxReserve: 150, reloadTime: 1.305, usesMagazine: true, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/syringegun_reload");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            player.GetModPlayer<BlutsaugerPlayer>().blutsaugerEquipped = true;
            player.GetModPlayer<TF2Player>().noRandomHealthKits = true;
        }
    }

    public class BlutsaugerPlayer : ModPlayer
    {
        public bool blutsaugerEquipped;

        public override void ResetEffects() => blutsaugerEquipped = false;
    }
}