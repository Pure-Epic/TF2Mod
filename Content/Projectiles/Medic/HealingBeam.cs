using Microsoft.Xna.Framework;
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

        public virtual float OverhealLimit => 0.5f;

        protected virtual int UberCharge => ModContent.BuffType<UberCharge>();

        public int HealCooldown
        {
            get => Player.GetModPlayer<TF2Player>().healCooldown;
            set => Player.GetModPlayer<TF2Player>().healCooldown = value;
        }

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(50, 50);
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            healingProjectile = true;
        }

        protected override void ProjectileAI()
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.Center = Main.MouseWorld;
                Vector2 velocity = Main.MouseWorld - Player.Center;
                Vector2 maxVelocity = Utils.SafeNormalize(velocity, Vector2.UnitY) * 250f;
                bool withinRange = velocity.Length() <= maxVelocity.Length();
                if (!withinRange)
                    Projectile.Center = Player.Center + maxVelocity;
                Entity target = GetNearestPlayer(Projectile, 200f);
                if (target != null && WithinHealRange(Player, Projectile, target))
                {
                    Player targetPlayer = target as Player;
                    if (!p.activateUberCharge)
                        AddUberCharge(Player.HeldItem);
                    else
                    {
                        if (UberCharge == ModContent.BuffType<QuickFixUberCharge>())
                            TF2.RemoveAllDebuffs(targetPlayer);
                        targetPlayer.AddBuff(UberCharge, TF2.Time(8), false);
                    }
                    HealPlayer(targetPlayer);
                    Projectile.Center = target.Center;
                    p.isHealing = true;
                    if (targetPlayer.moveSpeed > Player.moveSpeed)
                        p.healingMovementSpeed = targetPlayer.moveSpeed;
                }
                else
                {
                    target = GetNearestBuddy(Projectile, 200f);
                    if (target != null && WithinHealRange(Player, Projectile, target))
                    {
                        NPC npc = target as NPC;
                        if (npc.friendly && npc.life <= npc.lifeMax * 1.5f && HealCooldown <= 0)
                        {
                            if (!p.activateUberCharge)
                                AddUberCharge(Player.HeldItem);
                            else
                            {
                                if (UberCharge == ModContent.BuffType<QuickFixUberCharge>())
                                    TF2.RemoveAllDebuffs(npc);
                                npc.AddBuff(UberCharge, TF2.Time(8));
                            }
                            HealNPC(npc);
                        }
                        Projectile.Center = target.Center;
                        if (npc.ModNPC is Buddy buddy)
                        {
                            p.isHealing = true;
                            float moveSpeed = buddy.BaseSpeed * buddy.speedMultiplier;
                            if (moveSpeed > Player.moveSpeed)
                                p.healingMovementSpeed = moveSpeed;
                        }
                    }
                }
                Player.itemTime = 2;
                Player.itemAnimation = 2;
                Projectile.timeLeft = TF2.Time(0.05);
                int direction = Projectile.Center.X > Player.position.X ? 1 : -1;
                Player.ChangeDir(direction);
                Player.itemRotation = (Utils.DirectionTo(Player.Center, Projectile.Center) * direction).ToRotation();
                if (!Player.controlUseItem)
                    Projectile.Kill();
            }
            float length = Vector2.Distance(Player.Center, Projectile.Center);
            for (int i = 0; i < TF2.Round(length / 25f); i++)
            {
                Vector2 position = TF2.Lerp(Player.Center, Projectile.Center, Main.rand.NextFloat());
                Dust dust = Main.dust[Dust.NewDust(position - Vector2.One * 10, 20, 20, DustID.RedTorch)];
                dust.velocity = Player.velocity;
                dust.noGravity = true;
                dust.alpha = 128;
                dust.scale = Main.rand.Next(10, 20) * 0.1f;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
                p.SyncPlayer(-1, Main.myPlayer, false);
        }

        public override bool ShouldUpdatePosition() => false;

        protected static Player GetNearestPlayer(Projectile projectile, float maxDetectDistance)
        {
            Player playerFound = null;
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;
            foreach (Player player in Main.ActivePlayers)
            {
                float sqrDistanceToTarget = Vector2.DistanceSquared(player.Center, projectile.Center);
                if (sqrDistanceToTarget < sqrMaxDetectDistance && player != Main.player[projectile.owner] && !player.dead && !player.hostile)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    playerFound = player;
                }
            }
            return playerFound;
        }

        protected static NPC GetNearestBuddy(Projectile projectile, float maxDetectDistance)
        {
            NPC npcFound = null;
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                float sqrDistanceToTarget = Vector2.DistanceSquared(npc.Center, projectile.Center);
                if (sqrDistanceToTarget < sqrMaxDetectDistance && npc.ModNPC is Buddy)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    npcFound = npc;
                }
            }
            return npcFound;
        }

        protected static bool WithinHealRange(Player player, Projectile projectile, Entity target) => Vector2.Distance(player.Center, target.Center) <= Vector2.Distance(player.Center, projectile.Center);

        protected void HealPlayer(Player target)
        {
            if (HealCooldown <= 0)
            {
                int healingAmount = TF2.Round(TF2.GetRawHealth(target, (!target.GetModPlayer<TF2Player>().HealPenalty ? 1.2f : 0.4f) * HealMultiplier));
                if (healingAmount <= 0)
                    healingAmount = 1;
                TF2.OverhealMultiplayer(target, healingAmount, OverhealLimit);
                if (Player.HasBuff<QuickFixUberCharge>())
                    TF2.Overheal(Player, healingAmount, OverhealLimit);
                HealCooldown = TF2.Time(0.1);
            }
        }

        protected void HealNPC(NPC target)
        {
            if (target.ModNPC is not Buddy buddy) return;
            if (HealCooldown <= 0)
            {
                int healingAmount = TF2.Round(buddy.healthMultiplier * (!buddy.HealPenalty ? 1.2f : 0.4f) * HealMultiplier);
                if (healingAmount <= 0)
                    healingAmount = 1;
                if (Main.netMode == NetmodeID.SinglePlayer)
                    TF2.OverhealNPC(target, healingAmount, OverhealLimit);
                else
                    TF2.OverhealNPCMultiplayer(target, healingAmount, OverhealLimit);
                if (Player.HasBuff<QuickFixUberCharge>())
                    TF2.Overheal(Player, TF2.Round(TF2.GetRawHealth(Player, 0.4f * HealMultiplier)), OverhealLimit);
                HealCooldown = TF2.Time(0.1);
            }
        }

        protected void AddUberCharge(Item item)
        {
            if (HealCooldown <= 0)
                (item.ModItem as TF2Weapon).AddUberCharge(UberchargeMultiplier);
        }
    }
}