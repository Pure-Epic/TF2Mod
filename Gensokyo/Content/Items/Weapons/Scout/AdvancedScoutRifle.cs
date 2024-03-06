using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Weapons;
using TF2.Content.Projectiles;

namespace TF2.Gensokyo.Content.Items.Weapons.Scout
{
    [ExtendsFromMod("Gensokyo")]
    public class AdvancedScoutRifle : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Primary, Unique, Exclusive);
            SetWeaponSize(59, 22);
            SetWeaponOffset(-5f);
            SetGunUseStyle(focus: true, automatic: true);
            SetWeaponDamage(damage: 11, projectile: ModContent.ProjectileType<Bullet>());
            SetWeaponAttackSpeed(0.1);
            SetWeaponAttackSound("TF2/Gensokyo/Content/Sounds/SFX/advancedscoutrifle_shoot");
            SetWeaponAttackIntervals(maxAmmo: 40, maxReserve: 80, reloadTime: 1.1, usesMagazine: true, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/smg_reload");
            SetWeaponPrice(weapon: 10);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponActiveUpdate(Player player) => player.scope = true;

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<AdvancedScoutRiflePlayer>().advancedScoutRifleEquipped = true;
    }

    [ExtendsFromMod("Gensokyo")]
    public class AdvancedScoutRiflePlayer : ModPlayer
    {
        public bool advancedScoutRifleEquipped;

        public override void ResetEffects() => advancedScoutRifleEquipped = false;

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (advancedScoutRifleEquipped)
                modifiers.FinalDamage *= 1.5f;
        }
    }
}