using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TF2.Content.Buffs
{
    public class PyroFlames : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.IsATagBuff[Type] = true;
            TF2BuffBase.fireBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<PyroFlamesPlayer>().burnDebuff = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<PyroFlamesNPC>().burnDebuff = true;
    }

    public class PyroFlamesPlayer : ModPlayer
    {
        protected int timer;
        public bool burnDebuff;
        public float damageMultiplier = 1f;

        public override void ResetEffects() => burnDebuff = false;

        public override void UpdateBadLifeRegen()
        {
            if (burnDebuff)
            {
                timer++;
                TF2.Maximum(ref Player.lifeRegen, 0);
                Player.lifeRegenTime = 0;
                if (timer >= TF2.Time(0.5))
                {
                    Player.statLife -= TF2.Round(4 * damageMultiplier);
                    CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), CombatText.LifeRegen, (int)(4 * damageMultiplier), dramatic: false, dot: true);
                    if (Player.statLife <= 0)
                        Player.KillMe(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[5].ToNetworkText(Player.name)), (int)(4 * damageMultiplier), 0);
                    timer = 0;
                }
                int dustIndex = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y), Player.width, Player.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 5f;
            }
            else
                timer = 0;
        }
    }

    public class PyroFlamesNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        protected int timer;
        public bool burnDebuff;
        public float damageMultiplier = 1f;

        public override void ResetEffects(NPC npc) => burnDebuff = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (burnDebuff)
            {
                timer++;
                TF2.Maximum(ref npc.lifeRegen, 0);
                if (timer >= TF2.Time(0.5))
                {
                    npc.life -= TF2.Round(4 * damageMultiplier);
                    CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height), CombatText.LifeRegenNegative, (int)(4 * damageMultiplier), dramatic: false, dot: true);
                    npc.checkDead();
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        npc.netUpdate = true;
                    timer = 0;
                }
                int dustIndex = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 5f;
            }
            else
                timer = 0;
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(timer);
            binaryWriter.Write(burnDebuff);
            binaryWriter.Write(damageMultiplier);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            timer = binaryReader.ReadInt32();
            burnDebuff = binaryReader.ReadBoolean();
            damageMultiplier = binaryReader.ReadSingle();
        }
    }
}