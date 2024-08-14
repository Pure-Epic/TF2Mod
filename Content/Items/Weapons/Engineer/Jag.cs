using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Engineer;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Engineer
{
    public class Jag : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Engineer, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 49);
            SetWeaponAttackSpeed(0.68);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/wrench_swing");
            SetWeaponPrice(weapon: 1, scrap: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            player.GetModPlayer<TF2Player>().constructionSpeedMultiplier *= 1.3f;
            player.GetModPlayer<TF2Player>().repairRateMultiplier *= 0.8f;
        }

        protected override bool? WeaponOnUse(Player player)
        {
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<WrenchHitbox>(), 1, 0f);
            return true;
        }

        protected override void WeaponHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers) => modifiers.SourceDamage *= target.boss ? 1f : 0.66f;

        public override void AddRecipes() => CreateRecipe().AddIngredient<SouthernHospitality>().AddIngredient<ScrapMetal>().AddTile<CraftingAnvil>().Register();
    }
}