using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;

namespace TF2.Content.Items.Weapons.Scout
{
    public class BonkAtomicPunch : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Secondary, Unique, Unlock);
            SetWeaponSize(19, 40);
            SetDrinkUseStyle();
            SetWeaponAttackSpeed(1.2, hide: true);
            SetWeaponAttackSound(SoundID.Item3);
            SetUtilityWeapon();
            SetTimers(TF2.Time(22));
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        public override bool WeaponCanBeUsed(Player player) => timer[0] >= TF2.Time(22);

        protected override void WeaponPassiveUpdate(Player player)
        {
            if (timer[0] < TF2.Time(22))
                timer[0]++;
        }

        protected override bool? WeaponOnUse(Player player)
        {
            player.AddBuff(ModContent.BuffType<Radiation>(), TF2.Time(8));
            timer[0] = 0;
            return true;
        }
    }
}