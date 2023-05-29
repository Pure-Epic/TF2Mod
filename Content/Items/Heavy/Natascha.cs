using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Ammo;
using TF2.Content.Projectiles.Heavy;

namespace TF2.Content.Items.Heavy
{
    public class Natascha : TF2WeaponNoAmmo
    {
        public int spinTime = 0;
        public int bullets;
        public int trueDamage;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Heavy's Unlocked Primary");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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

            Item.damage = 1;
            Item.shoot = ModContent.ProjectileType<NataschaBullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) // needs System.Linq
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            if (tt != null)
            {
                trueDamage = Convert.ToInt32(6.75f * Main.LocalPlayer.GetModPlayer<TF2Player>().classMultiplier);
                tt.Text = Language.GetTextValue(trueDamage + " mercenary damage");
            }
            TooltipLine tt2 = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt2);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "On Hit: 100% chance to slow target\n"
                + "-20% damage resistance when below 50% health and spun up")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "-25% damage penalty\n"
                + "30% slower spin up time")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.ModItem is Natascha && player.controlUseItem && player.statLife < player.statLifeMax2 / 2)
                player.GetModPlayer<TF2Player>().damageReduction += 0.2f;

            if (player.HeldItem.ModItem is Natascha && player.itemAnimation > 0)
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
                spinTime = Utils.Clamp(spinTime, 0, 70);
            }
            else
                spinTime++;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (spinTime >= 70)
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
            if (spinTime >= 70 && Main.mouseLeft)
            {
                bullets++;
                SoundEngine.PlaySound(SoundID.Item11, player.Center);
                int newDamage = Convert.ToInt32(6.75f * Main.LocalPlayer.GetModPlayer<TF2Player>().classMultiplier);
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(10f));
                Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<NataschaBullet>(), newDamage, knockback, player.whoAmI);
            }
            else
                SoundEngine.PlaySound(SoundID.Item13, player.Center);
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10f, 15f);

        public override bool AltFunctionUse(Player player) => true;
    }
}