using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Buffs
{
    public class BaseballDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<SluggedNPC>().slowDebuff = true;
    }

    public class SluggedNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool slowDebuff;
        public int timer;

        public override void ResetEffects(NPC npc) => slowDebuff = false;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (slowDebuff)
            {
                timer++;
                npc.velocity = new Vector2(npc.velocity.X / 2, npc.velocity.Y / 2);
            }
            else
                timer = 0;
        }
    }

    public class BaseballCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            TF2BuffBase.cooldownBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<BaseballPlayer>().baseballCooldown = true;
    }

    public class BaseballPlayer : ModPlayer
    {
        public bool baseballCooldown;

        public override void ResetEffects() => baseballCooldown = false;
    }
}