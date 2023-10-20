using Terraria;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Buffs
{
    public class KritzkriegUberCharge : ModBuff
    {
        public override string Texture => "TF2/Content/Buffs/UberCharge";

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<KritzkriegUberChargeNPC>().uberCharge = true;

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<KritzkriegUberChargePlayer>().uberCharge = true;
            player.GetModPlayer<TF2Player>().crit = true;
        }

        public override bool RightClick(int buffIndex)
        {
            TF2Player uber = Main.LocalPlayer.GetModPlayer<TF2Player>();
            uber.uberChargeTime = 0;
            uber.uberCharge = 0;
            uber.activateUberCharge = false;
            return true;
        }
    }

    public class KritzkriegUberChargePlayer : UberChargePlayer
    {
        public override void PostUpdate()
        {
            TF2Player uber = Player.GetModPlayer<TF2Player>();
            if (uberCharge)
                SpawnDusts(Player);
            if (uber.activateUberCharge && Player.HasBuff<KritzkriegUberCharge>())
            {
                uber.uberChargeTime++;
                uber.uberChargeDuration = uberChargeDuration;
                if (uber.uberChargeTime >= uberChargeDuration)
                {
                    uber.uberChargeTime = 0;
                    uber.uberCharge = 0;
                    uber.activateUberCharge = false;
                }
            }
        }
    }

    public class KritzkriegUberChargeNPC : UberChargeNPC
    {
        public override bool InstancePerEntity => true;

        public override void AI(NPC npc) => base.AI(npc);

        /*
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (!uberCharge || projectile.ModProjectile is HealingBeam || !npc.friendly) return;
            IEntitySource projectileSource = npc.GetSource_FromAI();
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int projectileIndex = Projectile.NewProjectile(projectileSource, npc.Center, projectile.velocity * -1, projectile.type, damageDone * 10, 0f, Main.myPlayer, 0f, 0f);
                Projectile newProjectile = Main.projectile[projectileIndex];
                newProjectile.friendly = true;
                newProjectile.hostile = false;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectileIndex);
            }
        }

        public override void OnHitNPC(NPC npc, NPC target, NPC.HitInfo hit)
        {
            if (!npc.friendly && target.HasBuff<KritzkriegUberCharge>())
            {
                npc.life -= hit.Damage * 5;
                npc.checkDead();
            }
        }
        */
    }
}