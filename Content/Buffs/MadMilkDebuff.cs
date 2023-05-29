using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Buffs
{
    public class MadMilkDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mad Milk");
            Description.SetDefault("Life steal");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<MadMilkPlayer>().madMilkDebuff = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<MadMilkNPC>().madMilkDebuff = true;
    }

    public class MadMilkCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mad Milk Cooldown");
            Description.SetDefault("You need to restock on Mad Milk");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            TF2BuffBase.cooldownBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<MadMilkPlayer>().madMilkCooldown = true;
    }

    public class MadMilkPlayer : ModPlayer
    {
        public bool madMilkDebuff;
        public bool madMilkCooldown;

        public override void ResetEffects()
        {
            madMilkDebuff = false;
            madMilkCooldown = false;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (proj.type == ModContent.ProjectileType<Projectiles.Scout.MadMilkProjectile>())
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

    public class MadMilkNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool madMilkDebuff;
        public Color initialColor;
        public bool colorInitialized;

        public override void AI(NPC npc)
        {
            if (madMilkDebuff)
                npc.color = Color.Lime;
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
            madMilkDebuff = false;
            if (colorInitialized)
                npc.color = initialColor;
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            if (madMilkDebuff)
            {
                float classMultiplier = player.GetModPlayer<TF2Player>().classMultiplier;
                int healthDifference = player.statLifeMax2 / player.statLifeMax;
                player.statLife += (int)(damage * 0.6f / classMultiplier * healthDifference);
                player.HealEffect((int)(damage * 0.6f / classMultiplier * healthDifference), true);
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            if (madMilkDebuff)
            {
                Player player = Main.player[projectile.owner];
                float classMultiplier = player.GetModPlayer<TF2Player>().classMultiplier;
                int healthDifference = player.statLifeMax2 / player.statLifeMax;
                player.statLife += (int)(damage * 0.6f / classMultiplier * healthDifference);
                player.HealEffect((int)(damage * 0.6f / classMultiplier * healthDifference), true);
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (madMilkDebuff)
            {
                int dustIndex = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.CursedTorch, 0f, 0f, 100, default, 3f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 5f;
            }
        }
    }
}