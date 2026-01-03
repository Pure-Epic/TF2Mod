using Terraria;
using Terraria.Audio;
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
            else return;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (TF2.MeleeHitbox(Player).Intersects(npc.Hitbox) && npc.ModNPC is Building)
                {
                    HitBuilding(npc);
                    if (Main.dedServ)
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
            int cost;
            bool success = false;
            TF2Player p = Player.GetModPlayer<TF2Player>();
            Building buildingNPC = building.ModNPC as Building;
            if (!buildingNPC.Initialized && buildingNPC.BaseLevel())
            {
                buildingNPC.constructionSpeed += 2.5f * p.constructionSpeedMultiplier;
                buildingNPC.SyncBuilding();
                TF2.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success"), building.position);
            }
            if (buildingNPC is TF2Sentry sentry && buildingNPC.Initialized)
            {
                healMultiplier = sentry.wrangled ? 0.66666f : 1f;
                int health = ((building.lifeMax - building.life) / buildingNPC.healthMultiplier) >= 102 ? 102 : TF2.Round((building.lifeMax - building.life) / buildingNPC.healthMultiplier);
                int damageRepaired = TF2.Round(health * healMultiplier * p.repairRateMultiplier);
                cost = TF2.Round(damageRepaired / 3f);
                if (cost > 0)
                {
                    if (cost <= p.metal && p.metal > 0)
                    {
                        buildingNPC.Repair(TF2.Round(damageRepaired * buildingNPC.healthMultiplier));
                        p.metal -= cost;
                        buildingNPC.SyncBuilding();
                        success = true;
                    }
                    else
                    {
                        buildingNPC.Repair(TF2.Round(damageRepaired * (float)p.metal / cost * buildingNPC.healthMultiplier));
                        p.metal = 0;
                        buildingNPC.SyncBuilding();
                        success = true;
                    }
                }
                if (buildingNPC.Metal < 200 && !buildingNPC.MaxLevel())
                {
                    cost = (p.metal >= 25) ? 25 : p.metal;
                    sentry.Metal += cost;
                    p.metal -= cost;
                    buildingNPC.SyncBuilding();
                    success = cost > 0;
                }
                int ammo = (sentry.Rounds - sentry.Ammo >= 40) ? 40 : (sentry.Rounds - sentry.Ammo);
                if (ammo <= p.metal && ammo > 0)
                {
                    cost = ammo;
                    sentry.AddSentryAmmo(cost);
                    p.metal -= cost;
                    buildingNPC.SyncBuilding();
                    success = cost > 0;
                }
                else if (ammo > 0)
                {
                    cost = p.metal;
                    sentry.AddSentryAmmo(cost);
                    p.metal -= cost;
                    buildingNPC.SyncBuilding();
                    success = cost > 0;
                }
                if (sentry is SentryLevel3)
                {
                    int rocketAmmo = (20 - sentry.RocketAmmo >= 8) ? 16 : ((20 - sentry.RocketAmmo) * 2);
                    if (rocketAmmo <= p.metal && rocketAmmo > 0)
                    {
                        cost = rocketAmmo;
                        sentry.AddSentryRocketAmmo(cost / 2);
                        p.metal -= cost;
                        buildingNPC.SyncBuilding();
                        success = cost > 0;
                    }
                    else if (rocketAmmo > 0 && p.metal > 1)
                    {
                        cost = p.metal;
                        sentry.AddSentryRocketAmmo(cost / 2);
                        p.metal -= cost;
                        buildingNPC.SyncBuilding();
                        success = cost > 0;
                    }
                }
                TF2.PlaySound(new SoundStyle(success ? "TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success" : "TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_fail"), building.position);
            }
            else if (buildingNPC is TF2Dispenser dispenser && buildingNPC.Initialized)
            {
                int health = ((building.lifeMax - building.life) / buildingNPC.healthMultiplier) >= 102 ? 102 : TF2.Round((building.lifeMax - building.life) / buildingNPC.healthMultiplier);
                int damageRepaired = TF2.Round(health * healMultiplier * p.repairRateMultiplier);
                cost = TF2.Round(damageRepaired / 3f);
                if (cost > 0)
                {
                    if (cost <= p.metal && p.metal > 0)
                    {
                        buildingNPC.Repair(TF2.Round(damageRepaired * buildingNPC.healthMultiplier));
                        p.metal -= cost;
                        buildingNPC.SyncBuilding();
                        success = true;
                    }
                    else
                    {
                        buildingNPC.Repair(TF2.Round(damageRepaired * (float)p.metal / cost * buildingNPC.healthMultiplier));
                        p.metal = 0;
                        buildingNPC.SyncBuilding();
                        success = true;
                    }
                }
                if (buildingNPC.Metal < 200 && !buildingNPC.MaxLevel())
                {
                    cost = (p.metal >= 25) ? 25 : p.metal;
                    dispenser.Metal += cost;
                    p.metal -= cost;
                    buildingNPC.SyncBuilding();
                    success = cost > 0;
                }
                TF2.PlaySound(new SoundStyle(success ? "TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success" : "TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_fail"), building.position);
            }
            else if (buildingNPC is TF2Teleporter teleporter && buildingNPC.Initialized)
            {
                int health = ((building.lifeMax - building.life) / buildingNPC.healthMultiplier) >= 102 ? 102 : TF2.Round((building.lifeMax - building.life) / buildingNPC.healthMultiplier);
                int damageRepaired = TF2.Round(health * healMultiplier * p.repairRateMultiplier);
                cost = TF2.Round(damageRepaired / 3f);
                if (cost <= p.metal && cost > 0 && p.metal > 0)
                {
                    buildingNPC.Repair(TF2.Round(damageRepaired * buildingNPC.healthMultiplier));
                    p.metal -= cost;
                    buildingNPC.SyncBuilding();
                    success = true;
                }
                else if (cost > 0)
                {
                    buildingNPC.Repair(TF2.Round(damageRepaired * (float)p.metal / cost * buildingNPC.healthMultiplier));
                    p.metal = 0;
                    buildingNPC.SyncBuilding();
                    success = true;
                }
                if (buildingNPC.Metal < 200 && !buildingNPC.MaxLevel())
                {
                    cost = (p.metal >= 25) ? 25 : p.metal;
                    teleporter.Metal += cost;
                    buildingNPC.SyncBuilding();
                    TeleporterStatistics carriedTeleporter = Main.player[teleporter.Owner].GetModPlayer<TF2Player>().carriedTeleporter;
                    if (carriedTeleporter != null)
                        carriedTeleporter.Metal += cost;
                    if (teleporter is TeleporterEntrance)
                    {
                        (teleporter as TeleporterEntrance).FindTeleporterExit(out TeleporterExit exit);
                        if (exit != null)
                        {
                            exit.Metal += cost;
                            exit.SyncBuilding();
                        }
                    }
                    else if (teleporter is TeleporterExit)
                    {
                        (teleporter as TeleporterExit).FindTeleporterEntrance(out TeleporterEntrance entrance);
                        if (entrance != null)
                        {
                            entrance.Metal += cost;
                            entrance.SyncBuilding();
                        }
                    }
                    p.metal -= cost;
                    success = cost > 0;
                }
                TF2.PlaySound(new SoundStyle(success ? "TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success" : "TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_fail"), building.position);
            }
        }
    }
}