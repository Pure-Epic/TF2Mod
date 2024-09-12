using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Medic
{
    public class SolemnVow : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65);
            SetWeaponAttackSpeed(0.88);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponPrice(weapon: 8, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) => target.AddBuff(ModContent.BuffType<JarateDebuff>(), TF2.Time(5), false);

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<JarateDebuff>(), TF2.Time(5));

        public override void AddRecipes() => CreateRecipe().AddIngredient<Jarate>(8).AddIngredient<ReclaimedMetal>().AddTile<CraftingAnvil>().Register();
    }
}