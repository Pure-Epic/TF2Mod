using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Scout
{
    public class FanOWar : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 9);
            SetWeaponAttackSpeed(0.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponPrice(weapon: 1, reclaimed: 1, scrap: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override void WeaponActiveBonus(Player player)
        {
            if (player.GetModPlayer<TF2Player>().miniCrit)
                player.GetModPlayer<TF2Player>().crit = true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<MarkedForDeath>(), TF2.Time(15));

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) => target.AddBuff(ModContent.BuffType<MarkedForDeath>(), TF2.Time(15));

        public override void AddRecipes() => CreateRecipe().AddIngredient<MadMilk>().AddIngredient<ScrapMetal>().AddTile<CraftingAnvil>().Register();
    }
}