using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Heavy;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Heavy
{
    public class WarriorsSpirit : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Melee, Unique, Craft);
            SetLungeUseStyle();
            SetWeaponDamage(damage: 85, projectile: ModContent.ProjectileType<WarriorsSpiritProjectile>(), projectileSpeed: 2f);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/melee_swing");
            SetWeaponAttackIntervals(altClick: true);
            SetWeaponPrice(weapon: 1, scrap: 3);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddHeader(description);
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponAttackAnimation(Player player) => Item.noUseGraphic = true;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ScrapMetal>()
                .AddIngredient<GlovesOfRunningUrgently>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class WarriorsSpiritPlayer : ModPlayer
    {
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (Player.HeldItem.ModItem is WarriorsSpirit)
                modifiers.FinalDamage *= 1.3f;
        }
    }
}