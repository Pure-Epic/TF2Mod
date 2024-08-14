using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class Equalizer : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Melee, Unique, Unlock);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 33);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/pickaxe_swing");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddHeader(description);
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponEarlyUpdate(Player player) => Item.damage = TF2.Round(107.25f - 0.37295f * TF2Player.GetCurrentPlayerHealth(player) * 200f);

        protected override void WeaponActiveUpdate(Player player) => Item.damage = TF2.Round(107.25f - 0.37295f * TF2Player.GetCurrentPlayerHealth(player) * 200f);

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<EqualizerPlayer>().equalizerEquipped = true;
    }

    public class EqualizerPlayer : ModPlayer
    {
        public bool equalizerEquipped;

        public override void ResetEffects() => equalizerEquipped = false;

        public override void PostUpdate()
        {
            if (equalizerEquipped && Main.netMode == NetmodeID.SinglePlayer)
                Player.GetModPlayer<TF2Player>().healReduction *= 0.1f;
        }
    }
}