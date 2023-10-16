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
using TF2.Content.Projectiles.Scout;

namespace TF2.Content.Items.Scout
{
    public class ForceANature : TF2Weapon
    {
        public int trueDamage;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Force-A-Nature");
            Tooltip.SetDefault("Scout's Unlocked Primary");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 17;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/scatter_gun_shoot");
            Item.autoReuse = true;

            Item.damage = 1;
            Item.shoot = ModContent.ProjectileType<ForceANatureBullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.knockBack = 20f;

            ammoCost = 1;
            maxAmmoClip = 2;
            ammoInClip = 2;
            reloadRate = 86f;
            magazine = true;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/scatter_gun_double_tube_reload");

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            if (tt != null)
            {
                trueDamage = Convert.ToInt32(5.4f * Main.LocalPlayer.GetModPlayer<TF2Player>().classMultiplier);
                tt.Text = Language.GetTextValue(trueDamage + " mercenary damage");
            }
            TooltipLine tt2 = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt2);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "+50% faster firing speed\n"
                + "Knockback on the target\n"
                + "+20% bullets per shot")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "-10% damage penalty\n"
                + "-66% clip size")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
        }

        public override void HoldItem(Player player)
        {
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

        public override bool CanConsumeAmmo(Item ammo, Player player) => ammoInClip > 0;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.classAccessory && !p.classHideVanity)
                Item.noUseGraphic = true;
            else
                Item.noUseGraphic = false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (ammoInClip == 0 || reload) return false;
            ammoInClip -= ammoCost;

            for (int i = 0; i < 12; i++)
            {
                int newDamage = Convert.ToInt32(5.4f * Main.LocalPlayer.GetModPlayer<TF2Player>().classMultiplier);
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(12f));
                Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<ForceANatureBullet>(), newDamage, knockback, player.whoAmI);
            }

            if (ammoInClip <= 0)
                reload = true;
            return false;
        }

        public override bool CanUseItem(Player player) => !(ammoInClip <= 0 || reload);
    }
}