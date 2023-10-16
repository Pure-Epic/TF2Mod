using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Projectiles.Scout;

namespace TF2.Content.Items.Scout
{
    public class Sandman : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Melee, Unique, Unlock);
            SetSwingUseStyle(focus: true);
            SetWeaponDamage(damage: 35);
            SetWeaponAttackSpeed(0.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/melee_swing");
            SetWeaponAttackIntervals(altClick: true);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player) => !(player.controlUseTile && player.GetModPlayer<BaseballPlayer>().baseballCooldown);

        protected override void WeaponActiveUpdate(Player player)
        {
            if (player.controlUseTile && !player.GetModPlayer<BaseballPlayer>().baseballCooldown)
            {
                int newDamage = (int)(15 * Main.LocalPlayer.GetModPlayer<TF2Player>().classMultiplier);
                FocusShot(player, player.GetSource_ItemUse(Item), player.Center, player.DirectionTo(Main.MouseWorld) * 25f, ModContent.ProjectileType<Baseball>(), newDamage, 0f, player.whoAmI);
                player.AddBuff(ModContent.BuffType<BaseballCooldown>(), 600);
            }
            Item.noMelee = player.controlUseTile;
        }

        protected override void WeaponPassiveUpdate(Player player) => SetPlayerHealth(player, 88);
    }
}