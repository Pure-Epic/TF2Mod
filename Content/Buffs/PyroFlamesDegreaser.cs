using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Buffs
{
    public class PyroFlamesDegreaser : ModBuff
    {
        public override string Texture => "TF2/Content/Buffs/PyroFlames";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.IsATagBuff[Type] = true;
            TF2BuffBase.fireBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<PyroFlamesDegreaserPlayer>().burnDebuff = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<PyroFlamesDegreaserNPC>().burnDebuff = true;
    }

    public class PyroFlamesDegreaserPlayer : PyroFlamesPlayer
    {
        public override void UpdateBadLifeRegen()
        {
            if (burnDebuff)
            {
                timer++;
                TF2.Maximum(ref Player.lifeRegen, 0);
                Player.lifeRegenTime = 0;
                if (timer >= TF2.Time(0.5))
                {
                    Player.statLife -= (int)MathHelper.Max(1.33f * damageMultiplier, 1f);
                    CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), CombatText.LifeRegen, (int)(1.33f * damageMultiplier), dramatic: false, dot: true);
                    if (Player.statLife <= 0)
                        Player.KillMe(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[5].ToNetworkText(Player.name)), (int)(1.33f * damageMultiplier), 0);
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

    public class PyroFlamesDegreaserNPC : PyroFlamesNPC
    {
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (burnDebuff)
            {
                timer++;
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                if (timer >= TF2.Time(0.5))
                {
                    npc.life -= (int)MathHelper.Max(1.33f * damageMultiplier, 1f);
                    CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height), CombatText.LifeRegenNegative, (int)MathHelper.Max(1.33f * damageMultiplier, 1f), dramatic: false, dot: true);
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
    }
}