using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Engineer;

namespace TF2.Content.Items.Weapons.Engineer
{
    public class Wrench : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Engineer, Melee, Stock, Starter);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/wrench_swing");
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        protected override bool? WeaponOnUse(Player player)
        {
            TF2.CreateProjectile(this, player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<WrenchHitbox>(), 0, 0f);
            return true;
        }
    }
}