using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.Scout;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.MultiClass
{
    public class PainTrain : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(MultiClass, Melee, Unique, Craft);
            SetWeaponClass([Soldier, Demoman]);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponPrice(weapon: 1, scrap: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<PainTrainPlayer>().painTrainEquipped = true;

        protected override void WeaponHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers) => modifiers.SourceDamage *= target.boss ? 1.5f : 1f;

        public override void AddRecipes() => CreateRecipe().AddIngredient<Sandman>().AddIngredient<ScrapMetal>().AddTile<AustraliumAnvil>().Register();
    }

    public class PainTrainPlayer : ModPlayer
    {
        public bool painTrainEquipped;

        public override void ResetEffects() => painTrainEquipped = false;

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (painTrainEquipped)
                modifiers.FinalDamage *= 1.1f;
        }
    }
}