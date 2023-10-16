using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items.Ammo;
using TF2.Content.Projectiles;
using TF2.Common;

namespace TF2.Content.Items.Heavy
{
    public class Minigun : TF2WeaponNoAmmo
    {
        public int spinTime = 0;
        public int bullets;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Heavy's Starter Primary");

            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = null;
            Item.autoReuse = true;

            Item.damage = 9;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();

            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.ModItem is Minigun && player.itemAnimation > 0)
            {
                player.moveSpeed -= 0.47f;
                player.GetModPlayer<TF2Player>().speedMultiplier -= 0.47f;
                player.GetModPlayer<TF2Player>().disableFocusSlowdown = true;
                if (!player.mount.Active)
                    player.controlJump = false;
            }
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
            if (!player.controlUseItem && Main.mouseRightRelease)
            {
                if (spinTime > 0)
                {
                    spinTime--;
                    player.itemAnimation = 6;
                }
                spinTime = Utils.Clamp(spinTime, 0, 53);
            }
            else
                spinTime++;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (spinTime >= 53)
            {
                if (bullets >= 4)
                {
                    bullets = 0;
                    return true;
                }
                return false;
            }
            else
                return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (spinTime >= 53 && Main.mouseLeft)
            {
                bullets++;
                SoundEngine.PlaySound(SoundID.Item11, player.Center);
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(10f));
                Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Bullet>(), damage, knockback, player.whoAmI);
            }
            else
                SoundEngine.PlaySound(SoundID.Item13, player.Center);
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10f, 15f);

        public override bool AltFunctionUse(Player player) => true;
    }
}