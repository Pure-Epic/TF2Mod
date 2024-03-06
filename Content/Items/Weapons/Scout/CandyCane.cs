using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Weapons.Medic;
using TF2.Content.Items.Weapons.MultiClass;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Scout
{
    public class CandyCane : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 35);
            SetWeaponAttackSpeed(0.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponPrice(weapon: 2, scrap: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<CandyCanePlayer>().candyCaneEquipped = true;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Kritzkrieg>()
                .AddIngredient<PainTrain>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class CandyCanePlayer : ModPlayer
    {
        public bool candyCaneEquipped;

        public override void ResetEffects() => candyCaneEquipped = false;

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (candyCaneEquipped)
                modifiers.FinalDamage *= 1.25f;
        }
    }
}