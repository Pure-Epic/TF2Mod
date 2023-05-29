using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items.Ammo;
using TF2.Content.Projectiles;
using TF2.Common;

namespace TF2.Content.Items.Engineer
{
    public class FrontierJustice : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frontier Justice");
            Tooltip.SetDefault("Engineer's Unlocked Primary");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1f;
            Item.useTime = 37;
            Item.useAnimation = 37;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/shotgun_shoot");
            Item.autoReuse = true;

            Item.damage = 6;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.knockBack = 0;
            Item.crit = 0;
            Item.GetGlobalItem<TF2ItemBase>().noRandomCrits = true;

            ammoCost = 1;
            maxAmmoClip = 3;
            ammoInClip = 3;
            reloadRate = 30f;
            initialReloadRate = 60f;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/shotgun_reload");

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "Gain 3 revenge crits when your sentry is destroyed.\n"
                + "Gain a revenge crit every 3 times you get hit.\n"
                + "Has focus shot")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "-50% clip size\n"
                + "No random critical hits\n"
                + "Revenge crits are lost on death")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
        }

        public override void HoldItem(Player player)
        {
            WeaponSystem clip = player.GetModPlayer<WeaponSystem>();
            FrontierJusticePlayer f = player.GetModPlayer<FrontierJusticePlayer>();
            clip.ammoMax = maxAmmoClip;
            clip.ammoReloadRate = reloadRate;
            clip.initialAmmoReloadRate = initialReloadRate;
            clip.ammoCurrent = ammoInClip;
            if (clip.startReload)
            {
                reload = true;
                clip.startReload = false;
            }
            UpdateResource();
        }

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
            reload = false;
            ammoInClip -= ammoCost;

            for (int i = 0; i < 10; i++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(12f));
                var j = Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Bullet>(), damage, knockback, player.whoAmI);

                if (player.GetModPlayer<TF2Player>().focus)
                {
                    Main.projectile[j].GetGlobalProjectile<TF2ProjectileBase>().homing = true;
                    Main.projectile[j].GetGlobalProjectile<TF2ProjectileBase>().shootSpeed = Item.shootSpeed;
                    NetMessage.SendData(MessageID.SyncProjectile, number: j);
                }

                FrontierJusticePlayer f = player.GetModPlayer<FrontierJusticePlayer>();
                if (f.revenge > 0)
                {
                    Main.projectile[j].GetGlobalProjectile<TF2ProjectileBase>().crit = true;
                    f.revenge--;
                }
            }
            if (ammoInClip <= 0)
                reload = true;
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(0, 0);
    }

    public class FrontierJusticePlayer : ModPlayer
    {
        public int revenge = 0;
        public int hitCounter = 0;

        public override void PreUpdate()
        {
            if (revenge < 0)
                revenge = 0;
        }

        public override void OnRespawn(Player player) => revenge = 0;

        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
                revenge++;
            else
            {
                hitCounter++;
                if (hitCounter >= 3)
                {
                    revenge++;
                    hitCounter = 0;
                }
            }
        }
    }
}