using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Items.Weapons.Sniper;

namespace TF2.Content.Projectiles.Sniper
{
    public class JarateProjectile : TF2Projectile
    {
        protected override void ProjectileStatistics()
        {
            SetProjectileSize(32, 32);
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.ToxicFlask;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override void ProjectileAI()
        {
            CheckCollision();
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) * 0.04f - MathHelper.ToRadians(90f)) * Projectile.direction;
        }

        protected override bool ProjectileTileCollide(Vector2 oldVelocity)
        {
            Projectile.position = Projectile.Center;
            Projectile.Size = new Vector2(250, 250);
            Projectile.hide = true;
            Projectile.tileCollide = false;
            Projectile.Center = Projectile.position;
            FinalCollision();
            return false;
        }

        protected override void ProjectileDestroy(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/jar_explode"), Projectile.Center);
            for (int i = 0; i < 25; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.IchorTorch, 0f, 0f, 100, default, 3f);
                Main.dust[dustIndex].velocity *= 5f;
                dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.IchorTorch, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 3f;
            }
        }

        private void CheckCollision()
        {
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

        private void FinalCollision()
        {
            foreach (NPC npc in Main.npc)
            {
                if (Projectile.Hitbox.Intersects(npc.Hitbox) && npc.active)
                    HitNPC(npc);
            }
            foreach (Player player in Main.player)
            {
                if (Projectile.Hitbox.Intersects(player.Hitbox) && player.active)
                {
                    HitPlayer(player);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
                }
            }
            Projectile.Kill();
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
                        npc.DelBuff(i);
                        i = -1;
                    }
                }
                for (int i = 0; i < Player.inventory.Length; i++)
                {
                    Item item = Player.inventory[i];
                    if (item.ModItem is Jarate weapon)
                        weapon.timer[0] -= TF2.Time(4);
                }
            }
            else
                npc.AddBuff(ModContent.BuffType<JarateDebuff>(), TF2.Time(10));
        }

        protected virtual void HitPlayer(Player targetPlayer)
        {
            if (targetPlayer.hostile)
                targetPlayer.AddBuff(ModContent.BuffType<JarateDebuff>(), TF2.Time(10));
            else
            {
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    int buffTypes = targetPlayer.buffType[i];
                    if (Main.debuff[buffTypes] && targetPlayer.buffTime[i] > 0 && !BuffID.Sets.NurseCannotRemoveDebuff[buffTypes] && !TF2BuffBase.cooldownBuff[buffTypes])
                    {
                        targetPlayer.DelBuff(i);
                        i = -1;
                    }
                }
                for (int i = 0; i < Player.inventory.Length; i++)
                {
                    Item item = Player.inventory[i];
                    if (item.ModItem is Jarate weapon)
                        weapon.timer[0] -= TF2.Time(4);
                }
            }
        }
    }
}