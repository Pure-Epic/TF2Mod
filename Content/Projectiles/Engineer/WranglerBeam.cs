using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Projectiles.Engineer
{
    public class WranglerBeam : TF2Projectile
    {
        public override string Texture => "TF2/Content/Items/Weapons/Engineer/Wrangler";

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(25, 25);
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
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
                    if (Main.dedServ)
                    {
                        ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
                        packet.Write((byte)TF2.MessageType.KillProjectile);
                        packet.Write((byte)Projectile.whoAmI);
                        packet.Send();
                    }
                    Projectile.Kill();
                }
                if (Main.netMode != NetmodeID.SinglePlayer)
                    Player.GetModPlayer<TF2Player>().SyncPlayer(-1, Main.myPlayer, false);
            }
        }

        public override bool ShouldUpdatePosition() => false;
    }
}