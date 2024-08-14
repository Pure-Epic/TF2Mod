using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Weapons;
using TF2.Content.NPCs.Buddies;

namespace TF2.Content.Projectiles.Medic
{
    public class HealingBeam : TF2Projectile
    {
        public override string Texture => "TF2/Content/Items/Weapons/Medic/MediGun";

        protected virtual float HealMultiplier => 1f;

        protected virtual float UberchargeMultiplier => 1f;

        protected virtual float OverhealLimit => 0.5f;

        protected virtual int UberCharge => ModContent.BuffType<UberCharge>();

        protected int HealCooldown
        {
            get => Player.GetModPlayer<TF2Player>().multiplayerHealCooldown;
            set => Player.GetModPlayer<TF2Player>().multiplayerHealCooldown = value;
        }

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(25, 25);
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            healingProjectile = true;
        }

        protected override void ProjectileAI()
        {
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
                Player.itemTime = 2;
                Player.itemAnimation = 2;
                Vector2 velocity = Main.MouseWorld - Player.Center;
                velocity.SafeNormalize(Vector2.UnitX);
                Projectile.velocity = velocity;
                Projectile.direction = Main.MouseWorld.X >= Player.position.X ? 1 : -1;
                Player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
                Player.ChangeDir(Projectile.direction);
                if (!Player.controlUseItem)
                {
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
                        packet.Write((byte)TF2.MessageType.KillProjectile);
                        packet.Write((byte)Projectile.whoAmI);
                        packet.Send();
                    }
                    Projectile.Kill();
                }
                foreach (Player targetPlayer in Main.ActivePlayers)
                {
                    if (Projectile.Hitbox.Intersects(targetPlayer.Hitbox) && targetPlayer.whoAmI != Projectile.owner && !targetPlayer.dead && !targetPlayer.hostile && HealCooldown <= 0)
                    {
                        if (!Player.GetModPlayer<TF2Player>().activateUberCharge)
                            AddUberCharge(Player.HeldItem);
                        else
                        {
                            if (UberCharge == ModContent.BuffType<QuickFixUberCharge>())
                                TF2.RemoveAllDebuffs(targetPlayer);
                            targetPlayer.AddBuff(UberCharge, TF2.Time(8), false);
                        }
                        HealPlayer(targetPlayer);
                        HealCooldown = 5;
                        break;
                    }
                }
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (Projectile.Hitbox.Intersects(npc.Hitbox) && npc.friendly && npc.life <= npc.lifeMax * 1.5f && HealCooldown <= 0)
                    {
                        if (!Player.GetModPlayer<TF2Player>().activateUberCharge)
                            AddUberCharge(Player.HeldItem);
                        else
                        {
                            if (UberCharge == ModContent.BuffType<QuickFixUberCharge>())
                                TF2.RemoveAllDebuffs(npc);
                            npc.AddBuff(UberCharge, TF2.Time(8));
                        }
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            HealCooldown = 5;
                        break;
                    }
                }
            }
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Projectile.Hitbox.Intersects(npc.Hitbox) && npc.friendly && HealCooldown <= 0)
                    {
                        HealNPC(npc);
                        break;
                    }
                }
            }
            if (Main.netMode == NetmodeID.SinglePlayer) return;
            foreach (Player targetPlayer in Main.ActivePlayers)
            {
                if (Projectile.Hitbox.Intersects(targetPlayer.Hitbox) && targetPlayer.whoAmI != Projectile.owner && !targetPlayer.dead && !targetPlayer.hostile && Main.netMode == NetmodeID.Server)
                {
                    HealPlayer(targetPlayer);
                    break;
                }
            }
            if (HealCooldown > 0)
                HealCooldown--;
            Player.GetModPlayer<TF2Player>().SyncPlayer(-1, Main.myPlayer, false);
        }

        public override bool ShouldUpdatePosition() => false;

        protected void HealNPC(NPC target)
        {
            if (target.ModNPC is not MercenaryBuddy buddy) return;
            if (HealCooldown <= 0)
            {
                int healingAmount = TF2.Round(target.lifeMax / buddy.BaseHealth * 0.4f * HealMultiplier);
                if (healingAmount <= 0)
                    healingAmount = 1;
                if (Main.netMode == NetmodeID.SinglePlayer)
                    TF2.OverhealNPC(target, healingAmount, OverhealLimit);
                else if (Main.netMode == NetmodeID.Server)
                    TF2.OverhealNPCMultiplayer(target, healingAmount, OverhealLimit);
                if (Player.HasBuff<QuickFixUberCharge>())
                    TF2.Overheal(Player, TF2.Round(TF2.GetRawHealth(Player, 0.4f * HealMultiplier)), OverhealLimit);
                HealCooldown = 5;
                Projectile.netUpdate = true;
            }
        }

        protected void HealPlayer(Player target)
        {
            if (HealCooldown <= 0)
            {
                int healingAmount = TF2.Round(TF2.GetRawHealth(target, 0.4f * HealMultiplier));
                if (healingAmount <= 0)
                    healingAmount = 1;
                if (Main.netMode == NetmodeID.Server)
                    TF2.OverhealMultiplayer(target, healingAmount, OverhealLimit);
                if (Player.HasBuff<QuickFixUberCharge>())
                    TF2.Overheal(Player, healingAmount, OverhealLimit);
                HealCooldown = 5;
                Projectile.netUpdate = true;
            }
        }

        protected void AddUberCharge(Item item) => (item.ModItem as TF2Weapon).AddUberCharge(UberchargeMultiplier);
    }
}