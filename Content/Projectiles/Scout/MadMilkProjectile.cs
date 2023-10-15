using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Scout
{
    public class MadMilkProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.ToxicFlask;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool PreDraw(ref Color lightColor) => TF2.DrawProjectile(Projectile, ref lightColor);

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;      
            Projectile.position = Projectile.Center;
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.Center = Projectile.position;
            FinalCollision();
            Projectile.timeLeft = 0;
            return false;
        }

        public override void AI() => CheckCollision();

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 25; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.CursedTorch, 0f, 0f, 100, default, 3f);
                Main.dust[dustIndex].velocity *= 5f;
                dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.CursedTorch, 0f, 0f, 100, default, 2f);
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
                {
                    HitNPC(npc);
                    Projectile.Kill();
                }
            }
            foreach (Player player in Main.player)
            {
                if (Projectile.Hitbox.Intersects(player.Hitbox) && player.active)
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
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/jar_explode"), Main.player[Projectile.owner].Center);
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
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    if (Main.player[Projectile.owner].buffType[i] == ModContent.BuffType<MadMilkCooldown>())
                        Main.player[Projectile.owner].buffTime[i] -= 240;
                }
            }
            else
                npc.AddBuff(ModContent.BuffType<MadMilkDebuff>(), 600);
            Projectile.Kill();
        }

        protected virtual void HitPlayer(Player targetPlayer)
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
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (Main.player[Projectile.owner].buffType[i] == ModContent.BuffType<MadMilkCooldown>() && Main.player[Projectile.owner] != targetPlayer)
                    Main.player[Projectile.owner].buffTime[i] -= 240;
            }
        }
    }
}