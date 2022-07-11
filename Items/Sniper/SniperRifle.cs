using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Items.Ammo;
using Terraria.DataStructures;
using TF2.Projectiles;
using Terraria.Audio;

namespace TF2.Items.Sniper
{
    public class SniperRifle : TF2WeaponNoAmmo
    {
        public float chargeTime = 0;
        public int chargeUpDamage;
        public float maxChargeUp = 1f;
        public float sniperChargeInterval;
        public float chargeUpRate;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sniper Rifle");
            Tooltip.SetDefault("Sniper's Starter Primary");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1f;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/sniper_shoot");
            Item.autoReuse = true;
            Item.channel = true;

            Item.damage = 50;
            chargeUpDamage = Item.damage;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.knockBack = -1;
            Item.crit = 0;
            Item.rare = ItemRarityID.White;

            chargeTime = 0;
            sniperChargeInterval = 1f;
            chargeUpRate = 1f;
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

        public override void HoldItem(Player player)
        {
            player.scope = true;
            maxChargeUp = 200f;
            TFClass p = player.GetModPlayer<TFClass>();
            p.sniperMaxCharge = maxChargeUp;
            p.sniperCharge = chargeTime;
            p.sniperChargeTimer = sniperChargeInterval;
            Charge();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Bullet>(), chargeUpDamage, knockback, player.whoAmI);
            chargeUpDamage = Item.damage;
            chargeTime = 0;
            return false;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            if (chargeTime < maxChargeUp)
            {
                //player.GetDamage(DamageClass.Generic).Flat += (int)(100 * (chargeTime / maxChargeUp));
                chargeUpDamage = Item.damage + (int)(100 * p.classMultiplier * (chargeTime / maxChargeUp));
            }
            else
            {
                //player.GetDamage(DamageClass.Generic).Flat += 400;
                chargeUpDamage = (int)(Item.damage * 9 * p.classMultiplier);
            }
        }
        public override bool AltFunctionUse(Player player)
        {
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, 0);
        }

        public void Charge()
        {
            sniperChargeInterval++;
            if (sniperChargeInterval > chargeUpRate)
            {
                chargeTime += 1;
                sniperChargeInterval = 0;
            }
            chargeTime = Utils.Clamp(chargeTime, 0, maxChargeUp);
        }
    }
}