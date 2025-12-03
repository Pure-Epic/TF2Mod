using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Weapons.Medic;
using TF2.Content.Items.Weapons.Soldier;
using TF2.Content.NPCs.Buddies;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Scout
{
    public class CritaCola : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Secondary, Unique, Craft);
            SetWeaponSize(19, 40);
            SetDrinkUseStyle();
            SetWeaponAttackSpeed(1.2, hide: true);
            SetWeaponAttackSound(SoundID.Item3);
            SetUtilityWeapon();
            SetTimers(TF2.Time(22));
            SetWeaponPrice(weapon: 2);
            noThe = true;
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddNeutralAttribute(description);

        public override bool WeaponCanBeUsed(Player player) => timer[0] >= TF2.Time(22);

        protected override void WeaponActiveUpdate(Player player)
        {
            if (isActive && player.ItemAnimationEndingOrEnded)
            {
                player.AddBuff(ModContent.BuffType<CritaColaBuff>(), TF2.Time(8));
                isActive = false;
            }
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            if (timer[0] < TF2.Time(22))
                timer[0]++;
        }

        protected override bool? WeaponOnUse(Player player)
        {
            timer[0] = 0;
            isActive = true;
            return true;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<Kritzkrieg>().AddIngredient<BonkAtomicPunch>().AddTile<AustraliumAnvil>().Register();
    }

    public class CritaColaPlayer : ModPlayer
    {
        public bool critaColaActive;

        public override void ResetEffects() => critaColaActive = false;

        public override void OnHurt(Player.HurtInfo info)
        {
            if (!info.PvP) return;
            Player opponent = Main.player[info.DamageSource.SourcePlayerIndex];
            if (opponent.GetModPlayer<CritaColaPlayer>().critaColaActive)
                opponent.AddBuff(ModContent.BuffType<MarkedForDeath>(), TF2.Time(5));

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type != NPCID.TargetDummy && critaColaActive)
                Player.AddBuff(ModContent.BuffType<MarkedForDeath>(), TF2.Time(5));
        }
    }
}