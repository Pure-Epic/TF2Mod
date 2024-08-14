using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Weapons.Demoman
{
    public class Eyelander : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Melee, Unique, Unlock);
            SetSwingUseStyle(sword: true);
            SetWeaponDamage(damage: 65, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/demo_sword_swing1");
            SetWeaponAttackIntervals(deploy: 1, holster: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddNegativeAttribute(description);
            AddSwordAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override void WeaponAttackAnimation(Player player)
        {
            float duration = 1f - (float)player.itemAnimation / player.itemAnimationMax;
            float start = TF2.SwordRotation(player, -90f);
            float end = player.direction == 1 ? TF2.SwordRotation(player, 90f) : TF2.SwordRotation(player, -270f);
            float currentAngle = MathHelper.Lerp(start, end, duration);
            player.itemRotation = player.direction > 0 ? (currentAngle + (player.gravDir >= 0 ? MathHelper.PiOver4 : (MathHelper.PiOver4 * 7))) : currentAngle + (player.gravDir >= 0 ? (MathHelper.PiOver4 * 3) : (MathHelper.PiOver4 * 5));
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.gravDir >= 0 ? (currentAngle - MathHelper.PiOver2) : (-currentAngle - MathHelper.PiOver2));
            player.itemLocation = player.MountedCenter + Vector2.UnitX.RotatedBy(currentAngle);
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            TF2Player.SetPlayerHealth(player, player.GetModPlayer<EyelanderPlayer>().heads * 15 - 25);
            EyelanderPlayer p = player.GetModPlayer<EyelanderPlayer>();
            p.eyelanderEquipped = true;
            TF2Player.SetPlayerSpeed(player, 100 + (10 * p.heads));
        }

        protected override void WeaponHitPlayer(Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            if (player.GetModPlayer<TF2Player>().crit)
            {
                player.GetModPlayer<EyelanderPlayer>().heads++;
                player.Heal(TF2.GetHealth(player, 15));
            }
        }

        protected override void WeaponHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (player.GetModPlayer<TF2Player>().crit)
            {
                player.GetModPlayer<EyelanderPlayer>().heads++;
                player.Heal(TF2.GetHealth(player, 15));
            }
        }
    }

    public class EyelanderPlayer : ModPlayer
    {
        public int heads;
        public bool eyelanderEquipped;

        public override void ResetEffects() => eyelanderEquipped = false;

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            TF2Player.SetPlayerHealth(Player, Player.GetModPlayer<EyelanderPlayer>().heads * -15);
            heads = 0;
        }

        public override void OnRespawn() => heads = 0;

        public override void PreUpdate()
        {
            if (eyelanderEquipped)
                heads = Utils.Clamp(heads, 0, 4);
        }
    }
}