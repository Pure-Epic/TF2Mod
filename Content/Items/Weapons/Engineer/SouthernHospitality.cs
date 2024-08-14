using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.Spy;
using TF2.Content.Projectiles.Engineer;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Engineer
{
    public class SouthernHospitality : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Engineer, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/wrench_swing");
            SetWeaponPrice(weapon: 1, scrap: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<SouthernHospitalityPlayer>().southernHospitalityEquipped = true;

        protected override bool? WeaponOnUse(Player player)
        {
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<WrenchHitbox>(), 1, 0f);
            return true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            BleedingNPC npc = target.GetGlobalNPC<BleedingNPC>();
            npc.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<Bleeding>(), TF2.Time(5));
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            BleedingPlayer bleedPlayer = target.GetModPlayer<BleedingPlayer>();
            bleedPlayer.damageMultiplier = p.classMultiplier;
            target.AddBuff(ModContent.BuffType<Bleeding>(), TF2.Time(5));
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<Ambassador>().AddIngredient<ScrapMetal>().AddTile<CraftingAnvil>().Register();
    }

    public class SouthernHospitalityPlayer : ModPlayer
    {
        public bool southernHospitalityEquipped;

        public override void ResetEffects() => southernHospitalityEquipped = false;

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (southernHospitalityEquipped)
                modifiers.FinalDamage *= 1.2f;
        }
    }
}