using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Sniper;
using TF2.Content.Projectiles.Medic;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Medic
{
    public class CrusadersCrossbow : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Primary, Unique, Craft);
            SetWeaponSize(40, 40);
            SetGunUseStyle(focus: true);
            SetWeaponDamage(damage: 75, projectile: ModContent.ProjectileType<CrusadersCrossbowSyringe>(), projectileSpeed: 12.5f);
            SetWeaponAttackSpeed(0.24);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/crusaders_crossbow_shoot");
            SetWeaponAttackIntervals(noAmmo: true, customReloadTime: 1.75, reloadSoundPath: "TF2/Content/Sounds/SFX/syringegun_reload");
            SetWeaponPrice(weapon: 1, scrap: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player) => cooldownTimer >= Time(1.75);

        protected override bool WeaponCanConsumeAmmo(Player player) => true;

        protected override void WeaponActiveUpdate(Player player)
        {
            if (!finishReloadSound && cooldownTimer == Time(0.24))
            {
                SoundEngine.PlaySound(reloadSound, player.Center);
                finishReloadSound = true;
            }
            if (cooldownTimer >= Time(1.75))
                finishReloadSound = false;
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            cooldownTimer++;
            if (cooldownTimer > Time(1.75))
                cooldownTimer = Time(1.75);
        }

        protected override bool WeaponPreAttack(Player player)
        {
            cooldownTimer = 0;
            return base.WeaponPreAttack(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ScrapMetal>(2)
                .AddIngredient<Huntsman>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}