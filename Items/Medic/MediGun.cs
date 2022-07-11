using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Collections.Generic;
using System.Linq;
using TF2.Projectiles.Medic;

namespace TF2.Items.Medic
{
    public class MediGun : TF2WeaponNoAmmo
    {
        public int uberchargeCapacity = 1000;
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Medic's Starter Secondary");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) // needs System.Linq
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            tooltips.Remove(tt);
        }
        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1f;
            Item.useTime = 0;
            Item.useAnimation = 0;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item15;
            Item.autoReuse = true;
            Item.channel = true;

            Item.damage = 1;
            Item.shoot = ModContent.ProjectileType<HealingBeam>();
            Item.shootSpeed = 1f;
            Item.knockBack = -1;
            Item.crit = 0;

            uberchargeCapacity = 1000;
            Item.rare = ItemRarityID.White;
        }
        public override void HoldItem(Player player)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            p.maxUbercharge = uberchargeCapacity;
            //UpdateResource();
        }
        public override bool AltFunctionUse(Player player)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            if (p.ubercharge > 999)
            {
                p.activateUbercharge = true;
            }
            return false;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            if (p.classAccessory == true && !p.classHideVanity)
            {
                Item.scale = 0f;
            }
            else
            {
                Item.scale = 1f;
            }
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

    }
}