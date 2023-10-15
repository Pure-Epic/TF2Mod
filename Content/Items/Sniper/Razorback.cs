using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;

namespace TF2.Content.Items.Sniper
{
    public class Razorback : TF2AccessorySecondary
    {
        protected override void WeaponStatistics() => SetWeaponCategory(Sniper, Secondary, Unique, Unlock);

        protected override void WeaponDescription(List<TooltipLine> description) => AddPositiveAttribute(description);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            RazorbackPlayer p = player.GetModPlayer<RazorbackPlayer>();
            p.razorbackEquipped = true;
            if (p.timer >= 1800)
            {
                player.AddBuff(ModContent.BuffType<RazorbackBuff>(), 1800);
                if (p.evadeHit)
                    p.timer = 0;
            }
        }
    }

    public class RazorbackPlayer : ModPlayer
    {
        public bool razorbackEquipped;
        public bool evadeHit;
        public int timer = 1800;

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price) => timer = 1800;

        public override void PostUpdate()
        {
            if (!razorbackEquipped)
                Player.ClearBuff(ModContent.BuffType<RazorbackBuff>());
            else if (timer < 1800)
                timer++;
        }

        public override void ResetEffects()
        {
            razorbackEquipped = false;
            evadeHit = false;
        }

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (evadeHit)
            {
                Dodge();
                return true;
            }
            return false;
        }

        public void Dodge()
        {
            Player.SetImmuneTimeForAllTypes(80);
            if (Player.whoAmI != Main.myPlayer)
                return;
            Player.ClearBuff(ModContent.BuffType<RazorbackBuff>());
            timer = 0;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_shield_break"), Player.Center);
            NetMessage.SendData(MessageID.Dodge, -1, -1, null, Player.whoAmI, 2f);
        }
    }
}