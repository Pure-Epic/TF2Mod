using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Pyro
{
    public class Airblast : ModProjectile
    {
        private bool allowHeal;

        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 29;
            Projectile.aiStyle = 1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 64;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.damage = 1;
            Projectile.knockBack = 20f;
            AIType = ProjectileID.Bullet;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
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
                if (Projectile.Hitbox.Intersects(player.Hitbox) && player.whoAmI != Projectile.owner && player.active)
                {
                    HitPlayer(player);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) => TF2.DrawProjectile(Projectile, ref lightColor);

        public override void OnKill(int timeLeft)
        {
            // This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
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
                if (player.statLife == player.statLifeMax2 || !allowHeal) return;
                player.Heal((int)(Main.player[Projectile.owner].statLifeMax2 * 0.11428571428));
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
                knockbackPower = (npc.noGravity ? (knockbackPower * -0.5f) : (knockbackPower * -0.75f));
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
            if (player.statLife == player.statLifeMax2 || !allowHeal) return;
            player.Heal((int)(Main.player[Projectile.owner].statLifeMax2 * 0.11428571428));
        }
    }
}