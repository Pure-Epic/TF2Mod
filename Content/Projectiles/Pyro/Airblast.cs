using Terraria;
using Terraria.ID;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Pyro
{
    public class Airblast : TF2Projectile
    {
        private bool allowHeal;

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(100, 100);
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override void ProjectileAI()
        {
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile != Projectile && Projectile.Hitbox.Intersects(projectile.Hitbox) && TF2.CanParryProjectile(projectile) && projectile.hostile && !projectile.friendly)
                {
                    projectile.velocity *= -1f;
                    projectile.hostile = false;
                }
            }
            foreach (Player player in Main.ActivePlayers)
            {
                if (Projectile.Hitbox.Intersects(player.Hitbox) && player.whoAmI != Projectile.owner)
                {
                    HitPlayer(player);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
                    Projectile.Kill();
                }
            }
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (Projectile.Hitbox.Intersects(npc.Hitbox))
                {
                    HitNPC(npc);
                    Projectile.Kill();
                }
            }
        }

        protected virtual void HitPlayer(Player targetPlayer)
        {
            if (!targetPlayer.hostile)
            {
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    int buffTypes = targetPlayer.buffType[i];
                    if (Main.debuff[buffTypes] && targetPlayer.buffTime[i] > 0 && !BuffID.Sets.NurseCannotRemoveDebuff[buffTypes] && !TF2BuffBase.cooldownBuff[buffTypes])
                    {
                        allowHeal = true;
                        targetPlayer.DelBuff(i);
                        i = -1;
                    }
                }
                Player player = Main.player[Projectile.owner];
                if (TF2Player.IsHealthFull(player) || !allowHeal) return;
                player.Heal(TF2.GetHealth(player, 20));
            }
            else
                KnockbackPlayer(targetPlayer, Projectile.velocity.X > 0f ? 1 : -1, 10f);
        }

        protected virtual void HitNPC(NPC npc)
        {
            if (npc.friendly)
            {
                for (int i = 0; i < NPC.maxBuffs; i++)
                {
                    int buffTypes = npc.buffType[i];
                    if (Main.debuff[buffTypes] && npc.buffTime[i] > 0)
                    {
                        allowHeal = true;
                        npc.DelBuff(i);
                        i = -1;
                    }
                }
                Player player = Main.player[Projectile.owner];
                if (TF2Player.IsHealthFull(player) || !allowHeal) return;
                player.Heal(TF2.GetHealth(player, 20));
            }
            else
                KnockbackNPC(npc, Projectile.velocity.X > 0f ? 1 : -1, 10f);
        }
    }
}