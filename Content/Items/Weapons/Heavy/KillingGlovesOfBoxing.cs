using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Heavy;

namespace TF2.Content.Items.Weapons.Heavy
{
    public class KillingGlovesOfBoxing : TF2Weapon
    {
        protected override string ArmTexture => "TF2/Content/Textures/Items/Heavy/KillingGlovesOfBoxing";

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Melee, Unique, Unlock);
            SetLungeUseStyle();
            SetWeaponDamage(damage: 65, projectile: ModContent.ProjectileType<KillingGlovesOfBoxingProjectile>(), projectileSpeed: 2f);
            SetWeaponAttackSpeed(0.96);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponAttackIntervals(altClick: true);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponAttackAnimation(Player player) => Item.noUseGraphic = true;

        protected override bool WeaponAddTextureCondition(Player player) => HoldingWeapon<KillingGlovesOfBoxing>(player);
    }
}