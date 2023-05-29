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
using TF2.Content.Projectiles;

namespace TF2.Content.Items.Scout
{
    public class Sandman : TF2WeaponMelee
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Scout's Unlocked Melee");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/melee_swing");
            Item.autoReuse = true;

            Item.damage = 35;

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "Alt-Fire: Launches a ball that slows opponents")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "-12% max health on wearer")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
        }

        public override void UpdateInventory(Player player)
        {
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].type == Type && !inHotbar)
                    inHotbar = true;
            }
            if (!inHotbar && !ModContent.GetInstance<TF2ConfigClient>().InventoryStats) return;
            player.statLifeMax2 = (int)(player.statLifeMax2 * 0.88f);
            inHotbar = false;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shootSpeed = 25f;
                Item.scale = 1f;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.noMelee = true;
                Item.useTime = 15;
                Item.useAnimation = 15;
                Item.shoot = ModContent.ProjectileType<Projectiles.Scout.Baseball>();
                return !player.GetModPlayer<Buffs.BaseballPlayer>().baseballCooldown;
            }
            else
            {
                Item.scale = 1f;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.noMelee = false;
                Item.useTime = 30;
                Item.useAnimation = 30;
                Item.shoot = ProjectileID.None;
                return true;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                int newDamage = (int)(15 * Main.LocalPlayer.GetModPlayer<TF2Player>().classMultiplier);
                var i = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Projectiles.Scout.Baseball>(), newDamage, knockback, player.whoAmI);
                if (player.GetModPlayer<TF2Player>().focus)
                {
                    Main.projectile[i].GetGlobalProjectile<TF2ProjectileBase>().homing = true;
                    Main.projectile[i].GetGlobalProjectile<TF2ProjectileBase>().shootSpeed = Item.shootSpeed;
                    NetMessage.SendData(MessageID.SyncProjectile, number: i);
                }

                player.AddBuff(ModContent.BuffType<Buffs.BaseballCooldown>(), 600);
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => true;
    }
}