using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Sniper;
using TF2.Content.Projectiles.Scout;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Scout
{
    public class MadMilk : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Secondary, Unique, Craft);
            SetWeaponSize(21, 50);
            SetThrowableUseStyle();
            SetWeaponDamage(projectile: ModContent.ProjectileType<MadMilkProjectile>());
            SetWeaponAttackSpeed(0.5, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/scout_taunts14");
            SetUtilityWeapon();
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNeutralAttribute(description);
            AddOtherAttribute(description, (string)this.GetLocalization("Notes2"));
        }

        public override bool WeaponCanBeUsed(Player player) => !player.GetModPlayer<MadMilkPlayer>().madMilkCooldown;

        protected override bool? WeaponOnUse(Player player)
        {
            player.AddBuff(ModContent.BuffType<MadMilkCooldown>(), 1200);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Jarate>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}