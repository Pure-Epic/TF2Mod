using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TF2.Content.Buffs
{
    public class SapperDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<SapperDebuffPlayer>().sapperDebuff = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<SapperDebuffNPC>().sapperDebuff = true;
    }

    public class SapperDebuffPlayer : ModPlayer
    {
        public bool sapperDebuff;
        private int timer;
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
                        Player.KillMe(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[6].ToNetworkText(Player.name)), (int)(4 * damageMultiplier), 0);
                    timer = 0;
                }
                Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Electric);
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.alpha = 128;
                dust.scale = Main.rand.Next(10, 20) * 0.1f;
                Lighting.AddLight(Player.Center, Color.Aqua.ToVector3());
            }
        }
    }

    public class SapperDebuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool sapperDebuff;
        private int timer;
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
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Electric);
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.alpha = 128;
                dust.scale = Main.rand.Next(10, 20) * 0.1f;
                Lighting.AddLight(npc.Center, Color.Aqua.ToVector3());
            }
            else
                timer = 0;
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(sapperDebuff);
            binaryWriter.Write(timer);
            binaryWriter.Write(damageMultiplier);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            sapperDebuff = binaryReader.ReadBoolean();
            timer = binaryReader.ReadInt32();
            damageMultiplier = binaryReader.ReadSingle();
        }
    }
}