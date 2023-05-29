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
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Sniper
{
    public class SydneySleeper : SniperRifle
    {
        public int jarateDuration = 120;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sydney Sleeper");
            Tooltip.SetDefault("Sniper's Crafted Primary");

            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 13;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/sniper_shoot");
            Item.autoReuse = true;
            Item.channel = true;

            Item.damage = 50;
            chargeUpDamage = Item.damage;
            Item.shoot = ModContent.ProjectileType<SydneySleeperDart>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.GetGlobalItem<TF2ItemBase>().noRandomCrits = true;

            Item.value = Item.buyPrice(platinum: 1, gold: 6);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "+25% charge rate\n"
                + "On Scoped Hit: Apply Jarate for 2 to 5 seconds\n"
                + "based on charge level.\n"
                + "Nature's Call: Scoped headshots always mini-crit\n"
                + "and reduce the remaining cooldown of Jarate by 1 second.")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "No random critical hits")
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
            if (chargeUpDelayTimer >= chargeUpDelay)
                player.scope = true;
            maxChargeUp = 90f;
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
                p.miniCrit = true;

            jarateDuration = 120 + (int)(180 * (chargeTime / maxChargeUp));
            isCharging = false;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.ModItem is SydneySleeper && isCharging)
            {
                player.moveSpeed -= 0.73f;
                player.GetModPlayer<TF2Player>().speedMultiplier -= 0.73f;
                player.GetModPlayer<TF2Player>().disableFocusSlowdown = true;
                isCharging = false;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SydneySleeperDart>(), chargeUpDamage, knockback, player.whoAmI);
            if (player.GetModPlayer<TF2Player>().focus)
            {
                Main.projectile[proj].GetGlobalProjectile<TF2ProjectileBase>().homing = true;
                Main.projectile[proj].GetGlobalProjectile<TF2ProjectileBase>().shootSpeed = Item.shootSpeed;
            }
            if (chargeTime == maxChargeUp)
                Main.projectile[proj].GetGlobalProjectile<TF2ProjectileBase>().sniperMiniCrit = true;
            Main.projectile[proj].GetGlobalProjectile<TF2ProjectileBase>().jarateDuration = jarateDuration;
            NetMessage.SendData(MessageID.SyncProjectile, number: proj);
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

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Huntsman>()
                .AddIngredient<ReclaimedMetal>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}