using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Ammo;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Sniper;

namespace TF2.Content.Items.Sniper
{
    public class Huntsman : SniperRifle
    {
        public int fatigue;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Sniper's Unlocked Primary");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 11;
            Item.height = 50;
            Item.useTime = 116;
            Item.useAnimation = 116;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.channel = true;

            Item.damage = 50;
            chargeUpDamage = Item.damage;
            Item.shoot = ModContent.ProjectileType<Arrow>();
            Item.shootSpeed = 12.5f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.GetGlobalItem<TF2ItemBase>().noRandomCrits = true;

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
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
            if (ModContent.GetInstance<TF2ConfigClient>().Channel)
                Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/bow_shoot");
            else
                Item.UseSound = null;
            if (!ModContent.GetInstance<TF2ConfigClient>().Channel)
                maxChargeUp = 60f;
            else
                maxChargeUp = 60f + Item.useTime;
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.sniperMaxCharge = maxChargeUp;
            p.sniperCharge = chargeTime;
            p.sniperChargeTimer = sniperChargeInterval;
            chargeTime = (int)Utils.Clamp(chargeTime, 0, maxChargeUp);
            if (ModContent.GetInstance<TF2ConfigClient>().Channel)
                Charge();
            if (chargeTime == maxChargeUp)
            {
                p.crit = true;
                fatigue++;
            }

            if (Main.mouseLeftRelease && isCharging && !ModContent.GetInstance<TF2ConfigClient>().Channel && !player.dead)
            {
                Vector2 shootDirection = player.DirectionTo(Main.MouseWorld);
                if (fatigue >= 300)
                    shootDirection = shootDirection.RotatedByRandom(MathHelper.ToRadians(60f));
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, shootDirection * Item.shootSpeed, ModContent.ProjectileType<Arrow>(), chargeUpDamage, 0f, player.whoAmI);
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
                fatigue = 0;
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/bow_shoot"), player.Center);
                isCharging = false;
                ChargeWeaponConsumeAmmo(player);
            }

            if (player.dead)
                isCharging = false;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.ModItem is SniperRifle && isCharging)
            {
                player.moveSpeed -= 0.55f;
                player.GetModPlayer<TF2Player>().speedMultiplier -= 0.55f;
                player.GetModPlayer<TF2Player>().disableFocusSlowdown = true;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!ModContent.GetInstance<TF2ConfigClient>().Channel)
                return false;
            Vector2 newVelocity = velocity;
            if (fatigue >= 240)
                newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(60f));
            int proj = Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Arrow>(), chargeUpDamage, knockback, player.whoAmI);
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
            fatigue = 0;
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (!ModContent.GetInstance<TF2ConfigClient>().Channel && player.controlUseItem)
                {
                    if (!isCharging)
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/bow_shoot_pull"), player.Center);
                    isCharging = true;
                    player.itemTime = 1;
                    Charge();
                    player.itemAnimation = Item.useTime;
                    if (Main.mouseRight && fatigue > 0)
                    {
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/bow_shoot_pull_reverse"), player.Center);
                        fatigue = 0;
                        chargeTime = 0f;
                        isCharging = false;
                        player.itemTime = 116;
                    }
                    return false;
                }
                else
                    return true;
            }
            return null;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => ModContent.GetInstance<TF2ConfigClient>().Channel;

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            chargeUpDamage = (int)(Item.damage * p.classMultiplier) + (int)(70 * p.classMultiplier * (chargeTime / maxChargeUp));
        }

        public override bool AltFunctionUse(Player player)
        {
            if (ModContent.GetInstance<TF2ConfigClient>().Channel && fatigue > 0)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/bow_shoot_pull_reverse"), player.Center);
                fatigue = 0;
                chargeTime = 0f;
                isCharging = false;
                player.itemTime = 116;
                return true;
            }
            else
                return false;
        }
    }
}