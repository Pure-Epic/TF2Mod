using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Pyro
{
    public class Powerjack : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/melee_swing");
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddHeader(description);
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponActiveBonus(Player player) => SetPlayerSpeed(player, 115);

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit && target.type != NPCID.TargetDummy && player.statLife < player.statLifeMax2)
                player.Heal((int)(player.statLifeMax2 * 0.14285714285f));
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            if (player.GetModPlayer<TF2Player>().crit && player.statLife < player.statLifeMax2)
                player.Heal((int)(player.statLifeMax2 * 0.14285714285f));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Axtinguisher>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class PowerjackPlayer : ModPlayer
    {
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (Player.HeldItem.ModItem is Powerjack)
                modifiers.FinalDamage *= 1.2f;
        }
    }
}