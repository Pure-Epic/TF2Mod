﻿using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Items.Weapons.Pyro
{
    public class Axtinguisher : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Melee, Unique, Unlock);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 44, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponAttackIntervals(holster: 0.675);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }


        protected override void WeaponHitPlayer(Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            if (target.HasBuff(ModContent.BuffType<PyroFlames>()))
            {
                player.GetModPlayer<TF2Player>().miniCrit = true;
                int buffIndex = target.FindBuffIndex(ModContent.BuffType<PyroFlames>());
                modifiers.SourceDamage.Base = (int)((44f + 8f * target.buffTime[buffIndex] / 60f) * player.GetModPlayer<TF2Player>().damageMultiplier);
                target.buffTime[buffIndex] = 0;
            }
            else if (target.HasBuff(ModContent.BuffType<PyroFlamesDegreaser>()))
            {
                player.GetModPlayer<TF2Player>().miniCrit = true;
                int buffIndex = target.FindBuffIndex(ModContent.BuffType<PyroFlamesDegreaser>());
                modifiers.SourceDamage.Base = (int)((44f + 8f * target.buffTime[buffIndex] / 60f) * player.GetModPlayer<TF2Player>().damageMultiplier);
                target.buffTime[buffIndex] = 0;
            }
        }

        protected override void WeaponHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.HasBuff(ModContent.BuffType<PyroFlames>()))
            {
                player.GetModPlayer<TF2Player>().miniCrit = true;
                int buffIndex = target.FindBuffIndex(ModContent.BuffType<PyroFlames>());
                modifiers.SourceDamage.Base = TF2.Round(8f * target.buffTime[buffIndex] / 60f * player.GetModPlayer<TF2Player>().damageMultiplier);
                target.buffTime[buffIndex] = 0;
            }
            else if (target.HasBuff(ModContent.BuffType<PyroFlamesDegreaser>()))
            {
                player.GetModPlayer<TF2Player>().miniCrit = true;
                int buffIndex = target.FindBuffIndex(ModContent.BuffType<PyroFlamesDegreaser>());
                modifiers.SourceDamage.Base = TF2.Round(8f * target.buffTime[buffIndex] / 60f * player.GetModPlayer<TF2Player>().damageMultiplier);
                target.buffTime[buffIndex] = 0;
            }
        }

    }
}