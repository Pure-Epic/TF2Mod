using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Weapons.Spy;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Sniper
{
    public class TribalmansShiv : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Sniper, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 33);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponPrice(weapon: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            BleedingNPC npc = target.GetGlobalNPC<BleedingNPC>();
            npc.damageMultiplier = p.damageMultiplier;
            target.AddBuff(ModContent.BuffType<Bleeding>(), 360);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            BleedingPlayer bleedPlayer = target.GetModPlayer<BleedingPlayer>();
            bleedPlayer.damageMultiplier = p.damageMultiplier;
            target.AddBuff(ModContent.BuffType<Bleeding>(), TF2.Time(6));
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<DeadRinger>().AddIngredient<Huntsman>().AddTile<CraftingAnvil>().Register();
    }
}