using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.NPCs.Buddies;

namespace TF2.Content.Projectiles.Engineer
{
    public class DisciplinaryActionHitbox : TF2Projectile
    {
        public override string Texture => "TF2/Content/Items/Weapons/Soldier/DisciplinaryAction";

        protected override void ProjectileStatistics()
        {
            SetProjectileSize(50, 50);
            Projectile.penetrate = -1;
            Projectile.timeLeft = TF2.Time(0.8);
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        protected override void ProjectileAI()
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.Center = Player.Center;
            foreach (Player player in Main.ActivePlayers)
            {
                if (TF2.MeleeHitbox(Player).Intersects(player.Hitbox) && player.whoAmI != Projectile.owner && !player.dead && !player.hostile && Main.dedServ)
                {
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/disciplinary_action_hit"), player.Center);
                    if (!Player.HasBuff<DisciplinaryActionBuff>())
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/disciplinary_action_power_up"), Player.Center);
                    player.AddBuff(ModContent.BuffType<DisciplinaryActionBuff>(), TF2.Time(2), false);
                    Player.AddBuff(ModContent.BuffType<DisciplinaryActionBuff>(), TF2.Time(3));
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
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (TF2.MeleeHitbox(Player).Intersects(npc.Hitbox) && npc.ModNPC is Buddy)
                {
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/disciplinary_action_hit"), npc.Center);
                    if (!Player.HasBuff<DisciplinaryActionBuff>())
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/disciplinary_action_power_up"), Player.Center);
                    npc.AddBuff(ModContent.BuffType<DisciplinaryActionBuff>(), TF2.Time(2));
                    Player.AddBuff(ModContent.BuffType<DisciplinaryActionBuff>(), TF2.Time(3));
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
        }
    }
}