using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles.Medic;

namespace TF2.Content.Projectiles.NPCs
{
    public class SyringeNPC : CrusadersCrossbowSyringe
    {
        protected override void ProjectileStatistics()
        {
            SetProjectileSize(50, 8);
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            healingProjectile = true;
        }

        protected override void ProjectileAI()
        {
            SetRotation();
            foreach (Player player in Main.player)
            {
                if (Projectile.Hitbox.Intersects(player.Hitbox) && player.whoAmI != Projectile.owner && player.active && !player.hostile && (Main.netMode == NetmodeID.SinglePlayer || Main.netMode == NetmodeID.Server))
                {
                    TF2Player p = player.GetModPlayer<TF2Player>();
                    int healingAmount = TF2.Round(TF2.GetHealth(player, 75) * p.healReduction);
                    player.Heal(healingAmount);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SpiritHeal, number: player.whoAmI, number2: healingAmount);
                        ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
                        packet.Write((byte)TF2.MessageType.SyncSyringe);
                        packet.Write((byte)Projectile.whoAmI);
                        packet.Send();
                    }
                    Projectile.Kill();
                }
            }
        }
    }
}