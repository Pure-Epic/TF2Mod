using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
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
            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<MadMilkDebuffPlayer>().madMilkDebuff = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<MadMilkDebuffNPC>().madMilkDebuff = true;
    }

    public class MadMilkDebuffPlayer : ModPlayer
    {
        public bool madMilkDebuff;

        public override void ResetEffects() => madMilkDebuff = false;
    }

    public class MadMilkDebuffNPC : GlobalNPC
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
                if (TF2Player.IsAtFullHealth(player)) return;
                float classMultiplier = player.GetModPlayer<TF2Player>().damageMultiplier;
                player.Heal(TF2.Round(damageDone * 0.6f / classMultiplier * TF2.GetHealth(player, 1)));
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (madMilkDebuff)
            {
                Player player = Main.player[projectile.owner];
                if (TF2Player.IsAtFullHealth(player)) return;
                float classMultiplier = player.GetModPlayer<TF2Player>().damageMultiplier;
                player.Heal(TF2.Round(damageDone * 0.6f / classMultiplier * TF2.GetHealth(player, 1)));
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (madMilkDebuff)
            {
                int dustIndex = Dust.NewDust(npc.position, npc.width, npc.height, DustID.WhiteTorch, 0f, 0f, 100, default, 3f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 5f;
            }
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(madMilkDebuff);
            binaryWriter.Write(colorInitialized);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            madMilkDebuff = binaryReader.ReadBoolean();
            colorInitialized = binaryReader.ReadBoolean();
        }
    }
}