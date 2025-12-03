using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.NPCs.Buddies;

namespace TF2.Content.Projectiles.Medic
{
    public class CrusadersCrossbowSyringe : Syringe
    {
        public override string Texture => "TF2/Content/Projectiles/Medic/Syringe";

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(30, 10);
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            healingProjectile = true;
        }

        protected override void ProjectileAI()
        {
            SetRotation();
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (Projectile.Hitbox.Intersects(npc.Hitbox) && npc.friendly && npc.ModNPC is Buddy buddy && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int healingAmount = TF2.Round(npc.lifeMax / buddy.BaseHealth * BaseHealingAmount(npc));
                    buddy.Heal(healingAmount);
                    if (Main.dedServ)
                    {
                        ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
                        packet.Write((byte)TF2.MessageType.KillProjectile);
                        packet.Write((byte)Projectile.whoAmI);
                        packet.Send();
                    }
                    Projectile.Kill();
                    break;
                }
            }
            if (Main.netMode == NetmodeID.SinglePlayer) return;
            foreach (Player player in Main.ActivePlayers)
            {
                if (Projectile.Hitbox.Intersects(player.Hitbox) && player.whoAmI != Projectile.owner && !player.dead && !player.hostile && Main.dedServ)
                {
                    TF2Player p = player.GetModPlayer<TF2Player>();
                    int healingAmount = TF2.Round(TF2.GetHealth(player, BaseHealingAmount(player)) * p.healReduction);
                    player.Heal(healingAmount);
                    if (Main.dedServ)
                    {
                        ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
                        packet.Write((byte)TF2.MessageType.SyncSyringe);
                        packet.Write((byte)Projectile.whoAmI);
                        packet.Write((byte)player.whoAmI);
                        packet.Write(healingAmount);
                        packet.Send();
                        packet.Write((byte)TF2.MessageType.KillProjectile);
                        packet.Write((byte)Projectile.whoAmI);
                        packet.Send();
                    }
                    Projectile.Kill();
                    break;
                }
            }
        }

        private float BaseHealingAmount(Entity target) => 150f * (0.5f + Utils.Clamp(Vector2.Distance(Player.Center, target.Center) / 500f, 0f, 0.5f));
    }
}