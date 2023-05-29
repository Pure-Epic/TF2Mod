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
using TF2.Content.Projectiles;

namespace TF2.Content.Items.Sniper
{
    public class SniperRifle : TF2WeaponNoAmmo
    {
        public float chargeTime = 0f;
        public int chargeUpDamage;
        public float maxChargeUp = 1f;
        public float sniperChargeInterval = 1f;
        public float chargeUpRate = 1f;
        public float chargeUpDelay = 78f;
        public float chargeUpDelayTimer = 78f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sniper Rifle");
            Tooltip.SetDefault("Sniper's Starter Primary");

            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 21;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/sniper_shoot");
            Item.autoReuse = true;
            Item.channel = true;

            Item.damage = 50;
            chargeUpDamage = Item.damage;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.GetGlobalItem<TF2ItemBase>().noRandomCrits = true;

            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.classAccessory && !p.classHideVanity)
                Item.noUseGraphic = true;
            else
                Item.noUseGraphic = false;
        }

        public override void HoldItem(Player player)
        {
            if (chargeUpDelayTimer >= chargeUpDelay)
                player.scope = true;
            maxChargeUp = 120f;
            TF2Player p = player.GetModPlayer<TF2Player>();
            chargeTime = (int)Utils.Clamp(chargeTime, 0, maxChargeUp);
            p.sniperMaxCharge = maxChargeUp;
            p.sniperCharge = chargeTime;
            p.sniperChargeTimer = sniperChargeInterval;
            chargeUpDelayTimer++;
            if (Main.mouseRight && !ModContent.GetInstance<TF2ConfigClient>().SniperAutoCharge)
                Charge();
            if (ModContent.GetInstance<TF2ConfigClient>().SniperAutoCharge)
                Charge();
            if (Main.mouseRightRelease && !ModContent.GetInstance<TF2ConfigClient>().SniperAutoCharge)
                chargeTime = 0f;
            if (chargeTime == maxChargeUp)
                p.crit = true;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.ModItem is SniperRifle && isCharging)
            {
                player.moveSpeed -= 0.73f;
                player.GetModPlayer<TF2Player>().speedMultiplier -= 0.73f;
                player.GetModPlayer<TF2Player>().disableFocusSlowdown = true;
                isCharging = false;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Bullet>(), chargeUpDamage, knockback, player.whoAmI);
            if (player.GetModPlayer<TF2Player>().focus)
            {
                Main.projectile[proj].GetGlobalProjectile<TF2ProjectileBase>().homing = true;
                Main.projectile[proj].GetGlobalProjectile<TF2ProjectileBase>().shootSpeed = Item.shootSpeed;
                NetMessage.SendData(MessageID.SyncProjectile, number: proj);
            }
            if (chargeTime == maxChargeUp)
            {
                Main.projectile[proj].GetGlobalProjectile<TF2ProjectileBase>().sniperCrit = true;
                NetMessage.SendData(MessageID.SyncProjectile, number: proj);
            }
            chargeUpDamage = Item.damage;
            chargeTime = 0f;
            chargeUpDelayTimer = 0;
            return false;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            chargeUpDamage = (int)(Item.damage * p.classMultiplier) + (int)(100 * p.classMultiplier * (chargeTime / maxChargeUp));
        }

        public override bool AltFunctionUse(Player player) => false;

        public void Charge()
        {
            if (!(chargeUpDelayTimer < chargeUpDelay))
            {
                isCharging = true;
                sniperChargeInterval++;
                if (sniperChargeInterval >= chargeUpRate)
                {
                    chargeTime += 1f;
                    sniperChargeInterval = 0f;
                }
                chargeTime = Utils.Clamp(chargeTime, 0, maxChargeUp);
            }
        }
    }
}