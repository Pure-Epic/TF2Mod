using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Scout;

namespace TF2.Content.Items.Weapons.Scout
{
    public class Sandman : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Melee, Unique, Unlock);
            SetSwingUseStyle(focus: true);
            SetWeaponDamage(damage: 35);
            SetWeaponAttackSpeed(0.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponAttackIntervals(altClick: true);
            SetTimers(TF2.Time(10));
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player) => !(player.controlUseTile && timer[0] < TF2.Time(10));

        protected override void WeaponActiveUpdate(Player player)
        {
            if (player.controlUseTile && timer[0] >= TF2.Time(10) && timer[2] <= 0)
            {
                player.itemAnimation = player.itemAnimationMax = TF2.Time(0.5);
                timer[1] = TF2.Time(0.5);
                timer[2] = TF2.Time(0.25);
                int newDamage = TF2.Round(15 * player.GetModPlayer<TF2Player>().classMultiplier);
                TF2Projectile projectile = TF2.CreateProjectile(this, player.GetSource_ItemUse(Item), player.Center, player.DirectionTo(Main.MouseWorld) * 25f, ModContent.ProjectileType<Baseball>(), newDamage, 0f, player.whoAmI);               
                if (player.GetModPlayer<TF2Player>().focus)
                {
                    projectile.homing = true;
                    projectile.shootSpeed = Item.shootSpeed;
                    projectile.Projectile.netUpdate = true;
                }
                TF2.SetPlayerDirection(player);
                timer[0] = 0;
            }
            Item.noMelee = timer[1] > 0;
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            TF2Player.SetPlayerHealth(player, -15);
            if (timer[0] < TF2.Time(10))
                timer[0]++;
            if (timer[1] > 0)
                timer[1]--;
            if (timer[2] > 0)
                timer[2]--;
        }

        public override void WeaponDistanceModifier(Player player, Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            bool normalRange = Vector2.Distance(player.Center, target.Center) / 1000f < 1f;
            modifiers.FinalDamage *= normalRange ? 1f : 1.5f;
            if (!normalRange)
                (projectile.ModProjectile as Baseball).moonShot = true;
        }
    }
}