using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Scout
{
    public class SunonaStick : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 26);
            SetWeaponAttackSpeed(0.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponPrice(weapon: 3, reclaimed: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override void WeaponHitPlayer(Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            if (TF2.IsPlayerOnFire(target))
                player.GetModPlayer<TF2Player>().crit = true;
        }

        protected override void WeaponHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (TF2.IsNPCOnFire(target))
                player.GetModPlayer<TF2Player>().crit = true;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<BostonBasher>().AddIngredient<ReclaimedMetal>(2).AddTile<AustraliumAnvil>().Register();
    }

    public class SunonaStickPlayer : ModPlayer
    {
        public override void PostUpdateBuffs()
        {
            if (TF2Weapon.HoldingWeapon<SunonaStick>(Player))
            {
                for (int i = 0; i < BuffLoader.BuffCount; i++)
                {
                    if (Main.debuff[i] && !BuffID.Sets.NurseCannotRemoveDebuff[i] && !TF2BuffBase.cooldownBuff[i])
                        Player.buffImmune[i] = true;
                }
            }
        }
    }
}