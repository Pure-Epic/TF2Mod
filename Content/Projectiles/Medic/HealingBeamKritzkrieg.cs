using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Medic
{
    public class HealingBeamKritzkrieg : HealingBeam
    {
        public override string Texture => "TF2/Content/Items/Medic/Kritzkrieg";

        protected override void HealNPC(NPC npc)
        {
            Player player = Main.player[Projectile.owner];
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (npc.life <= npc.lifeMax * 1.5f && npcHealCooldown[npc.whoAmI] <= 0)
            {
                int healingAmount = (int)(2 * p.classMultiplier) <= 1 ? 2 : (int)(2 * p.classMultiplier);
                npc.life += healingAmount;
                npc.HealEffect(healingAmount);
                npc.GetGlobalNPC<UberChargeNPC>().timer = 0;
                npc.netUpdate = true;
                npcHealCooldown[npc.whoAmI] = 5;
                if (!p.activateUberCharge)
                    AddUberCharge(player.HeldItem, 1.25f);
            }
            if (p.activateUberCharge)
                npc.AddBuff(ModContent.BuffType<KritzkriegUberCharge>(), 480);
            Projectile.netUpdate = true;
        }

        protected override void HealPlayer(Player targetPlayer)
        {
            Player player = Main.player[Projectile.owner];
            TF2Player p = player.GetModPlayer<TF2Player>();
            int healingAmount = (int)(Main.player[Projectile.owner].statLifeMax2 * 0.004f);
            targetPlayer.Heal(healingAmount);
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.PlayerHeal, number: player.whoAmI, number2: healingAmount);
            if (!p.activateUberCharge)
                AddUberCharge(player.HeldItem, 1.25f);
            else
                targetPlayer.AddBuff(ModContent.BuffType<KritzkriegUberCharge>(), 480);
            Projectile.netUpdate = true;
        }
    }
}