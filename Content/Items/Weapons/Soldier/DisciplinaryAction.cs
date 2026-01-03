using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.MultiClass;
using TF2.Content.Projectiles.Soldier;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class DisciplinaryAction : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Melee, Unique, Craft);
            SetSwingUseStyle(scale: 170);
            SetWeaponDamage(damage: 49);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/disciplinary_action_swing", "TF2/Content/Sounds/SFX/Weapons/disciplinary_action_hit");
            SetWeaponPrice(weapon: 1, reclaimed: 2, scrap: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override bool? WeaponOnUse(Player player)
        {
            TF2.CreateProjectile(this, player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<DisciplinaryActionHitbox>(), 0, 0f);
            return true;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<PainTrain>().AddIngredient<ReclaimedMetal>(2).AddTile<AustraliumAnvil>().Register();
    }
}