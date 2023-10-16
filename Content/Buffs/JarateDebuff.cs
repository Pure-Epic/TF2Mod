using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Buffs
{
    public class JarateDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jarate");
            Description.SetDefault("Crit vulnerability");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<JaratePlayer>().jarateDebuff = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<JarateNPC>().jarateDebuff = true;
    }

    public class JarateCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jarate Cooldown");
            Description.SetDefault("You need to restock on Jarate");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            TF2BuffBase.cooldownBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<JaratePlayer>().jarateCooldown = true;
    }

    public class JaratePlayer : ModPlayer
    {
        public bool jarateDebuff;
        public bool jarateCooldown;

        public override void ResetEffects()
        {
            jarateDebuff = false;
            jarateCooldown = false;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (proj.type == ModContent.ProjectileType<Projectiles.Sniper.JarateProjectile>())
            {
                Player.noKnockback = true;
                Player.statLife += 1;
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    int buffTypes = Player.buffType[i];
                    if (Main.debuff[buffTypes] && Player.buffTime[i] > 0 && !BuffID.Sets.NurseCannotRemoveDebuff[buffTypes] && !Buffs.TF2BuffBase.cooldownBuff[buffTypes])
                    {
                        Player.DelBuff(i);
                        i = -1;
                    }
                }
            }
        }
    }

    public class JarateNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool jarateDebuff;
        public Color initialColor;
        public bool colorInitialized;

        public override void AI(NPC npc)
        {
            if (jarateDebuff)
                npc.color = Color.Yellow;
        }

        public override void PostAI(NPC npc)
        {
            if (!colorInitialized)
            {
                initialColor = npc.color;
                colorInitialized = true;
            }
        }

        public override void ResetEffects(NPC npc)
        {
            jarateDebuff = false;
            if (colorInitialized)
                npc.color = initialColor;
        }
    }
}