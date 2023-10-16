using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Heavy;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Heavy
{
    public class GlovesOfRunningUrgently : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Melee, Unique, Craft);
            SetLungeUseStyle();
            SetWeaponDamage(damage: 65, projectile: ModContent.ProjectileType<GlovesOfRunningUrgentlyProjectile>(), projectileSpeed: 2f);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/melee_swing");
            SetWeaponAttackIntervals(holster: 0.75, altClick: true);
            SetWeaponPrice(weapon: 1, scrap: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponAttackAnimation(Player player) => Item.noUseGraphic = true;

        protected override bool WeaponResetHolsterTimeCondition(Player player) => player.controlUseItem || player.controlUseTile;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<KillingGlovesOfBoxing>()
                .AddIngredient<ScrapMetal>(2)
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class GlovesOfRunningUrgentlyPlayer : ModPlayer
    {
        public int timer;
        public int returnedHealth;

        public override void PostUpdate()
        {
            if (Player.HeldItem.ModItem is GlovesOfRunningUrgently)
            {
                Player.statLifeMax2 = (int)(Player.statLifeMax2 * 0.9333f);
                TF2Weapon.SetPlayerSpeed(Player, 133);
            }
        }

        public override void UpdateBadLifeRegen()
        {
            if (Player.HeldItem.ModItem is GlovesOfRunningUrgently && Player.statLife > Player.statLifeMax2 / 3)
            {
                timer++;
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                Player.lifeRegenTime = 0;
                if (timer >= 12)
                {
                    int decayAmount = (int)MathF.Ceiling(Player.statLifeMax2 / 300f);
                    if (Player.statLife >= Player.statLifeMax2 / 3 + decayAmount)
                    {
                        Player.statLife -= decayAmount;
                        returnedHealth += decayAmount;
                    }
                    else
                    {
                        Player.statLife = Player.statLifeMax2 / 3;
                        returnedHealth += Player.statLifeMax2 % 3;
                    }
                    CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), CombatText.LifeRegen, decayAmount, dramatic: false, dot: true);
                    timer = 0;
                }
            }
            else if (Player.HeldItem.ModItem is not GlovesOfRunningUrgently && Player.statLife < Player.statLifeMax2)
            {
                if (returnedHealth > 0)
                {
                    timer++;
                    if (Player.lifeRegen > 0)
                        Player.lifeRegen = 0;
                    Player.lifeRegenTime = 0;
                    if (timer >= 12)
                    {
                        int healAmount = (int)MathF.Ceiling(Player.statLifeMax2 / 300f);
                        if (healAmount < (int)MathF.Ceiling(Player.statLifeMax2 / 300f))
                            healAmount = returnedHealth;
                        Player.Heal(healAmount);
                        returnedHealth -= healAmount;
                        if (returnedHealth < 0)
                            returnedHealth = 0;
                        timer = 0;
                    }
                }
            }
        }
    }
}