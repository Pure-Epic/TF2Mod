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
using TF2.Content.Items;

namespace TF2.Gensokyo.Content.Items.Sniper
{
    [ExtendsFromMod("Gensokyo")]
    public class HarshPunisher : TF2Weapon
    {
        public float chargeTime = 0f;
        public int chargeUpDamage;
        public float maxChargeUp = 60f;
        public float sniperChargeInterval = 1f;
        public float chargeUpRate = 1f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harsh Punisher");
            Tooltip.SetDefault("Sniper's Exclusive Primary");

            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 59;
            Item.height = 21;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/sniper_shoot");
            Item.autoReuse = true;
            Item.channel = true;

            Item.damage = 100;
            chargeUpDamage = Item.damage;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.GetGlobalItem<TF2ItemBase>().noRandomCrits = true;

            ammoCost = 1;
            maxAmmoClip = 7;
            ammoInClip = 7;
            reloadRate = 120f;
            magazine = true;
            reloadSound = new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/harshpunisher_reload");

            Item.value = Item.buyPrice(platinum: 10);
            Item.rare = ModContent.RarityType<UnusualRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "+100% damage bonus\n"
                + "+50% faster firing speed\n"
                + "+100% charge rate\n"
                + "No unscoping")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "On Full Charge: -20% damage per shot\n"
                + "Reloading required")
            {
                OverrideColor = new Color(255, 64, 64)
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

        public override void HoldItem(Player player)
        {
            if (!reload)
                player.scope = true;
            TF2Player p = player.GetModPlayer<TF2Player>();
            chargeTime = (int)Utils.Clamp(chargeTime, 0, maxChargeUp);
            p.sniperMaxCharge = maxChargeUp;
            p.sniperCharge = chargeTime;
            p.sniperChargeTimer = sniperChargeInterval;
            if (Main.mouseRight && !ModContent.GetInstance<TF2ConfigClient>().SniperAutoCharge)
                Charge();
            if (ModContent.GetInstance<TF2ConfigClient>().SniperAutoCharge)
                Charge();
            if (Main.mouseRightRelease && !ModContent.GetInstance<TF2ConfigClient>().SniperAutoCharge)
                chargeTime = 0f;
            if (chargeTime == maxChargeUp)
                p.crit = true;

            WeaponSystem clip = player.GetModPlayer<WeaponSystem>();
            clip.ammoMax = maxAmmoClip;
            clip.ammoReloadRate = reloadRate;
            clip.ammoCurrent = ammoInClip;
            if (clip.startReload)
            {
                reload = true;
                clip.startReload = false;
            }
            UpdateResource();
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.ModItem is HarshPunisher && isCharging)
            {
                player.moveSpeed -= 0.73f;
                player.GetModPlayer<TF2Player>().speedMultiplier -= 0.73f;
                player.GetModPlayer<TF2Player>().disableFocusSlowdown = true;
                isCharging = false;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (ammoInClip == 0 || reload) return false;
            ammoInClip -= ammoCost;

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

            if (ammoInClip <= 0)
                reload = true;
            return false;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            chargeUpDamage = (int)(Item.damage * p.classMultiplier) + (int)(20 * p.classMultiplier * (chargeTime / maxChargeUp));
        }

        public override bool AltFunctionUse(Player player) => false;

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool CanUseItem(Player player) => !(ammoInClip <= 0 || reload);

        public void Charge()
        {
            if (!reload)
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
            else
                chargeTime = 0f;
        }
    }
}