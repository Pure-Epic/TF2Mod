using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TF2.Content.Buffs
{
    public class Sapped : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<SappedPlayer>().sapperDebuff = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<SappedNPC>().sapperDebuff = true;
    }

    public class SappedPlayer : ModPlayer
    {
        private int timer;
        public bool sapperDebuff;
        public float damageMultiplier = 1f;

        public override void ResetEffects() => sapperDebuff = false;

        public override void UpdateBadLifeRegen()
        {
            if (sapperDebuff)
            {
                TF2.Maximum(ref Player.lifeRegen, 0);
                Player.lifeRegenTime = 0;
                if (timer >= TF2.Time(1))
                {
                    Player.statLife -= TF2.Round(25 * damageMultiplier);
                    CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), CombatText.LifeRegen, (int)(25 * damageMultiplier), dramatic: false, dot: true);
                    if (Player.statLife <= 0)
                        Player.KillMe(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[6].Format(Player.name)), (int)(4 * damageMultiplier), 0);
                    timer = 0;
                }
                Dust dust = Dust.NewDustDirect(new Vector2(Player.position.X, Player.position.Y), Player.width, Player.height, DustID.Electric);
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.alpha = 128;
                dust.scale = Main.rand.Next(10, 20) * 0.1f;
            }
        }
    }

    public class SappedNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private int timer;
        public bool sapperDebuff;
        public float damageMultiplier = 1f;

        public override void ResetEffects(NPC npc) => sapperDebuff = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (sapperDebuff)
            {
                timer++;
                TF2.Maximum(ref npc.lifeRegen, 0);
                if (timer >= TF2.Time(1))
                {
                    npc.velocity = new Vector2(0f, 0f);
                    npc.life -= TF2.Round(25 * damageMultiplier);
                    CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height), CombatText.LifeRegenNegative, (int)(25 * damageMultiplier), dramatic: false, dot: true);
                    npc.checkDead();
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        npc.netUpdate = true;
                    timer = 0;
                }
                Dust dust = Dust.NewDustDirect(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Electric);
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.alpha = 128;
                dust.scale = Main.rand.Next(10, 20) * 0.1f;
            }
            else
                timer = 0;
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(timer);
            binaryWriter.Write(sapperDebuff);
            binaryWriter.Write(damageMultiplier);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            timer = binaryReader.ReadInt32();
            sapperDebuff = binaryReader.ReadBoolean();
            damageMultiplier = binaryReader.ReadSingle();
        }
    }
}