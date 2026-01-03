using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Pyro
{
    public class Fire : TF2Projectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Flames;

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(6, 6);
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = TF2.Time(1);
            Projectile.alpha = 255;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override bool ProjectileDraw(Projectile projectile, ref Color lightColor)
        {
            float num = 60f;
            float num10 = 12f;
            float fromMax = num + num10;
            Texture2D value = TextureAssets.Projectile[Projectile.type].Value;
            Color color = new Color(255, 80, 20, 200);
            Color color2 = new Color(255, 255, 20, 70);
            Color color3 = Color.Lerp(new Color(255, 80, 20, 100), color2, 0.25f);
            Color color4 = new Color(80, 80, 80, 100);
            float num11 = 0.35f;
            float num12 = 0.7f;
            float num13 = 0.85f;
            float num14 = (Timer > num - 10f) ? 0.175f : 0.2f;
            int verticalFrames = 7;
            float num15 = Utils.Remap(Timer, num, fromMax, 1f, 0f);
            float num2 = Math.Min(Timer, 20f);
            float num3 = Utils.Remap(Timer, 0f, fromMax, 0f, 1f);
            float num4 = Utils.Remap(num3, 0.2f, 0.5f, 0.25f, 1f);
            Rectangle rectangle = value.Frame(1, verticalFrames, 0, 3);
            for (int i = 0; i < 2; i++)
            {
                for (float num5 = 1f; num5 >= 0f; num5 -= num14)
                {
                    Color val = (num3 < 0.1f) ? Color.Lerp(Color.Transparent, color, Utils.GetLerpValue(0f, 0.1f, num3, clamped: true)) : ((num3 < 0.2f) ? Color.Lerp(color, color2, Utils.GetLerpValue(0.1f, 0.2f, num3, clamped: true)) : ((num3 < num11) ? color2 : ((num3 < num12) ? Color.Lerp(color2, color3, Utils.GetLerpValue(num11, num12, num3, clamped: true)) : ((num3 < num13) ? Color.Lerp(color3, color4, Utils.GetLerpValue(num12, num13, num3, clamped: true)) : ((!(num3 < 1f)) ? Color.Transparent : Color.Lerp(color4, Color.Transparent, Utils.GetLerpValue(num13, 1f, num3, clamped: true)))))));
                    float num6 = (1f - num5) * Utils.Remap(num3, 0f, 0.2f, 0f, 1f);
                    Vector2 vector = Projectile.Center - Main.screenPosition + Projectile.velocity * (0f - num2) * num5;
                    Color color5 = val * num6;
                    Color color6 = color5;
                    color6.G = (byte)(color6.G / 2);
                    color6.B = (byte)(color6.B / 2);
                    color6.A = (byte)Math.Min(color5.A + 80f * num6, 255f);
                    Utils.Remap(Timer, 20f, fromMax, 0f, 1f);
                    float num7 = 1f / num14 * (num5 + 1f);
                    float num8 = Projectile.rotation + num5 * ((float)Math.PI / 2f) + Main.GlobalTimeWrappedHourly * num7 * 2f;
                    float num9 = Projectile.rotation - num5 * ((float)Math.PI / 2f) - Main.GlobalTimeWrappedHourly * num7 * 2f;
                    switch (i)
                    {
                        case 0:
                            Main.EntitySpriteDraw(value, vector + Projectile.velocity * (0f - num2) * num14 * 0.5f, rectangle, color6 * num15 * 0.25f, num8 + (float)Math.PI / 4f, rectangle.Size() / 2f, num4, SpriteEffects.None);
                            Main.EntitySpriteDraw(value, vector, rectangle, color6 * num15, num9, rectangle.Size() / 2f, num4, SpriteEffects.None);
                            break;
                        case 1:
                            Main.EntitySpriteDraw(value, vector + Projectile.velocity * (0f - num2) * num14 * 0.2f, rectangle, color5 * num15 * 0.25f, num8 + (float)Math.PI / 2f, rectangle.Size() / 2f, num4 * 0.75f, SpriteEffects.None);
                            Main.EntitySpriteDraw(value, vector, rectangle, color5 * num15, num9 + (float)Math.PI / 2f, rectangle.Size() / 2f, num4 * 0.75f, SpriteEffects.None);
                            break;
                    }
                }
            }
            return false;
        }

        protected override void ProjectileAI()
        {
            if (Projectile.wet)
                Projectile.Kill();
            foreach (Player player in Main.ActivePlayers)
            {
                if (Projectile.Hitbox.Intersects(player.Hitbox) && !player.dead && !player.hostile)
                    player.GetModPlayer<TF2Player>().igniteArrow = true;
            }
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3());
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = 30;
            hitbox.X -= size;
            hitbox.Y -= size;
            hitbox.Width += size * 2;
            hitbox.Height += size * 2;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => projHitbox.Intersects(targetHitbox) && Collision.CanHit(Projectile.Center, 0, 0, projHitbox.Center.ToVector2(), 0, 0);

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            FlameThrowerDebuffPlayer burntPlayer = target.GetModPlayer<FlameThrowerDebuffPlayer>();
            burntPlayer.damageMultiplier = p.damageMultiplier;
            target.ClearBuff(ModContent.BuffType<DegreaserDebuff>());
            target.AddBuff(ModContent.BuffType<FlameThrowerDebuff>(), TF2.Time(10), true);
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            FlameThrowerDebuffNPC npc = target.GetGlobalNPC<FlameThrowerDebuffNPC>();
            npc.damageMultiplier = p.damageMultiplier;
            TF2.ExtinguishPyroFlames(target, ModContent.BuffType<DegreaserDebuff>());
            target.AddBuff(ModContent.BuffType<FlameThrowerDebuff>(), TF2.Time(10));
        }
    }
}