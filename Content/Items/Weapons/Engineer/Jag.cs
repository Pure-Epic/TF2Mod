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

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<JagPlayer>().jagEquipped = true;

        protected override bool? WeaponOnUse(Player player)
        {
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<WrenchHitbox>(), 1, 0f);
            return true;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers) => modifiers.SourceDamage *= target.boss ? 1f : 0.66f;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SouthernHospitality>()
                .AddIngredient<ScrapMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class JagPlayer : ModPlayer
    {
        public bool jagEquipped;

        public override void ResetEffects() => jagEquipped = false;

        public override void PostUpdate()
        {
            if (jagEquipped)
            {
                // Note that this doesn't change building cost, it's here for reference. The actual code resides in Engineer's PDAs themselves.
                TF2Player p = Player.GetModPlayer<TF2Player>();
                p.sentryCostReduction *= 0.7f;
                p.dispenserCostReduction *= 1.2f;
            }
        }
    }
}