using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Ammo;
using TF2.Content.Projectiles.Pyro;

namespace TF2.Content.Items.Pyro
{
    public class FlameThrower : TF2WeaponNoAmmo
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame Thrower");
            Tooltip.SetDefault("Pyro's Starter Primary");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 14;
            Item.useTime = 6;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;

            Item.damage = 78;
            Item.shoot = ModContent.ProjectileType<Fire>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.mana = 20;

            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "Extinguishing teammates restores 11% of max health")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Neutral Attributes",
                "Alt-Fire: Release a blast of air that pushes enemies\n"
                + "and extinguish teammates that have debuffs.")
            {
                OverrideColor = new Color(255, 255, 255)
            };
            tooltips.Add(line2);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.classAccessory && !p.classHideVanity)
                Item.noUseGraphic = true;
            else
                Item.noUseGraphic = false;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 45;
                Item.useAnimation = 45;
                Item.damage = 1;
                Item.shoot = ModContent.ProjectileType<Airblast>();
                Item.shootSpeed = 25f;
            }
            else
            {
                Item.useTime = 6;
                Item.useAnimation = 30;
                Item.damage = 78;
                Item.shoot = ModContent.ProjectileType<Fire>();
                Item.shootSpeed = 10f;
            }
            return base.CanUseItem(player);
        }
        public override bool AltFunctionUse(Player player) => true;

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse != 2)
                reduce -= 20;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (player.altFunctionUse != 2)
                return player.itemAnimation >= player.itemAnimationMax - 5;
            else
                return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                Vector2 muzzleOffset = Vector2.Normalize(velocity) * 54f;
                if (Collision.CanHit(position, 6, 6, position + muzzleOffset, 6, 6))
                {
                    position += muzzleOffset;
                    SoundEngine.PlaySound(SoundID.Item34, player.Center);
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Fire>(), damage, knockback, player.whoAmI);

                }
                return false;
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/flame_thrower_airblast"), player.Center);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Airblast>(), damage, knockback, player.whoAmI);
                return false;
            }
        }
    }
}