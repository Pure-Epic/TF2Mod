using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Items.Weapons.Sniper;

namespace TF2.Content.Projectiles.Sniper
{
    public class SydneySleeperDart : Bullet
    {
        public int jarateDuration;

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(30, 10);
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = TF2.Time(6);
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 9;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override void ProjectileAI()
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (Projectile.Hitbox.Intersects(player.Hitbox) && player.whoAmI != Projectile.owner && !player.hostile)
                {
                    HitPlayer(player);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
                    Projectile.Kill();
                }
            }
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (Projectile.Hitbox.Intersects(npc.Hitbox) && npc.friendly)
                {
                    HitNPC(npc);
                    Projectile.Kill();
                }
            }
            SetRotation();
        }

        protected override void ProjectilePostHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!info.PvP) return;
            target.AddBuff(ModContent.BuffType<JarateDebuff>(), jarateDuration, true);
            if (miniCrit)
            {
                for (int i = 0; i < Player.inventory.Length; i++)
                {
                    Item item = Player.inventory[i];
                    if (item.ModItem is Jarate weapon)
                        weapon.timer[0] -= TF2.Time(1);
                }
            }
        }

        protected override void ProjectilePostHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<JarateDebuff>(), jarateDuration);
            if (miniCrit)
            {
                for (int i = 0; i < Player.inventory.Length; i++)
                {
                    Item item = Player.inventory[i];
                    if (item.ModItem is Jarate weapon)
                        weapon.timer[0] -= TF2.Time(1);
                }
            }
        }

        protected virtual void HitPlayer(Player targetPlayer)
        {
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                int buffTypes = targetPlayer.buffType[i];
                if (Main.debuff[buffTypes] && targetPlayer.buffTime[i] > 0 && !BuffID.Sets.NurseCannotRemoveDebuff[buffTypes] && !TF2BuffBase.cooldownBuff[buffTypes])
                {
                    targetPlayer.DelBuff(i);
                    i = -1;
                }
            }
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                Item item = Player.inventory[i];
                if (item.ModItem is Jarate weapon)
                    weapon.timer[0] -= TF2.Time(4);
            }
        }

        protected virtual void HitNPC(NPC npc)
        {
            for (int i = 0; i < NPC.maxBuffs; i++)
            {
                int buffTypes = npc.buffType[i];
                if (Main.debuff[buffTypes] && npc.buffTime[i] > 0)
                {
                    npc.DelBuff(i);
                    i = -1;
                }
            }
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                Item item = Player.inventory[i];
                if (item.ModItem is Jarate weapon)
                    weapon.timer[0] -= TF2.Time(4);
            }
        }

        protected override void ProjectileSendExtraAI(BinaryWriter writer) => writer.Write(jarateDuration);

        protected override void ProjectileReceiveExtraAI(BinaryReader binaryReader) => jarateDuration = binaryReader.ReadInt32();
    }
}