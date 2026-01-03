using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TF2.Content.Buffs
{
    public class Bleeding : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<BleedingPlayer>().bleedingDebuff = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<BleedingNPC>().bleedingDebuff = true;
    }

    public class BleedingPlayer : ModPlayer
    {
        public bool bleedingDebuff;
        private int timer;
        public float damageMultiplier = 1f;

        public override void ResetEffects() => bleedingDebuff = false;

        public override void UpdateBadLifeRegen()
        {
            if (bleedingDebuff)
            {
                timer++;
                TF2.Maximum(ref Player.lifeRegen, 0);
                Player.lifeRegenTime = 0;
                if (timer >= TF2.Time(0.5))
                {
                    Player.statLife -= TF2.Round(4 * damageMultiplier);
                    CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), CombatText.LifeRegen, (int)(4 * damageMultiplier), dramatic: false, dot: true);
                    if (Player.statLife <= 0)
                        Player.KillMe(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[0].ToNetworkText(Player.name)), (int)(4 * damageMultiplier), 0);
                    timer = 0;
                }
                int dustIndex = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Blood, 0f, 0f, 100, default, 3f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 5f;
            }
            else
                timer = 0;
        }
    }

    public class BleedingNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool bleedingDebuff;
        private int timer;
        public float damageMultiplier = 1f;

        public override void ResetEffects(NPC npc) => bleedingDebuff = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (bleedingDebuff)
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
                int dustIndex = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, 0f, 0f, 100, default, 3f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 5f;
            }
            else
                timer = 0;
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(bleedingDebuff);
            binaryWriter.Write(timer);
            binaryWriter.Write(damageMultiplier);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            bleedingDebuff = binaryReader.ReadBoolean();
            timer = binaryReader.ReadInt32();
            damageMultiplier = binaryReader.ReadSingle();
        }
    }
}