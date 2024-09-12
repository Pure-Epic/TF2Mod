using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Buffs
{
    public class KritzkriegUberCharge : ModBuff
    {
        public override string Texture => "TF2/Content/Buffs/UberCharge";

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<UberChargeNPC>().uberCharge = true;

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<KritzkriegUberChargePlayer>().uberCharge = true;
            player.GetModPlayer<TF2Player>().crit = true;
        }

        public override bool RightClick(int buffIndex)
        {
            TF2Player p = Main.LocalPlayer.GetModPlayer<TF2Player>();
            p.uberChargeTime = 0;
            p.uberCharge = 0;
            p.activateUberCharge = false;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/invulnerable_off"), Main.LocalPlayer.Center);
            return true;
        }
    }

    public class KritzkriegUberChargePlayer : UberChargePlayer
    {
        public override void PostUpdate()
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (uberCharge)
                SpawnDusts(Player);
            if (p.activateUberCharge && Player.HasBuff<KritzkriegUberCharge>())
            {
                p.uberChargeTime++;
                p.uberChargeDuration = uberChargeDuration;
                if (p.uberChargeTime >= uberChargeDuration)
                {
                    p.uberChargeTime = 0;
                    p.uberCharge = 0;
                    p.activateUberCharge = false;
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/invulnerable_off"), Player.Center);
                }
            }
        }
    }
}