using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Pyro;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Demoman
{
    public class ScotsmansSkullcutter : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Melee, Unique, Craft);
            SetSwingUseStyle(sword: true);
            SetWeaponDamage(damage: 78);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/demo_sword_swing1");
            SetWeaponAttackIntervals(deploy: 1, holster: 1);
            SetWeaponPrice(weapon: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddSwordAttribute(description);
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponAttackAnimation(Player player)
        {
            float duration = 1f - (float)player.itemAnimation / player.itemAnimationMax;
            float start = player.direction == 1 ? TF2.SwordRotation(player, -180f) : TF2.SwordRotation(player, 0f);
            float end = player.direction == 1 ? TF2.SwordRotation(player, 0f) : TF2.SwordRotation(player, -180f);
            float currentAngle = MathHelper.Lerp(start, end, duration);
            player.itemRotation = player.direction > 0 ? (currentAngle + (player.gravDir >= 0 ? MathHelper.PiOver4 : (MathHelper.PiOver4 * 7))) : currentAngle + (player.gravDir >= 0 ? (MathHelper.PiOver4 * 3) : (MathHelper.PiOver4 * 5));
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.gravDir >= 0 ? (currentAngle - MathHelper.PiOver2) : (-currentAngle - MathHelper.PiOver2));
            player.itemLocation = player.MountedCenter + Vector2.UnitX.RotatedBy(currentAngle);
        }

        protected override void WeaponPassiveUpdate(Player player) => TF2Player.SetPlayerSpeed(player, 85);

        public override void AddRecipes() => CreateRecipe().AddIngredient<Jarate>().AddIngredient<Axtinguisher>().AddTile<CraftingAnvil>().Register();
    }
}