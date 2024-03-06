using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.NPCs;

namespace TF2.Content.Items.Weapons.Engineer
{
    public class DestructionPDA : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Engineer, PDA2, Stock, Starter);
            SetWeaponSize(50, 50);
            SetPDAUseStyle();
            SetWeaponAttackSpeed(1, hide: true);
            SetWeaponAttackIntervals(altClick: true, noAmmo: true);
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        protected override bool? WeaponOnUse(Player player)
        {
            int sentry = player.GetModPlayer<EngineerBuildings>().sentryWhoAmI;
            int dispenser = player.GetModPlayer<EngineerBuildings>().dispenserWhoAmI;
            if (player.altFunctionUse != 2)
            {
                if (Main.netMode != NetmodeID.SinglePlayer) return false;
                if (sentry != -1 && sentry <= Main.npc.Length && Main.npc[sentry].ModNPC is Sentry)
                {
                    Main.npc[sentry].ModNPC.NPC.active = false;
                    player.GetModPlayer<EngineerBuildings>().sentryWhoAmI = -1;
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/sentry_explode"), player.Center);
                }
            }
            else
            {
                if (Main.netMode != NetmodeID.SinglePlayer) return false;
                if (dispenser != -1 && dispenser <= Main.npc.Length && Main.npc[dispenser].ModNPC is Dispenser)
                {
                    Main.npc[dispenser].ModNPC.NPC.active = false;
                    player.GetModPlayer<EngineerBuildings>().dispenserWhoAmI = -1;
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/dispenser_explode"), player.Center);
                }
            }
            return true;
        }
    }
}