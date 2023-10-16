using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Demoman;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.MultiClass
{
    public class HalfZatoichi : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(MultiClass, Melee, Unique, Craft);
            SetWeaponClass(new int[] { Soldier, Demoman });
            SetSwingUseStyle(sword: true);
            SetWeaponDamage(damage: 65, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/demo_sword_swing1");
            SetWeaponAttackIntervals(deploy: 0.875, holster: 0.875);
            SetWeaponPrice(weapon: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddSwordAttribute(description);
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override bool WeaponResetHolsterTimeCondition(Player player) => player.controlUseItem;

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.miniCrit || p.crit || p.critMelee)
            {
                player.Heal(player.statLifeMax2 / 2);
                if (p.critMelee)
                    p.crit = true;
                player.ClearBuff(ModContent.BuffType<MeleeCrit>());
            }
            else
                modifiers.DisableCrit();
        }

        public override void ModifyHitPvp(Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            TF2Player oppponent = target.GetModPlayer<TF2Player>();
            if (oppponent.currentClass == 2 || oppponent.currentClass == 4)
                modifiers.SourceDamage *= 3f;
            if (p.miniCrit || p.crit || p.critMelee)
            {
                player.statLife += player.statLifeMax2 / 2;
                if (p.critMelee)
                    p.crit = true;
                player.ClearBuff(ModContent.BuffType<MeleeCrit>());
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Eyelander>(2)
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class HalfZatoichiPlayer : ModPlayer
    {
        public bool halfZatoichiCheck;

        public override void PostUpdate()
        {
            if (Player.HeldItem.ModItem is not HalfZatoichi)
            {
                if (!TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<HalfZatoichi>())) return;
                if (Player.statLife > (int)(Player.statLifeMax2 * 0.25f) && halfZatoichiCheck)
                {
                    Player.Hurt(PlayerDeathReason.ByCustomReason(Player.name + " was a coward."), (int)(Player.statLifeMax2 * 0.25f), 0);
                    halfZatoichiCheck = false;
                }
            }
            else
            {
                if (Player.HeldItem == Player.inventory[58]) return;
                    halfZatoichiCheck = true;
                HalfZatoichi weapon = Player.HeldItem.ModItem as HalfZatoichi;
                weapon.lockWeapon = Player.statLife <= (int)(Player.statLifeMax2 * 0.25f) && halfZatoichiCheck;
            }
        }

        public override void UpdateDead() => halfZatoichiCheck = false;
    }
}