using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.NPCs.Buddies;

namespace TF2.Content.Buffs
{
    public class DisciplinaryActionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            double speed = player.GetModPlayer<TF2Player>().currentClass switch
            {
                TF2Item.Scout => 126.3,
                TF2Item.Soldier or TF2Item.Heavy => 140,
                TF2Item.Pyro or TF2Item.Engineer or TF2Item.Sniper or TF2Item.Spy => 135,
                TF2Item.Demoman => 137.5,
                TF2Item.Medic => 132.8,
                _ => 125
            };
            TF2Player.SetPlayerSpeed(player, speed);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.ModNPC is Buddy buddy)
            {
                double speed = buddy switch
                {
                    ScoutNPC => 126.3,
                    SoldierNPC or HeavyNPC => 140,
                    PyroNPC or EngineerNPC or SniperNPC or SpyNPC => 135,
                    DemomanNPC => 137.5,
                    MedicNPC => 132.8,
                    _ => 125
                };
                Buddy.SetBuddySpeed(buddy, speed);
            }
        }
    }

    public class WhippedPlayer : ModPlayer
    {
        private bool playBuffSound;

        public override void PostUpdate()
        {
            if (Player.HasBuff<DisciplinaryActionBuff>())
                playBuffSound = true;
            else if (playBuffSound)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/disciplinary_action_power_down"), Player.Center);
                playBuffSound = false;
            }
        }
    }
}