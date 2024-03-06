using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Weapons;

namespace TF2.Content.Projectiles.Medic
{
    public class HealingBeam : TF2Projectile
    {
        public override string Texture => "TF2/Content/Items/Weapons/Medic/MediGun";

        protected virtual float Heal => 1f;

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
                foreach (NPC npc in Main.npc)
                {
                    if (Projectile.Hitbox.Intersects(npc.Hitbox) && npc.friendly && npc.active && npc.life <= npc.lifeMax * 1.5f && HealCooldown <= 0)
                    {
                        if (!Player.GetModPlayer<TF2Player>().activateUberCharge)
                            AddUberCharge(Player.HeldItem);
                        else
                            npc.AddBuff(UberCharge, TF2.Time(8));
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            HealCooldown = 5;
                    }
                }
                foreach (Player targetPlayer in Main.player)
                {
                    if (Projectile.Hitbox.Intersects(targetPlayer.Hitbox) && targetPlayer.whoAmI != Projectile.owner && targetPlayer.active && !targetPlayer.dead && !targetPlayer.hostile && HealCooldown <= 0)
                    {
                        if (!Player.GetModPlayer<TF2Player>().activateUberCharge)
                            AddUberCharge(Player.HeldItem);
                        else
                            targetPlayer.AddBuff(UberCharge, TF2.Time(8), false);
                        HealPlayer(targetPlayer);
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
                if (Projectile.Hitbox.Intersects(targetPlayer.Hitbox) && targetPlayer.whoAmI != Projectile.owner && targetPlayer.active && !targetPlayer.dead && !targetPlayer.hostile && Main.netMode == NetmodeID.Server)
                    HealPlayer(targetPlayer);
            }
            if (HealCooldown > 0)
                HealCooldown--;
            Player.GetModPlayer<TF2Player>().SyncPlayer(-1, Main.myPlayer, false);
        }

        public override bool ShouldUpdatePosition() => false;

        protected void HealNPC(NPC npc)
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (npc.life <= npc.lifeMax * 1.5f && HealCooldown <= 0)
            {
                int healingAmount = (int)(2 * p.classMultiplier) <= 1 ? 2 : (int)(2 * p.classMultiplier);
                npc.life += healingAmount;
                if (npc.life > TF2.Round(npc.lifeMax * 1.5f))
                    npc.life = TF2.Round(npc.lifeMax * 1.5f);
                npc.HealEffect(healingAmount);
                npc.GetGlobalNPC<TF2GlobalNPC>().overhealTimer = 0;
                HealCooldown = 5;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    npc.netUpdate = true;
                    Projectile.netUpdate = true;
                }
            }
        }

        protected void HealPlayer(Player targetPlayer)
        {
            if (HealCooldown <= 0)
            {
                int healingAmount = TF2.Round(TF2.GetHealthRaw(targetPlayer, 0.4f));
                if (healingAmount <= 0)
                    healingAmount = 1;
                targetPlayer.Heal(healingAmount);
                targetPlayer.HealEffect(healingAmount);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    TF2.OverhealMultiplayer(targetPlayer, healingAmount, OverhealLimit);
                HealCooldown = 5;
                Projectile.netUpdate = true;
            }
        }

        protected void AddUberCharge(Item item) => (item.ModItem as TF2Weapon).AddUberCharge(Heal);
    }
}