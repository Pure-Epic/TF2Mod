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
        protected override string ArmTexture => "TF2/Content/Textures/Items/Heavy/GlovesOfRunningUrgently";

        protected override int HealthBoost => -20;

        protected override bool TemporaryHealthBoost => true;

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

        protected override bool WeaponAddTextureCondition(Player player) => HoldingWeapon<GlovesOfRunningUrgently>(player);

        protected override bool WeaponModifyHealthCondition(Player player) => HoldingWeapon<GlovesOfRunningUrgently>(player);

        protected override void WeaponActiveBonus(Player player) => TF2Player.SetPlayerSpeed(player, 133);

        public override void AddRecipes() => CreateRecipe().AddIngredient<KillingGlovesOfBoxing>().AddIngredient<ScrapMetal>(2).AddTile<AustraliumAnvil>().Register();
    }

    public class GlovesOfRunningUrgentlyPlayer : ModPlayer
    {
        public int timer;
        public int returnedHealth;
        public bool giveBackInitialHealth;

        public override void UpdateBadLifeRegen()
        {
            if ((TF2Weapon.HoldingWeapon<GlovesOfRunningUrgently>(Player) || TF2Weapon.HoldingWeapon<EvictionNotice>(Player)) && Player.statLife > TF2.GetHealth(Player, 100))
            {
                if (Player.statLife < TF2.GetHealth(Player, Player.GetModPlayer<TF2Player>().cachedHealth))
                    giveBackInitialHealth = true;
                timer++;
                if (timer >= TF2.Time(0.2))
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
            else if (!TF2Player.IsHealthFull(Player))
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