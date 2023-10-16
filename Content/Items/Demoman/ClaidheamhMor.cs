using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Pyro;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Demoman
{
    public class ClaidheamhMor : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Melee, Unique, Craft);
            SetSwingUseStyle(sword: true);
            SetWeaponDamage(damage: 65, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/demo_sword_swing1");
            SetWeaponAttackIntervals(deploy: 0.875, holster: 0.875);
            SetWeaponPrice(weapon: 2, scrap: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddSwordAttribute(description);
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override bool WeaponResetHolsterTimeCondition(Player player) => player.controlUseItem;

        protected override void WeaponPassiveUpdate(Player player)
        {
            player.GetModPlayer<ClaidheamhMorPlayer>().claidheamhMorInInventory = true;
            ShieldPlayer shield = player.GetModPlayer<TF2Player>().shieldType switch
            {
                1 => player.GetModPlayer<CharginTargePlayer>(),
                _ => player.GetModPlayer<CharginTargePlayer>(),
            };

            if (player.GetModPlayer<ClaidheamhMorPlayer>().claidheamhMorInInventory && timer[0] == 1 && !shield.chargeActive)
            {
                shield.timer += (int)(shield.shieldRechargeTime * 0.25f);
                timer[0] = 0;
            }
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.crit || p.critMelee)
            {
                timer[0] = 1;
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
            if (p.crit || p.critMelee)
            {
                timer[0] = 1;
                if (p.critMelee)
                    p.crit = true;
                player.ClearBuff(ModContent.BuffType<MeleeCrit>());
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Homewrecker>()
                .AddIngredient<CharginTarge>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class ClaidheamhMorPlayer : ModPlayer
    {
        public bool claidheamhMorInInventory;

        public override void ResetEffects() => claidheamhMorInInventory = false;

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (claidheamhMorInInventory)
                modifiers.FinalDamage *= 1.15f;
        }
    }
}