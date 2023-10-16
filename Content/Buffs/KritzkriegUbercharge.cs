using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Buffs
{
    public class KritzkriegUbercharge : ModBuff
    {
        public override string Texture => "TF2/Content/Buffs/Ubercharge";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ubercharge");
            Description.SetDefault("Crits everything");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<KritzkriegUberchargeNPC>().ubercharge = true;

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<KritzkriegUberchargePlayer>().ubercharge = true;
            player.GetModPlayer<TF2Player>().crit = true;
        }
    }

    public class KritzkriegUberchargePlayer : ModPlayer
    {
        public bool ubercharge;

        public override void ResetEffects() => ubercharge = false;

        public override void PostUpdate()
        {
            TF2Player uber = Player.GetModPlayer<TF2Player>();
            if (ubercharge)
                SpawnDusts(Player);
            if (uber.activateUbercharge == true && Player.HasBuff<KritzkriegUbercharge>())
            {
                uber.uberchargeTime++;
                if (uber.uberchargeTime >= 480)
                {
                    uber.uberchargeTime = 0;
                    uber.ubercharge = 0;
                    uber.activateUbercharge = false;
                }
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (proj.Name == "Healing Beam")
            {
                damage = -1;
                crit = false;
            }
        }

        private static void SpawnDusts(Player player)
        {
            for (int i = 0; i < 2; ++i)
            {
                Dust dust = Main.dust[Dust.NewDust(player.position, 8, 8, DustID.Clentaminator_Red, 0.0f, 0.0f, 100, new Color(), 1.5f)];
                dust.velocity *= 0.5f;
                dust.velocity.Y = -Math.Abs(dust.velocity.Y);
            }
        }
    }

    public class KritzkriegUberchargeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int timer = 0;
        public bool ubercharge;

        public override void ResetEffects(NPC npc) => ubercharge = false;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.Name == "Healing Beam")
            {
                damage = -1;
                crit = false;
                timer = 0;
            }
        }

        public override void AI(NPC npc)
        {
            timer++;
            if (npc.life > npc.lifeMax && timer > 50)
            {
                npc.life -= 1;
                timer = 0;
            }
            if (ubercharge)
                SpawnDusts(npc);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            if (!ubercharge || projectile.Name == "Healing Beam" || !npc.friendly) return;
            var projectileSource = npc.GetSource_FromAI();
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int projectileIndex = Projectile.NewProjectile(projectileSource, npc.Center, projectile.velocity * -1, projectile.type, damage * 10, 0f, Main.myPlayer, 0f, 0f);
                Projectile newProjectile = Main.projectile[projectileIndex];
                newProjectile.friendly = true;
                newProjectile.hostile = false;
                NetMessage.SendData(MessageID.SyncProjectile, number: projectileIndex);
            }
        }

        public override void OnHitNPC(NPC npc, NPC target, int damage, float knockback, bool crit)
        {
            if (!npc.friendly && target.HasBuff<KritzkriegUbercharge>())
            {
                npc.life -= damage * 5;
                npc.checkDead();
            }
        }

        private static void SpawnDusts(NPC npc)
        {
            for (int i = 0; i < 2; ++i)
            {
                Dust dust = Main.dust[Dust.NewDust(npc.position, 8, 8, DustID.Clentaminator_Red, 0.0f, 0.0f, 100, new Color(), 1.5f)];
                dust.velocity *= 0.5f;
                dust.velocity.Y = -Math.Abs(dust.velocity.Y);
            }
        }
    }
}