using Terraria;
using Terraria.ID;
using TF2.Common;
using TF2.Content.Projectiles.Medic;

namespace TF2.Content.Projectiles.NPCs
{
    public class SyringeNPC : CrusadersCrossbowSyringe
    {
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.GetGlobalProjectile<TF2ProjectileBase>().healingProjectile = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            foreach (Player player in Main.player)
            {
                if (Projectile.Hitbox.Intersects(player.Hitbox) && player.active && !healedPlayer[player.whoAmI])
                {
                    TF2Player p = player.GetModPlayer<TF2Player>();
                    int healingAmount = (int)(75 * player.statLifeMax2 / 500 * p.healReduction);
                    player.Heal(healingAmount);
                    healedPlayer[player.whoAmI] = true;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
                    Projectile.penetrate--;
                }
            }
        }
    }
}