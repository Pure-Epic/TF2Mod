using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Heavy;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Heavy
{
    public class FistsOfSteel : TF2Weapon
    {
        protected override string ArmTexture => "TF2/Content/Textures/Items/Heavy/FistsOfSteel";

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Melee, Unique, Craft);
            SetLungeUseStyle();
            SetWeaponDamage(damage: 65, projectile: ModContent.ProjectileType<FistsOfSteelProjectile>(), projectileSpeed: 2f);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponAttackIntervals(holster: 1, altClick: true);
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponAttackAnimation(Player player) => Item.noUseGraphic = true;

        protected override bool WeaponAddTextureCondition(Player player) => HoldingWeapon<FistsOfSteel>(player);

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<FistsOfSteelPlayer>().fistsOfSteelEquipped = true;

        public override void AddRecipes() => CreateRecipe().AddIngredient<KillingGlovesOfBoxing>().AddIngredient<ReclaimedMetal>().AddTile<AustraliumAnvil>().Register();
    }

    public class FistsOfSteelPlayer : ModPlayer
    {
        public bool fistsOfSteelEquipped;

        public override void ResetEffects() => fistsOfSteelEquipped = false;

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (TF2Weapon.HoldingWeapon<FistsOfSteel>(Player))
                modifiers.FinalDamage *= 2f;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (TF2Weapon.HoldingWeapon<FistsOfSteel>(Player))
                modifiers.FinalDamage *= 0.6f;
        }
    }
}