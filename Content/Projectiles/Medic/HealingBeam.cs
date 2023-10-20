using Microsoft.Xna.Framework;
using System;
using System.IO;
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

        protected virtual float Heal => 1f;

        protected virtual int UberCharge => ModContent.BuffType<UberCharge>();

        protected int HealCooldown
        {
            get => Main.player[Projectile.owner].GetModPlayer<TF2Player>().multiplayerHealCooldown;
            set => Main.player[Projectile.owner].GetModPlayer<TF2Player>().multiplayerHealCooldown = value;
        }

        // protected int[] npcHealCooldown = new int[Main.maxNPCs];
        // protected int[] playerHealCooldown = new int[Main.maxPlayers];
        protected const int maxHealCooldown = 5;

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
                foreach (NPC npc in Main.npc)
                {
                    if (Projectile.Hitbox.Intersects(npc.Hitbox) && npc.friendly && npc.active && npc.life <= npc.lifeMax * 1.5f && HealCooldown <= 0)
                    {
                        if (!player.GetModPlayer<TF2Player>().activateUberCharge)
                            AddUberCharge(player.HeldItem);
                        else
                            npc.AddBuff(UberCharge, 480);
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            HealCooldown = 5;
                    }
                }
                foreach (Player targetPlayer in Main.player)
                {
                    if (Projectile.Hitbox.Intersects(targetPlayer.Hitbox) && targetPlayer.whoAmI != Projectile.owner && targetPlayer.active && !targetPlayer.dead && HealCooldown <= 0)
                    {
                        if (!player.GetModPlayer<TF2Player>().activateUberCharge)
                            AddUberCharge(player.HeldItem);
                        else
                            targetPlayer.AddBuff(UberCharge, 480, false);
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            HealCooldown = 5;
                    }
                }
            }
            foreach (NPC npc in Main.npc)
            {
                if (Main.netMode == NetmodeID.SinglePlayer || Main.netMode == NetmodeID.Server)
                {
                    if (Projectile.Hitbox.Intersects(npc.Hitbox) && npc.friendly && npc.active && HealCooldown <= 0)
                        HealNPC(npc);
                }
            }
            if (Main.netMode == NetmodeID.SinglePlayer) return;
            foreach (Player targetPlayer in Main.player)
            {
                if (Main.netMode == NetmodeID.Server)
                {
                    if (Projectile.Hitbox.Intersects(targetPlayer.Hitbox) && targetPlayer.whoAmI != Projectile.owner && targetPlayer.active && HealCooldown <= 0)
                        HealPlayer(targetPlayer);
                }
            }
        }

        public override bool ShouldUpdatePosition() => false;

        protected void HealNPC(NPC npc)
        {
            Player player = Main.player[Projectile.owner];
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (npc.life <= npc.lifeMax * 1.5f && HealCooldown <= 0)
            {
                int healingAmount = (int)(2 * p.classMultiplier) <= 1 ? 2 : (int)(2 * p.classMultiplier);
                npc.life += healingAmount;
                npc.HealEffect(healingAmount);
                npc.GetGlobalNPC<UberChargeNPC>().timer = 0;
                HealCooldown = 5;
                npc.netUpdate = true;
                Projectile.netUpdate = true;
            }
        }

        protected void HealPlayer(Player targetPlayer)
        {
            if (HealCooldown <= 0)
            {
                int healingAmount = (int)(Main.player[Projectile.owner].statLifeMax2 * 0.004f);
                targetPlayer.Heal(healingAmount);
                targetPlayer.HealEffect(healingAmount);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SpiritHeal, number: targetPlayer.whoAmI, number2: healingAmount);
                HealCooldown = 5;
                Projectile.netUpdate = true;
            }
        }

        protected void AddUberCharge(Item item)
        {
            TF2Weapon mediGun = item.ModItem as TF2Weapon;
            mediGun.AddUberCharge(Heal);
        }
    }
}