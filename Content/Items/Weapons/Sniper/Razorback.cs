using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Weapons.Sniper
{
    public class Razorback : TF2Accessory
    {
        protected override void WeaponStatistics() => SetWeaponCategory(Sniper, Secondary, Unique, Unlock);

        protected override void WeaponDescription(List<TooltipLine> description) => AddPositiveAttribute(description);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            RazorbackPlayer razorbackPlayer = player.GetModPlayer<RazorbackPlayer>();
            razorbackPlayer.razorbackEquipped = true;
            if (Main.netMode == NetmodeID.SinglePlayer)
                player.GetModPlayer<TF2Player>().overhealMultiplier = 0f;
        }
    }

    public class RazorbackPlayer : ModPlayer
    {
        public bool razorbackEquipped;
        public int timer = TF2.Time(30);

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price) => timer = TF2.Time(30);

        public override void PostUpdate()
        {
            if (razorbackEquipped && timer < TF2.Time(30))
                timer++;
        }

        public override void ResetEffects() => razorbackEquipped = false;

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (razorbackEquipped && timer >= TF2.Time(30))
            {
                Dodge();
                return true;
            }
            return false;
        }

        public void Dodge()
        {
            Player.SetImmuneTimeForAllTypes(80);
            if (Player.whoAmI != Main.myPlayer) return;
            timer = 0;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/spy_shield_break"), Player.Center);
            NetMessage.SendData(MessageID.Dodge, -1, -1, null, Player.whoAmI, 2f);
        }
    }
}