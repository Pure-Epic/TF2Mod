using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.Projectiles.Scout;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Scout
{
    public class BostonBasher : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Melee, Unique, Craft);
            SetSwingUseStyle(noMelee: true);
            SetWeaponDamage(damage: 35);
            SetWeaponAttackSpeed(0.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponPrice(weapon: 3);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            BostonBasherPlayer p = player.GetModPlayer<BostonBasherPlayer>();
            if (!p.resetHit && p.miss && player.ItemAnimationEndingOrEnded)
            {
                if (!player.immuneNoBlink)
                {
                    player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " " + TF2.TF2DeathMessagesLocalization[1]), TF2.GetHealth(player, 18), 0, cooldownCounter: 5);
                    player.GetModPlayer<BleedingPlayer>().damageMultiplier = TF2.GetHealth(player, 1);
                    player.AddBuff(ModContent.BuffType<Bleeding>(), TF2.Time(5));
                }
                p.resetHit = true;
            }
        }

        protected override bool? WeaponOnUse(Player player)
        {
            BostonBasherPlayer p = player.GetModPlayer<BostonBasherPlayer>();
            p.miss = true;
            p.resetHit = false;
            TF2.CreateProjectile(this, player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<BostonBasherHitbox>(), TF2.Round(35 * player.GetModPlayer<TF2Player>().classMultiplier), 0f);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Sandman>()
                .AddIngredient<TribalmansShiv>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class BostonBasherPlayer : ModPlayer
    {
        public bool miss;
        public bool resetHit;
    }
}