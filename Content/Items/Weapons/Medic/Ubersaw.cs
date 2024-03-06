using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Weapons.Medic
{
    public class Ubersaw : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Melee, Unique, Unlock);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65);
            SetWeaponAttackSpeed(0.96);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<TF2Player>().noRandomHealthKits = true;

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (target.type != NPCID.TargetDummy && item.ModItem is TF2Weapon weapon && weapon.GetWeaponMechanic("Medi Gun"))
                    weapon.uberCharge += weapon.uberChargeCapacity / 4f;
            }
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item.ModItem is TF2Weapon weapon && weapon.GetWeaponMechanic("Medi Gun"))
                    weapon.uberCharge += weapon.uberChargeCapacity / 4f;
            }
        }
    }
}