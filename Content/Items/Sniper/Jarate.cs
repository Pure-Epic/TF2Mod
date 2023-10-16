using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Projectiles.Sniper;

namespace TF2.Content.Items.Sniper
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
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/sniper_jaratetoss01");
            SetUtilityWeapon();
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddHeader(description);
            AddPositiveAttribute(description);
            AddNeutralAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player) => !player.GetModPlayer<JaratePlayer>().jarateCooldown;

        protected override bool? WeaponOnUse(Player player)
        {
            player.AddBuff(ModContent.BuffType<JarateCooldown>(), 1200);
            return true;
        }
    }
}