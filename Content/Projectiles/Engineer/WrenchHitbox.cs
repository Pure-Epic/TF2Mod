using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.NPCs.Buildings;
using TF2.Content.NPCs.Buildings.Dispenser;
using TF2.Content.NPCs.Buildings.SentryGun;
using TF2.Content.NPCs.Buildings.Teleporter;

namespace TF2.Content.Projectiles.Engineer
{
    public class WrenchHitbox : TF2Projectile
    {
        public override string Texture => "TF2/Content/Items/Weapons/Engineer/Wrench";

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
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (TF2.MeleeHitbox(Player).Intersects(npc.Hitbox) && npc.ModNPC is Building && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    HitBuilding(npc);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
                        packet.Write((byte)TF2.MessageType.KillProjectile);
                        packet.Write((byte)Projectile.whoAmI);
                        packet.Send();
                    }
                    Projectile.Kill();
                }
            }
        }

        protected void HitBuilding(NPC building)
        {
            float healMultiplier = 1f;
            int cost = 0;
            bool success = false;
            TF2Player p = Player.GetModPlayer<TF2Player>();
            Building buildingNPC = building.ModNPC as Building;
            if (!buildingNPC.Initialized && Building.BaseLevel(buildingNPC))
            {
                buildingNPC.constructionSpeed += 2.5f * p.constructionSpeedMultiplier;
                building.netUpdate = true;
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success"));
            }
            if (buildingNPC is TF2Sentry sentry && buildingNPC.Initialized)
            {
                healMultiplier = sentry.wrangled ? 0.66666f : 1f;
                int health = ((building.lifeMax - building.life) / p.healthMultiplier) >= 102 ? 102 : TF2.Round((building.lifeMax - building.life) / p.healthMultiplier);
                int damageRepaired = TF2.Round(health * healMultiplier * p.repairRateMultiplier);
                cost = TF2.Round(damageRepaired / 3f);
                if (cost > 0)
                {
                    if (cost <= p.metal && p.metal > 0)
                    {
                        building.life += TF2.Round(damageRepaired * p.healthMultiplier);
                        p.metal -= cost;
                        building.HealEffect(cost);
                        building.netUpdate = true;
                        success = true;
                    }
                    else
                    {
                        building.life += TF2.Round(damageRepaired * (float)p.metal / cost * p.healthMultiplier);
                        p.metal = 0;
                        building.HealEffect(cost);
                        building.netUpdate = true;
                        success = true;
                    }
                }
                if (buildingNPC.Metal < 200 && !Building.MaxLevel(buildingNPC))
                {
                    cost = (p.metal >= 25) ? 25 : p.metal;
                    sentry.Metal += cost;
                    p.metal -= cost;
                    building.netUpdate = true;
                    success = cost > 0;
                }
                int ammo = (sentry.Rounds - sentry.Ammo >= 40) ? 40 : (sentry.Rounds - sentry.Ammo);
                if (ammo <= p.metal && ammo > 0)
                {
                    cost = ammo;
                    sentry.Ammo += cost;
                    p.metal -= cost;
                    building.netUpdate = true;
                    success = cost > 0;
                }
                else if (ammo > 0)
                {
                    cost = p.metal;
                    sentry.Ammo += cost;
                    p.metal -= cost;
                    building.netUpdate = true;
                    success = cost > 0;
                }
                int rocketAmmo = (20 - sentry.RocketAmmo >= 8) ? 16 : ((20 - sentry.RocketAmmo) * 2);
                if (rocketAmmo <= p.metal && rocketAmmo > 0)
                {
                    cost = rocketAmmo;
                    sentry.RocketAmmo += cost / 2;
                    p.metal -= cost;
                    building.netUpdate = true;
                    success = cost > 0;
                }
                else if (rocketAmmo > 0 && p.metal > 1)
                {
                    cost = p.metal;
                    sentry.RocketAmmo += cost / 2;
                    p.metal -= cost;
                    building.netUpdate = true;
                    success = cost > 0;
                }
                SoundEngine.PlaySound(new SoundStyle(success ? "TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success" : "TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_fail"));
            }
            else if (buildingNPC is TF2Dispenser dispenser && buildingNPC.Initialized)
            {
                int health = ((building.lifeMax - building.life) / p.healthMultiplier) >= 102 ? 102 : TF2.Round((building.lifeMax - building.life) / p.healthMultiplier);
                int damage = TF2.Round(health * healMultiplier);
                cost = TF2.Round(damage / 3f);
                if (cost > 0)
                {
                    if (cost <= p.metal && p.metal > 0)
                    {
                        building.life += TF2.Round(damage * p.healthMultiplier);
                        p.metal -= cost;
                        building.HealEffect(cost);
                        building.netUpdate = true;
                        success = true;
                    }
                    else
                    {
                        building.life += TF2.Round(damage * (float)p.metal / cost * p.healthMultiplier);
                        p.metal = 0;
                        building.HealEffect(cost);
                        building.netUpdate = true;
                        success = true;
                    }
                }
                if (buildingNPC.Metal < 200 && !Building.MaxLevel(buildingNPC))
                {
                    cost = (p.metal >= 25) ? 25 : p.metal;
                    dispenser.Metal += cost;
                    p.metal -= cost;
                    building.netUpdate = true;
                    success = cost > 0;
                }
                SoundEngine.PlaySound(new SoundStyle(success ? "TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success" : "TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_fail"));
            }
            else if (buildingNPC is TF2Teleporter teleporter && buildingNPC.Initialized)
            {
                int health = ((building.lifeMax - building.life) / p.healthMultiplier) >= 102 ? 102 : TF2.Round((building.lifeMax - building.life) / p.healthMultiplier);
                int damage = TF2.Round(health * healMultiplier);
                cost = TF2.Round(damage / 3f);
                if (cost <= p.metal && cost > 0 && p.metal > 0)
                {
                    building.life += TF2.Round(damage * p.healthMultiplier);
                    p.metal -= cost;
                    building.HealEffect(cost);
                    building.netUpdate = true;
                    success = true;
                }
                else if (cost > 0)
                {
                    building.life += TF2.Round(damage * (float)p.metal / cost * p.healthMultiplier);
                    p.metal = 0;
                    building.HealEffect(cost);
                    building.netUpdate = true;
                    success = true;
                }
                if (buildingNPC.Metal < 200 && !Building.MaxLevel(buildingNPC))
                {
                    cost = (p.metal >= 25) ? 25 : p.metal;
                    teleporter.Metal += cost;
                    building.netUpdate = true;
                    TeleporterStatistics carriedTeleporter = Main.player[teleporter.Owner].GetModPlayer<TF2Player>().carriedTeleporter;
                    if (carriedTeleporter != null)
                        carriedTeleporter.Metal += cost;
                    if (teleporter is TeleporterEntrance)
                    {
                        (teleporter as TeleporterEntrance).FindTeleporterExit(out TeleporterExit exit);
                        if (exit != null)
                        {
                            exit.Metal += cost;
                            exit.NPC.netUpdate = true;
                        }
                    }
                    else if (teleporter is TeleporterExit)
                    {
                        (teleporter as TeleporterExit).FindTeleporterEntrance(out TeleporterEntrance entrance);
                        if (entrance != null)
                        {
                            entrance.Metal += cost;
                            entrance.NPC.netUpdate = true;
                        }
                    }
                    p.metal -= cost;
                    success = cost > 0;
                }
                SoundEngine.PlaySound(new SoundStyle(success ? "TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success" : "TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_fail"));
            }
        }
    }
}