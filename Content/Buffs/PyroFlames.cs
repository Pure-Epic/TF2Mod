using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Buffs
{
    public class PyroFlames : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Afterburn");
            Description.SetDefault("Losing life");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<PyroFlamesPlayer>().lifeRegenDebuff = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<PyroFlamesNPC>().lifeRegenDebuff = true;
    }

    public class PyroFlamesPlayer : ModPlayer
    {
        public bool lifeRegenDebuff;
        public float damageMultiplier = 1f;
        public int timer;

        public override void ResetEffects() => lifeRegenDebuff = false;

        public override void UpdateBadLifeRegen()
        {
            if (lifeRegenDebuff)
            {
                timer++;
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                Player.lifeRegenTime = 0;
                if (timer >= 30)
                {
                    Player.statLife -= (int)(4 * damageMultiplier);
                    CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), CombatText.LifeRegenNegative, (int)(4 * damageMultiplier), dramatic: false, dot: true);
                    if (Player.statLife <= 0)
                        Player.KillMe(PlayerDeathReason.ByCustomReason(Player.name + " burnt to death."), (int)(4 * damageMultiplier), 0);
                    timer = 0;
                }
            }
            else
                timer = 0;
        }
    }

    public class PyroFlamesNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool lifeRegenDebuff;
        public float damageMultiplier = 1f;
        public int timer;

        public override void ResetEffects(NPC npc) => lifeRegenDebuff = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (lifeRegenDebuff)
            {
                timer++;
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                if (timer >= 30)
                {
                    npc.life -= (int)(4 * damageMultiplier);
                    CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height), CombatText.LifeRegenNegative, (int)(4 * damageMultiplier), dramatic: false, dot: true);
                    npc.checkDead();
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