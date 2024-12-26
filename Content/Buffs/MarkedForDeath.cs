using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Dusts;

namespace TF2.Content.Buffs
{
    public class MarkedForDeath : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<MarkedForDeathPlayer>().markedForDeath = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<MarkedForDeathNPC>().markedForDeath = true;
    }

    public class MarkedForDeathPlayer : ModPlayer
    {
        public bool markedForDeath;

        public override void ResetEffects() => markedForDeath = false;

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (markedForDeath && !modifiers.PvP)
            {
                modifiers.SourceDamage.Base *= 1.35f;
                Dust.NewDust(Player.Center, 0, 0, ModContent.DustType<MiniCrit>(), 0f, 0);
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit_mini"), Player.Center);
            }
        }
    }

    public class MarkedForDeathNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool markedForDeath;

        public override void ResetEffects(NPC npc) => markedForDeath = false;
    }
}