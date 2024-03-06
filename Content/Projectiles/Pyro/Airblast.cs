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
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile != Projectile && Projectile.Hitbox.Intersects(projectile.Hitbox) && TF2.CanParryProjectile(projectile) && projectile.hostile && !projectile.friendly)
                {
                    projectile.velocity *= -1f;
                    projectile.hostile = false;
                }
            }
            foreach (NPC npc in Main.npc)
            {
                if (Projectile.Hitbox.Intersects(npc.Hitbox) && npc.active)
                {
                    HitNPC(npc);
                    Projectile.Kill();
                }
            }
            foreach (Player player in Main.player)
            {
                if (Projectile.Hitbox.Intersects(player.Hitbox) && player.whoAmI != Projectile.owner && player.active && !player.hostile)
                {
                    HitPlayer(player);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
                    Projectile.Kill();
                }
            }
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
            else if (!npc.friendly || npc.boss)
            {
                float knockbackPower = 10f;
                int direction = Projectile.velocity.X > 0f ? 1 : -1;
                if (npc.type == NPCID.TargetDummy) return;
                if (direction < 0 && npc.velocity.X > 0f - knockbackPower)
                {
                    if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X -= knockbackPower;
                    }
                    npc.velocity.X -= knockbackPower;
                    if (npc.velocity.X < 0f - knockbackPower)
                    {
                        npc.velocity.X = 0f - knockbackPower;
                    }
                }
                else if (direction > 0 && npc.velocity.X < knockbackPower)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X += knockbackPower;
                    }
                    npc.velocity.X += knockbackPower;
                    if (npc.velocity.X > knockbackPower)
                    {
                        npc.velocity.X = knockbackPower;
                    }
                }
                knockbackPower = npc.noGravity ? (knockbackPower * -0.5f) : (knockbackPower * -0.75f);
                if (npc.velocity.Y > knockbackPower)
                {
                    npc.velocity.Y += knockbackPower;
                    if (npc.velocity.Y < knockbackPower)
                    {
                        npc.velocity.Y = knockbackPower;
                    }
                }
            }
        }

        protected virtual void HitPlayer(Player targetPlayer)
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
    }
}