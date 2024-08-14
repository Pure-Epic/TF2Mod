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
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<MadMilkPlayer>().madMilkDebuff = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<MadMilkNPC>().madMilkDebuff = true;
    }

    public class MadMilkPlayer : ModPlayer
    {
        public bool madMilkDebuff;

        public override void ResetEffects() => madMilkDebuff = false;
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

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (madMilkDebuff)
            {
                if (TF2Player.IsHealthFull(player)) return;
                float classMultiplier = player.GetModPlayer<TF2Player>().classMultiplier;
                player.Heal(TF2.Round(damageDone * 0.6f / classMultiplier * TF2.GetHealth(player, 1)));
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (madMilkDebuff)
            {
                Player player = Main.player[projectile.owner];
                if (TF2Player.IsHealthFull(player)) return;
                float classMultiplier = player.GetModPlayer<TF2Player>().classMultiplier;
                player.Heal(TF2.Round(damageDone * 0.6f / classMultiplier * TF2.GetHealth(player, 1)));
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