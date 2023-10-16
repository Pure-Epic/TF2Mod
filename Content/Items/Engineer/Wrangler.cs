using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Engineer;
using TF2.Common;

namespace TF2.Content.Items.Engineer
{
    public class Wrangler : TF2WeaponNoAmmo
    {
        public int rocketTimer;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Engineer's Unlocked Secondary\n"
                + "Take manual control of your Sentry Gun.\n"
                + "Wrangled sentries gain a shield that reduces damage and repairs by 66%.\n"
                + "Sentries are disabled for 3 seconds after becoming unwrangled.");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            TooltipLine tt2 = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt2);
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
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.channel = true;

            Item.damage = 0;
            Item.shoot = ModContent.ProjectileType<WranglerBeam>();
            Item.shootSpeed = 1f;
            Item.knockBack = 0;
            Item.crit = 0;

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Item.shootSpeed = 1f;
                Item.useTime = 0;
                Item.useAnimation = 0;
                Item.damage = 0;
                if (player.altFunctionUse != 2)
                {
                    Item.noUseGraphic = false;
                    Item.shoot = ModContent.ProjectileType<WranglerBeam>();
                }
                else
                {
                    Item.noUseGraphic = true;
                    Item.shoot = ProjectileID.None;
                }
            }
            else if (player.GetModPlayer<GunslingerPlayer>().gunslingerEquipped)
            {
                Item.shootSpeed = 10f;
                Item.useTime = 4;
                Item.useAnimation = 4;
                Item.damage = 8;
                Item.shoot = ModContent.ProjectileType<Projectiles.Bullet>();
            }
            else if (!Main.hardMode)
            {
                Item.shootSpeed = 10f;
                Item.useTime = 6;
                Item.useAnimation = 6;
                Item.damage = 16;
                Item.shoot = ModContent.ProjectileType<Projectiles.Bullet>();
            }
            else
            {
                Item.shootSpeed = 10f;
                Item.useTime = 3;
                Item.useAnimation = 3;
                Item.damage = 16;
                Item.shoot = ModContent.ProjectileType<Projectiles.Bullet>();
            }
            return base.CanUseItem(player);
        }

        public override void HoldItem(Player player)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
                rocketTimer++;
        }

        public override bool AltFunctionUse(Player player) => NPC.CountNPCS(ModContent.NPCType<NPCs.SentryLevel3>()) >= 1;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.classAccessory && !p.classHideVanity)
                Item.scale = 0f;
            else
                Item.scale = 1f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(0, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (Main.netMode == NetmodeID.SinglePlayer)
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            else if (player.altFunctionUse != 2)
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Projectiles.Bullet>(), (int)(16 * p.classMultiplier), knockback, player.whoAmI);
            if (Main.netMode != NetmodeID.SinglePlayer && NPC.downedMoonlord && rocketTimer >= 90 && player.altFunctionUse == 2)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15f));
                    Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Projectiles.NPCs.SentryRocket>(), (int)(100 * p.classMultiplier), knockback, player.whoAmI);
                }
                rocketTimer = 0;
            }
            return false;
        }

    }
}