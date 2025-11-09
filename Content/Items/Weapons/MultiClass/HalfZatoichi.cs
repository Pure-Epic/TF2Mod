using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.Demoman;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.MultiClass
{
    public class HalfZatoichi : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(MultiClass, Melee, Unique, Craft);
            SetWeaponClass([Soldier, Demoman]);
            SetSwingUseStyle(sword: true);
            SetWeaponDamage(damage: 65, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/katana_swing");
            SetWeaponAttackIntervals(deploy: 1, holster: 1);
            SetWeaponPrice(weapon: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddSwordAttribute(description);
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override void WeaponAttackAnimation(Player player)
        {
            float start = player.direction == 1 ? TF2.SwordRotation(player, -135f) : TF2.SwordRotation(player, -45f);
            float end = player.direction == 1 ? TF2.SwordRotation(player, -300f) : TF2.SwordRotation(player, 120f);
            float currentAngle;
            if (player.itemAnimation < player.itemAnimationMax * 0.5)
            {
                float duration = 1f - (float)player.itemAnimation / player.itemAnimationMax;
                currentAngle = MathHelper.Lerp(start, end, duration * 2f);
            }
            else
            {
                float duration = 1f - (float)(player.itemAnimation - player.itemAnimationMax / 2) / player.itemAnimationMax;
                currentAngle = MathHelper.Lerp(end, start, duration * 2f);
            }
            player.itemRotation = player.direction > 0 ? (currentAngle + (player.gravDir >= 0 ? MathHelper.PiOver4 : (MathHelper.PiOver4 * 7))) : currentAngle + (player.gravDir >= 0 ? (MathHelper.PiOver4 * 3) : (MathHelper.PiOver4 * 5));
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.gravDir >= 0 ? (currentAngle - MathHelper.PiOver2) : (-currentAngle - MathHelper.PiOver2));
            player.itemLocation = player.MountedCenter + Vector2.UnitX.RotatedBy(currentAngle);
        }

        protected override void WeaponHitPlayer(Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            TF2Player oppponent = target.GetModPlayer<TF2Player>();
            if (oppponent.currentClass == Soldier || oppponent.currentClass == Demoman)
                modifiers.SourceDamage *= 3f;
            if (p.miniCrit || p.crit || p.critMelee)
                player.Heal(TF2Player.GetPlayerHealthFromPercentage(player, 50));
        }

        protected override void WeaponHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.miniCrit || p.crit || p.critMelee)
                player.Heal(TF2Player.GetPlayerHealthFromPercentage(player, 50));
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<Eyelander>(2).AddIngredient<ReclaimedMetal>().AddTile<AustraliumAnvil>().Register();
    }

    public class HalfZatoichiPlayer : ModPlayer
    {
        public bool halfZatoichiCheck;

        public override void PostUpdate()
        {
            if (Player.HeldItem.ModItem is not HalfZatoichi)
            {
                if (!TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<HalfZatoichi>())) return;
                int healthPenalty = TF2.GetHealth(Player, 50);
                if (Player.statLife > healthPenalty && halfZatoichiCheck)
                {
                    Player.Hurt(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[4].ToNetworkText(Player.name)), healthPenalty, 0);
                    halfZatoichiCheck = false;
                }
            }
            else
            {
                if (Player.HeldItem == Player.inventory[58]) return;
                halfZatoichiCheck = true;
                (Player.HeldItem.ModItem as HalfZatoichi).lockWeapon = Player.statLife <= TF2.GetHealth(Player, 50) && halfZatoichiCheck;
            }
        }

        public override void UpdateDead() => halfZatoichiCheck = false;
    }
}