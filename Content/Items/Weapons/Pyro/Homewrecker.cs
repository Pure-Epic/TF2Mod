using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.Soldier;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Pyro
{
    public class Homewrecker : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Melee, Unique, Craft);
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

        protected override void WeaponAttackAnimation(Player player)
        {
            foreach (Projectile projectile in Main.projectile)
            {
                if (TF2.MeleeHitbox(player).Intersects(projectile.Hitbox) && TF2.CanParryProjectile(projectile) && projectile.hostile && !projectile.friendly)
                {
                    projectile.velocity *= -1f;
                    projectile.hostile = false;
                }
            }
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            // Temporary solution
            ModLoader.TryGetMod("Gensokyo", out Mod gensokyo);
            if (gensokyo == null && target.boss) return;
            modifiers.SourceDamage *= target.ModNPC?.Mod == gensokyo && target.boss || target.TypeName == "Byakuren Hijiri" ? 2f : 0.75f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Equalizer>()
                .AddIngredient<ScrapMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}