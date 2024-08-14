using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Pyro
{
    public class SharpenedVolcanoFragment : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 52);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponPrice(weapon: 1, reclaimed: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            PyroFlamesPlayer burntPlayer = target.GetModPlayer<PyroFlamesPlayer>();
            burntPlayer.damageMultiplier = p.classMultiplier;
            target.ClearBuff(ModContent.BuffType<PyroFlamesDegreaser>());
            target.AddBuff(ModContent.BuffType<PyroFlames>(), TF2.Time(7.5), false);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            PyroFlamesNPC npc = target.GetGlobalNPC<PyroFlamesNPC>();
            npc.damageMultiplier = p.classMultiplier;
            TF2.ExtinguishPyroFlames(target, ModContent.BuffType<PyroFlamesDegreaser>());
            target.AddBuff(ModContent.BuffType<PyroFlames>(), TF2.Time(7.5));
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<Axtinguisher>().AddIngredient<ReclaimedMetal>(2).AddTile<CraftingAnvil>().Register();
    }
}