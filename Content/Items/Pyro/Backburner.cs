using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Pyro;

namespace TF2.Content.Items.Pyro
{
    public class Backburner : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Primary, Unique, Unlock);
            SetWeaponSize(50, 16);
            SetGunUseStyle();
            SetWeaponDamage(damage: 78, projectile: ModContent.ProjectileType<Fire>(), noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.1, 0.5, hide: true);
            SetWeaponAttackIntervals(altClick: true, noAmmo: true);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player)
        {
            if (player.controlUseTile && !(airblastCooldown >= 45 && player.statMana >= 20)) return false;
            Item.useTime = player.altFunctionUse != 2 ? 6 : 30;
            return base.WeaponCanBeUsed(player);
        }

        protected override bool WeaponCanConsumeAmmo(Player player) => player.altFunctionUse != 2 && player.itemAnimation >= player.itemAnimationMax - 5;

        protected override void WeaponActiveUpdate(Player player)
        {
            airblastCooldown++;
            if (airblastCooldown >= 45)
                airblastCooldown = 45;
        }

        protected override void WeaponFireProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                Vector2 muzzleOffset = Vector2.Normalize(velocity) * 54f;
                if (Collision.CanHit(position, 6, 6, position + muzzleOffset, 6, 6))
                {
                    position += muzzleOffset;
                    SoundEngine.PlaySound(SoundID.Item34, player.Center);
                    int i = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Fire>(), damage, knockback, player.whoAmI);
                    if (player.statLife == player.statLifeMax2)
                        Main.projectile[i].GetGlobalProjectile<TF2ProjectileBase>().crit = true;
                }
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/flame_thrower_airblast"), player.Center);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Airblast>(), 1, knockback, player.whoAmI);
                player.statMana -= 50;
                player.manaRegenDelay = 250;
            }
        }
    }
}