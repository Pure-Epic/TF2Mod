using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TF2.Common;
using TF2.Content.Projectiles.Medic;

namespace TF2.Content.Buffs
{
    public class UberCharge : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<UberChargeNPC>().uberCharge = true;

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<UberChargePlayer>().uberCharge = true;

        public override bool RightClick(int buffIndex)
        {
            TF2Player uber = Main.LocalPlayer.GetModPlayer<TF2Player>();
            uber.uberChargeTime = 0;
            uber.uberCharge = 0;
            uber.activateUberCharge = false;
            return true;
        }
    }

    public class UberChargePlayer : ModPlayer
    {
        public bool uberCharge;
        public int uberChargeDuration = 480;

        public override void ResetEffects()
        {
            uberCharge = false;
            Player.creativeGodMode = false;
        }

        public override void PostUpdate()
        {
            TF2Player uber = Player.GetModPlayer<TF2Player>();
            if (uberCharge)
            {
                Player.creativeGodMode = true;
                SpawnDusts(Player);
            }
            if (uber.activateUberCharge && Player.HasBuff<UberCharge>())
            {
                uber.uberChargeTime++;
                uber.uberChargeDuration = uberChargeDuration;
                if (uber.uberChargeTime >= uberChargeDuration)
                {
                    uber.uberChargeTime = 0;
                    uber.uberCharge = 0;
                    uber.activateUberCharge = false;
                }
            }
        }

        protected static void SpawnDusts(Player player)
        {
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(player.position, 8, 8, DustID.Clentaminator_Red, 0.0f, 0.0f, 100, new Color(), 1.5f)];
                dust.velocity *= 0.5f;
                dust.velocity.Y = -Math.Abs(dust.velocity.Y);
            }
        }
    }

    public class UberChargeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int timer = 0;
        public bool uberCharge;

        public override void ResetEffects(NPC npc)
        {
            uberCharge = false;
            npc.immortal = false;
        }

        public override void AI(NPC npc)
        {
            if (uberCharge)
            {
                if (npc.HasBuff<UberCharge>())
                    npc.immortal = true;
                SpawnDusts(npc);
            }
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) => binaryWriter.Write(timer);

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) => timer = binaryReader.ReadInt32();

        protected static void SpawnDusts(NPC npc)
        {
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(npc.position, 8, 8, DustID.Clentaminator_Red, 0.0f, 0.0f, 100, new Color(), 1.5f)];
                dust.velocity *= 0.5f;
                dust.velocity.Y = -Math.Abs(dust.velocity.Y);
            }
        }
    }
}