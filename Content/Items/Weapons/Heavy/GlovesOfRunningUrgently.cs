using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Heavy;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Heavy
{
    public class GlovesOfRunningUrgently : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Heavy, Melee, Unique, Craft);
            SetLungeUseStyle();
            SetWeaponDamage(damage: 65, projectile: ModContent.ProjectileType<GlovesOfRunningUrgentlyProjectile>(), projectileSpeed: 2f);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponAttackIntervals(holster: 0.75, altClick: true);
            SetWeaponPrice(weapon: 1, scrap: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponAttackAnimation(Player player) => Item.noUseGraphic = true;

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
        public bool giveBackInitialHealth;

        public override void PostUpdate()
        {
            if (Player.HeldItem.ModItem is GlovesOfRunningUrgently && Player.inventory[58].ModItem is not GlovesOfRunningUrgently)
            {
                TF2Player.SetPlayerHealth(Player, -20);
                TF2Player.SetPlayerSpeed(Player, 133);
            }
        }

        public override void UpdateBadLifeRegen()
        {
            if (Player.HeldItem.ModItem is GlovesOfRunningUrgently && Player.inventory[58].ModItem is not GlovesOfRunningUrgently && Player.statLife > TF2.GetHealth(Player, 100))
            {
                if (Player.statLife < TF2.GetHealth(Player, 280 + Player.GetModPlayer<TF2Player>().healthBonus))
                    giveBackInitialHealth = true;
                timer++;
                if (timer >= 12)
                {
                    int decayAmount = TF2.GetHealth(Player, 1);
                    if (Player.statLife >= TF2.GetHealth(Player, 100) + decayAmount)
                    {
                        Player.statLife -= decayAmount;
                        returnedHealth += decayAmount;
                    }
                    else
                    {
                        Player.statLife = TF2.GetHealth(Player, 100);
                        returnedHealth += TF2Player.TotalHealth(Player) % 3;
                    }
                    CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), CombatText.LifeRegen, decayAmount, dramatic: false, dot: true);
                    timer = 0;
                }
            }
            else if (Player.HeldItem.ModItem is not GlovesOfRunningUrgently && !TF2Player.IsHealthFull(Player))
            {
                if (giveBackInitialHealth)
                {
                    returnedHealth += TF2.GetHealth(Player, 20);
                    giveBackInitialHealth = false;
                }
                if (returnedHealth > 0)
                {
                    timer++;
                    if (timer >= 12)
                    {
                        int healAmount = TF2.GetHealth(Player, 1);
                        if (healAmount < TF2.GetHealth(Player, 1))
                            healAmount = returnedHealth;
                        Player.Heal(healAmount);
                        returnedHealth -= healAmount;
                        if (returnedHealth < 0)
                            returnedHealth = 0;
                        timer = 0;
                    }
                }
            }
            if (TF2Player.IsHealthFull(Player))
                returnedHealth = 0;
        }
    }
}