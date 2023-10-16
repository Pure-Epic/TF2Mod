using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items;

namespace TF2.Content.Projectiles.Medic
{
    public class HealingBeam : ModProjectile
    {
        public override string Texture => "TF2/Content/Items/Medic/MediGun";

        protected int[] npcHealCooldown = new int[Main.maxNPCs];
        protected int[] playerHealCooldown = new int[Main.maxPlayers];
        protected const int healCooldown = 5;
        protected Player player;

        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.GetGlobalProjectile<TF2ProjectileBase>().healingProjectile = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.position = Main.MouseWorld;
                for (int i = 0; i < 10; i++)
                {
                    Vector2 spawn = Projectile.position + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (12f - i * 2);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.position, 20, 20, DustID.Clentaminator_Red, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f)];
                    dust.velocity = Vector2.Normalize(Projectile.position - spawn) * 1.5f * (10f - i * 2f) / 10f;
                    dust.noGravity = true;
                    dust.scale = Main.rand.Next(10, 20) * 0.05f;
                }
                player.itemTime = 2;
                player.itemAnimation = 2;
                Vector2 velocity = Main.MouseWorld - player.Center;
                velocity.SafeNormalize(Vector2.UnitX);
                Projectile.velocity = velocity;
                Projectile.direction = Main.MouseWorld.X >= player.position.X ? 1 : -1;
                player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
                player.ChangeDir(Projectile.direction);
                if (!player.controlUseItem)
                    Projectile.Kill();

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (Projectile.Hitbox.Intersects(npc.Hitbox) && npc.friendly && npc.active)
                        HealNPC(npc);
                    npcHealCooldown[npc.whoAmI]--;
                    npcHealCooldown[npc.whoAmI] = Utils.Clamp(npcHealCooldown[npc.whoAmI], 0, healCooldown);
                }
                foreach (Player targetPlayer in Main.player)
                {
                    if (Projectile.Hitbox.Intersects(targetPlayer.Hitbox) && targetPlayer.whoAmI != Projectile.owner && targetPlayer.active)
                        HealPlayer(targetPlayer);
                    playerHealCooldown[targetPlayer.whoAmI]--;
                    playerHealCooldown[targetPlayer.whoAmI] = Utils.Clamp(playerHealCooldown[targetPlayer.whoAmI], 0, healCooldown);
                }
            }
        }

        public override bool ShouldUpdatePosition() => false;

        protected virtual void HealNPC(NPC npc)
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
                    AddUberCharge(player.HeldItem);
            }
            if (p.activateUberCharge)
                npc.AddBuff(ModContent.BuffType<UberCharge>(), 480);
            Projectile.netUpdate = true;
        }

        protected virtual void HealPlayer(Player targetPlayer)
        {
            Player player = Main.player[Projectile.owner];
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (playerHealCooldown[targetPlayer.whoAmI] <= 0)
            {
                int healingAmount = (int)(Main.player[Projectile.owner].statLifeMax2 * 0.004f);
                targetPlayer.Heal(healingAmount);
                playerHealCooldown[targetPlayer.whoAmI] = 5;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.PlayerHeal, number: player.whoAmI, number2: healingAmount);
            }
            if (!p.activateUberCharge)
                AddUberCharge(player.HeldItem);
            else
                targetPlayer.AddBuff(ModContent.BuffType<UberCharge>(), 480);
            Projectile.netUpdate = true;
        }

        protected static void AddUberCharge(Item item, float rate = 1)
        {
            TF2Weapon mediGun = item.ModItem as TF2Weapon;
            mediGun.AddUberCharge(rate);
        }
    }
}