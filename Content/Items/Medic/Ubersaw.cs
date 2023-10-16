using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace TF2.Content.Items.Medic
{
    public class Ubersaw : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Melee, Unique, Unlock);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65);
            SetWeaponAttackSpeed(0.96);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/melee_swing");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => player.GetModPlayer<UbersawPlayer>().ubersawHit = true;

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) => player.GetModPlayer<UbersawPlayer>().ubersawHit = true;
    }

    public class UbersawPlayer : ModPlayer
    {
        public bool ubersawHit;
        public int timer;

        public override void PostUpdate()
        {
            if (ubersawHit)
            {
                timer++;
                if (timer >= 2) // MUST always be 2!
                {
                    ubersawHit = false;
                    timer = 0;
                }
            }
        }
    }
}