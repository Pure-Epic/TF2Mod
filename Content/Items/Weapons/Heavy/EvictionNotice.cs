using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Heavy;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Heavy
{
    public class EvictionNotice : TF2Weapon
    {
        protected override string ArmTexture => "TF2/Content/Textures/Items/Heavy/EvictionNotice";

        protected override int HealthBoost => -20;

        protected override bool TemporaryHealthBoost => true;

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Melee, Unique, Craft);
            SetLungeUseStyle();
            SetWeaponDamage(damage: 26, projectile: ModContent.ProjectileType<EvictionNoticeProjectile>(), projectileSpeed: 2f);
            SetWeaponAttackSpeed(0.48);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponAttackIntervals(holster: 1, altClick: true);
            SetWeaponPrice(weapon: 1, reclaimed: 3);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponAttackAnimation(Player player) => Item.noUseGraphic = true;

        protected override bool WeaponAddTextureCondition(Player player) => HoldingWeapon<EvictionNotice>(player);

        protected override bool WeaponModifyHealthCondition(Player player) => HoldingWeapon<EvictionNotice>(player);

        protected override void WeaponActiveBonus(Player player) => TF2Player.SetPlayerSpeed(player, 115);

        public override void AddRecipes() => CreateRecipe().AddIngredient<FistsOfSteel>().AddIngredient<ReclaimedMetal>(2).AddTile<CraftingAnvil>().Register();
    }
}