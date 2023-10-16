using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Items.Demoman
{
    public class Eyelander : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Melee, Unique, Unlock);
            SetSwingUseStyle(sword: true);
            SetWeaponDamage(damage: 65, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/demo_sword_swing1");
            SetWeaponAttackIntervals(deploy: 0.875, holster: 0.875);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddSwordAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override bool WeaponResetHolsterTimeCondition(Player player) => player.controlUseItem;

        protected override void WeaponPassiveUpdate(Player player)
        {
            int baseLife = (int)(player.statLifeMax2 * 0.85714285714f);
            player.statLifeMax2 = (int)(baseLife + (baseLife * player.GetModPlayer<EyelanderPlayer>().heads * 0.1f));
            player.GetModPlayer<EyelanderPlayer>().eyelanderInInventory = true;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.crit || p.critMelee)
            {
                player.GetModPlayer<EyelanderPlayer>().heads += 1;
                player.Heal((int)(0.1f * player.statLifeMax2));
                if (p.critMelee)
                    p.crit = true;
                player.ClearBuff(ModContent.BuffType<MeleeCrit>());
            }
            else
                modifiers.DisableCrit();
        }

        public override void ModifyHitPvp(Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.crit || p.critMelee)
            {
                player.GetModPlayer<EyelanderPlayer>().heads += 1;
                player.Heal((int)(0.1f * player.statLifeMax2));
                if (p.critMelee)
                    p.crit = true;
                player.ClearBuff(ModContent.BuffType<MeleeCrit>());
            }
        }
    }

    public class EyelanderPlayer : ModPlayer
    {
        public int heads;
        public bool eyelanderInInventory;

        public override void ResetEffects() => eyelanderInInventory = false;

        public override void UpdateDead()
        {
            if (heads > 0)
                Player.statLifeMax2 -= heads * (int)(Player.statLifeMax2 * 0.1f);
            heads = 0;
        }

        public override void OnRespawn() => heads = 0;

        public override void PostUpdate()
        {
            if (eyelanderInInventory)
            {
                heads = Utils.Clamp(heads, 0, 4);
                TF2Weapon.SetPlayerSpeed(Player, 100 + (10 * heads));
            }
        }
    }
}