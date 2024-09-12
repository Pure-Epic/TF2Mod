using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TF2.Common;

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
            TF2Player p = Main.LocalPlayer.GetModPlayer<TF2Player>();
            p.uberChargeTime = 0;
            p.uberCharge = 0;
            p.activateUberCharge = false;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/invulnerable_off"), Main.LocalPlayer.Center);
            return true;
        }
    }

    public class UberChargePlayer : ModPlayer
    {
        public bool uberCharge;
        public int uberChargeDuration = TF2.Time(8);

        public override void ResetEffects()
        {
            uberCharge = false;
            Player.creativeGodMode = false;
        }

        public override void PostUpdate()
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (uberCharge)
            {
                Player.creativeGodMode = true;
                SpawnDusts(Player);
            }
            if (p.activateUberCharge && Player.HasBuff<UberCharge>())
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

        protected static void SpawnDusts(Player player)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(player.Bottom - Vector2.One * 5, 10, 10, DustID.RedTorch, Main.rand.NextFloat(-5f, 5f), -5f)];
                dust.velocity.Y = -7.5f;
                dust.noGravity = true;
                dust.alpha = 128;
                dust.scale = Main.rand.Next(10, 20) * 0.1f;
            }
        }
    }

    public class UberChargeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private int timer;
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
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Main.dust[Dust.NewDust(npc.position, 8, 8, DustID.Clentaminator_Red, 0.0f, 0.0f, 100, new Color(), 1.5f)];
                    dust.velocity *= 0.5f;
                    dust.velocity.Y = -Math.Abs(dust.velocity.Y);
                }
            }
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(timer);
            binaryWriter.Write(uberCharge);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            timer = binaryReader.ReadInt32();
            uberCharge = binaryReader.ReadBoolean();
        }
    }
}