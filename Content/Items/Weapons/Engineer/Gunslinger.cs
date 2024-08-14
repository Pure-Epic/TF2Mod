using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles.Engineer;

namespace TF2.Content.Items.Weapons.Engineer
{
    public class Gunslinger : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Engineer, Melee, Unique, Unlock);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/gunslinger_swing");
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            TF2Player.SetPlayerHealth(player, 25);
            player.GetModPlayer<GunslingerPlayer>().gunslingerEquipped = true;
        }

        protected override bool? WeaponOnUse(Player player)
        {
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<WrenchHitbox>(), 1, 0f);
            return true;
        }


        protected override void WeaponHitPlayer(Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            timer[0]++;
            if (timer[0] >= 3)
            {
                player.GetModPlayer<TF2Player>().crit = true;
                timer[0] = 0;
            }
        }

        protected override void WeaponHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            timer[0]++;
            if (timer[0] >= 3)
            {
                player.GetModPlayer<TF2Player>().crit = true;
                timer[0] = 0;
            }
        }
    }

    public class GunslingerPlayer : ModPlayer
    {
        public bool gunslingerEquipped;

        public override void ResetEffects() => gunslingerEquipped = false;
    }
}