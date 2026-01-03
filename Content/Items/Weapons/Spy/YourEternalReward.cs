using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Spy;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Spy
{
    public class YourEternalReward : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Spy, Melee, Unique, Craft);
            SetLungeUseStyle(knife: true);
            SetWeaponDamage(damage: 40, projectile: ModContent.ProjectileType<YourEternalRewardProjectile>(), projectileSpeed: 2f, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/knife_swing");
            SetKnife(maxChargeDuration: 1);
            SetWeaponPrice(weapon: 1, reclaimed: 1);
            noThe = true;
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public override bool KnifeCanBackstab(Player player) => player.GetModPlayer<InvisWatchBuffPlayer>().fullCloak || player.GetModPlayer<CloakAndDaggerBuffPlayer>().fullCloak || player.GetModPlayer<DeadRingerPlayer>().fullCloak;

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<YourEternalRewardPlayer>().yourEternalRewardEquipped = true;

        protected override void WeaponPostFireProjectile(Player player, int projectile)
        {
            if (Main.projectile[projectile].ModProjectile is YourEternalRewardProjectile knife && knife.backstab)
            {
                if (player.GetModPlayer<InvisWatchBuffPlayer>().invisWatchEquipped)
                    player.GetModPlayer<InvisWatchBuffPlayer>().cloakMeter = 0;
                if (player.GetModPlayer<CloakAndDaggerBuffPlayer>().cloakAndDaggerEquipped)
                    player.GetModPlayer<CloakAndDaggerBuffPlayer>().cloakMeter = 0;
                if (player.GetModPlayer<DeadRingerPlayer>().deadRingerEquipped)
                    player.GetModPlayer<DeadRingerPlayer>().cloakMeter = 0;
            }
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<CloakAndDagger>().AddIngredient<ReclaimedMetal>().AddTile<AustraliumAnvil>().Register();
    }

    public class YourEternalRewardPlayer : ModPlayer
    {
        public bool yourEternalRewardEquipped;

        public override void ResetEffects() => yourEternalRewardEquipped = false;
    }
}