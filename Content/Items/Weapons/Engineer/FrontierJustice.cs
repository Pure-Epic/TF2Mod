using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.Weapons.Engineer
{
    public class FrontierJustice : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Engineer, Primary, Unique, Unlock);
            SetWeaponSize(50, 18);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 6, projectile: ModContent.ProjectileType<Bullet>(), projectileCount: 10, shootAngle: 12f, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.625);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/shotgun_shoot");
            SetWeaponAttackIntervals(maxAmmo: 3, maxReserve: 32, reloadTime: 0.5, initialReloadTime: 1, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/shotgun_reload");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<FrontierJusticePlayer>().frontierJusticeEquipped = true;

        protected override void WeaponPostAttack(Player player)
        {
            FrontierJusticePlayer f = player.GetModPlayer<FrontierJusticePlayer>();
            if (f.revenge > 0)
                f.revenge--;
        }

        protected override void WeaponPostFireProjectile(Player player, int projectile)
        {
            if (player.GetModPlayer<FrontierJusticePlayer>().revenge > 0)
                (Main.projectile[projectile].ModProjectile as TF2Projectile).crit = true;
        }
    }

    public class FrontierJusticePlayer : ModPlayer
    {
        public bool frontierJusticeEquipped;
        public int revenge = 0;
        public int hitCounter = 0;

        public override void ResetEffects() => frontierJusticeEquipped = false;

        public override void PreUpdate()
        {
            if (revenge < 0)
                revenge = 0;
        }

        public override void OnRespawn() => revenge = 0;

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (!frontierJusticeEquipped) return;
            hitCounter++;
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (!frontierJusticeEquipped) return;
            if (proj.ModProjectile is not TF2Projectile projectile || (proj.ModProjectile == projectile && projectile.spawnedFromNPC))
                hitCounter++;
        }

        public override void PostHurt(Player.HurtInfo info)
        {
            if (!frontierJusticeEquipped) return;
            if (info.PvP)
                hitCounter++;
            if (hitCounter >= 3)
            {
                revenge++;
                hitCounter = 0;
            }
        }
    }
}